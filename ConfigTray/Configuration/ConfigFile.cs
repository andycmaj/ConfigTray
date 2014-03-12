using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using NLog;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace ConfigTray.Configuration
{
    [XmlRoot("file")]
    public class ConfigFile
    {
        #region Fields

        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private static List<string> s_disconnectedMachines = new List<string>();

        private FileSystemWatcher m_watcher;

        private Collection<Setting> m_settings;

        private string m_path;
        private bool m_watchForChanges;

        private XmlDocument m_xpDoc;
        private XPathNavigator m_xpNav;

        public event Action<ConfigFile> ExternalFileChange;

        #endregion

        #region Properties

        [XmlAttribute("name")]
        [Category(Constants.CATEGORY_FILE), Description("Name to display for this file."), Browsable(true), ReadOnly(false)]
        public string Name { get; set; }

        [XmlAttribute("watchForChanges")]
        [Category(Constants.CATEGORY_FILE), Description("Specifies whether settings defined for this file are automatically updated when the file changes."), Browsable(true), ReadOnly(false)]
        public bool WatchForChanges
        {
            get
            {
                return m_watchForChanges;
            }

            set
            {
                if (m_watcher != null)
                {
                    m_watcher.EnableRaisingEvents = value;
                }

                m_watchForChanges = value;
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public bool Exists { get; private set; }

        [XmlAttribute("path")]
        [Category(Constants.CATEGORY_FILE), Description("Path to the file."), Browsable(true), ReadOnly(false)]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string Path
        {
            get
            {
                return m_path;
            }

            set
            {
                if (m_path != value)
                {
                    m_path = System.IO.Path.GetFullPath(value);
                    string root = System.IO.Path.GetPathRoot(m_path);

                    if (s_disconnectedMachines.Contains(root))
                    {
                        Exists = false;
                    }
                    else if (!Directory.Exists(root))
                    {
                        Exists = false;
                        s_disconnectedMachines.Add(root);
                    }
                    else if (File.Exists(value))
                    {
                        Exists = true;

                        string directory = System.IO.Path.GetDirectoryName(m_path);
                        string filename = System.IO.Path.GetFileName(m_path);

                        m_watcher = new FileSystemWatcher(directory, filename);
                        m_watcher.Changed += OnFileChanged;
                        m_watcher.NotifyFilter = NotifyFilters.LastWrite;
                        m_watcher.EnableRaisingEvents = WatchForChanges;
                    }
                    else
                    {
                        Exists = false;
                    }
                }
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public string MachineName
        {
            get
            {
                if (Path.StartsWith("\\\\"))
                {
                    return Path.Substring(2).Split("\\".ToCharArray())[0];
                }
                else
                {
                    return Environment.MachineName;
                }
            }
        }

        /// <summary>
        /// Gets or sets the list of setting names to load for this config file.
        /// </summary>
        [XmlArray("settings")]
        [XmlArrayItem("settingName")]
        [Category(Constants.CATEGORY_FILE), Description("Settings controlled by ConfigTray for this file."), Browsable(true), ReadOnly(false)]
        public Collection<string> SettingNames { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public Collection<Setting> Settings
        {
            get
            {
                if (m_settings == null)
                {
                    m_settings = LoadSettings();
                }

                return m_settings;
            }
        }

        #endregion

        public ConfigFile()
        {
            SettingNames = new Collection<string>();
        }

        private Collection<Setting> LoadSettings()
        {
            //TODO: Change to foreach loop. That will allow handling of exceptions...
            var settings = from sName in SettingNames
                           select Setting.Load(ConfigTrayConfiguration.Instance[sName], this);

            return new Collection<Setting>(settings.ToList());
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            s_logger.Debug("File change observed: {0}", m_path);
            if (ExternalFileChange != null)
            {
                ExternalFileChange(this);
            }
        }

        public void SetValue(Setting setting)
        {
            DoWithoutWatching(() => {
                string oldValue = GetValue(setting);

                XPathNavigator valueNavigator = GetXPathNavigator();
                try
                {
                    if (string.IsNullOrEmpty(setting.XmlNamespace))
                    {
                        valueNavigator.SelectSingleNode(setting.ValueXPath).SetTypedValue(setting.Value);
                    }
                    else
                    {
                        XmlNamespaceManager namespaceManager = new XmlNamespaceManager(valueNavigator.NameTable);
                        namespaceManager.AddNamespace("ns", setting.XmlNamespace);

                        valueNavigator.SelectSingleNode(setting.ValueXPath, namespaceManager).SetTypedValue(setting.Value);
                    }

                    m_xpDoc.Save(Path);

                    s_logger.Info("Modified <{0}>, {1}: {2} -> {3}", Name, setting.Name, oldValue, setting.Value);
                }
                catch (Exception ex)
                {
                    s_logger.ErrorException(string.Format("Error modifying <{0}>.", Name), ex);
                    throw;
                }
            });
        }

        public string GetValue(Setting setting)
        {
            return GetValue(setting, false);
        }

        public string GetValue(Setting setting, bool refreshFile)
        {
            if (refreshFile)
            {
                m_xpNav = null;
            }

            //m_watcher.EnableRaisingEvents = false;

            XPathNavigator valueNavigator = GetXPathNavigator();

            string value;
            if (string.IsNullOrEmpty(setting.XmlNamespace))
            {
                var node = valueNavigator.SelectSingleNode(setting.ValueXPath);
                if (node == null)
                {
                    return null;
                }

                value = node.Value.Trim();
            }
            else
            {
                XmlNamespaceManager namespaceManager = new XmlNamespaceManager(valueNavigator.NameTable);
                namespaceManager.AddNamespace("ns", setting.XmlNamespace);

                value = valueNavigator.SelectSingleNode(setting.ValueXPath, namespaceManager).Value.Trim();
            }

            //m_watcher.EnableRaisingEvents = true;

            return value;
        }

        private XPathNavigator GetXPathNavigator()
        {
            if (m_xpNav == null)
            {
                try
                {
                    m_xpDoc = new XmlDocument();
                    m_xpDoc.Load(Path);
                    m_xpNav = m_xpDoc.CreateNavigator();

                    //m_xpDoc.Schemas.Add("ns", "http://scripts.live.com/namespaces/ExFxFEConfiguration.xsd");
                }
                catch (Exception ex)
                {
                    s_logger.ErrorException(string.Format("Error loading <{0}>.", Name), ex);
                    throw;
                }
            }

            return m_xpNav;
        }

        public void TouchFile()
        {
            DoWithoutWatching(() => {
                File.SetLastWriteTime(Path, DateTime.Now);
            });
        }

        public void DoWithoutWatching(Action action)
        {
            m_watcher.EnableRaisingEvents = false;

            action();

            m_watcher.EnableRaisingEvents = WatchForChanges;
        }

        public override string ToString()
        {
            return string.Format("{0} : {1}", MachineName, Name);
        }
    }
}
