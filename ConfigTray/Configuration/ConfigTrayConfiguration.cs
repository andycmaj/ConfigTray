using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using ConfigTray.Extensibility;
using System.Text;

namespace ConfigTray.Configuration
{
    [XmlRoot("trayConfig")]
    public class ConfigTrayConfiguration : IConfigurationSectionHandler, IConfiguration
    {
        private const string c_defaultEditorPath = "%windir%\\notepad.exe";

        private static ConfigTrayConfiguration s_instance;

        private string m_editorPath;

        /// <summary>
        /// Gets an instance of the RewriterConfiguration class with the values 
        /// populated from the Web.config file. Store in Cache for use later.
        /// </summary>
        public static ConfigTrayConfiguration Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = (ConfigTrayConfiguration)ConfigurationManager.GetSection("trayConfig");
                }

                return s_instance;
            }
        }

        #region Configuration

        /// <summary>
        /// Gets or sets the path to the program to use when opening config files.
        /// </summary>
        [XmlAttribute("editor")]
        public string EditorPath
        {
            get
            {
                return m_editorPath ?? c_defaultEditorPath;
            }

            set
            {
                m_editorPath = value;
            }
        }

        /// <summary>
        /// Gets or sets the regex pattern to use when capturing version number.
        /// </summary>
        [XmlAttribute("versionRegex")]
        public string VersionCaptureRegex { get; set; }

        /// <summary>
        /// Gets or sets a value that determines whether or not to show the external change notification.
        /// </summary>
        [XmlAttribute("showChangeNotifications")]
        public bool ShowChangeNotifications { get; set; }

        /// <summary>
        /// Gets or sets the collection of setting types from the application config.
        /// </summary>
        [XmlArray("addins")]
        [XmlArrayItem("addin")]
        public Collection<AddIn> AddIns { get; set; }

        [XmlArray("version")]
        [XmlArrayItem("file", typeof(FileVersionSource))]
        [XmlArrayItem("webpage", typeof(WebPageVersionSource))]
        public Collection<VersionSource> Version { get; set; }

        /// <summary>
        /// Gets or sets the collection of setting types from the application config.
        /// </summary>
        [XmlArray("settings")]
        [XmlArrayItem("toggle", typeof(ToggleSetting))]
        [XmlArrayItem("choice", typeof(ChoiceSetting))]
        [XmlArrayItem("literal", typeof(LiteralSetting))]
        public Collection<Setting> Settings { get; set; }

        /// <summary>
        /// Gets or sets the set of config files to load.
        /// </summary>
        [XmlArray("files")]
        [XmlArrayItem("file", typeof(ConfigFile))]
        public Collection<ConfigFile> Files { get; set; }

        /// <summary>
        /// Gets or sets the collection of UserDefinedTools to load.
        /// </summary>
        [XmlArray("tools")]
        [XmlArrayItem("tool", typeof(UserDefinedTool))]
        public Collection<UserDefinedTool> Tools { get; set; }

        public Setting this[string settingName]
        {
            get
            {
                Setting setting = (from s in Settings
                                   where s.Name == settingName
                                   select s).SingleOrDefault();

                if (setting == null)
                {
                    throw new InvalidOperationException(string.Format("Setting <{0}> does not exist in configuration.", settingName));
                }

                return setting;
            }
        }

        #endregion

        #region Management

        public static void Refresh()
        {
            ConfigurationManager.RefreshSection("trayConfig");

            string xml = ConfigurationManager.OpenExeConfiguration("configtray.exe").GetSection("trayConfig").SectionInformation.GetRawXml();

            s_instance = (ConfigTrayConfiguration)Create(xml);
        }

        public static void Refresh(ConfigTrayConfiguration newConfig)
        {
            newConfig.Save();
            ConfigurationManager.RefreshSection("trayConfig");
            s_instance = null;
        }

        public static ConfigTrayConfiguration Clone()
        {
            return (ConfigTrayConfiguration)ConfigurationManager.GetSection("trayConfig");
        }

        public void Save()
        {
            string xml = Serialize();

            System.Configuration.Configuration appConfig = ConfigurationManager.OpenExeConfiguration("configtray.exe");
            ConfigurationSection section = appConfig.GetSection("trayConfig");//Activator.CreateInstance(appConfig.GetSection("trayConfig").GetType()) as ConfigurationSection;
            appConfig.Sections.Remove("trayConfig");
            section.SectionInformation.SetRawXml(xml);
            appConfig.Sections.Add("trayConfig", section);
            appConfig.Save();
        }

        private string Serialize()
        {
            XmlWriterSettings settings = new XmlWriterSettings { OmitXmlDeclaration = true };
            string xml;
            using (MemoryStream stream = new MemoryStream())
            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ConfigTrayConfiguration), new Type[] { typeof(ToggleSetting), typeof(ChoiceSetting), typeof(LiteralSetting) });
                serializer.Serialize(writer, this);
                xml = Encoding.UTF8.GetString(stream.GetBuffer());
            }
            return xml.Substring(1);
        }

        #endregion

        #region IConfigurationSectionHandler Members

        public object Create(object parent, object configContext, XmlNode section)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ConfigTrayConfiguration), new Type[] { typeof(ToggleSetting), typeof(ChoiceSetting), typeof(LiteralSetting) });
            return serializer.Deserialize(new XmlNodeReader(section));
        }

        public static object Create(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ConfigTrayConfiguration), new Type[] { typeof(ToggleSetting), typeof(ChoiceSetting), typeof(LiteralSetting) });
            return serializer.Deserialize(new StringReader(xml));
        }

        #endregion
    }
}
