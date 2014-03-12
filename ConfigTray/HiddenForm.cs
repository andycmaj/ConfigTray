using System;
using System.Linq;
using System.Windows.Forms;

using ConfigTray.Configuration;
using ConfigTray.Controls;
using ConfigTray.Properties;

namespace ConfigTray
{
    public partial class HiddenForm : Form
    {
        private ContextMenuStrip m_contextMenu;
        private delegate void DoShowBalloon(ConfigFile file, Setting setting);

        public event EventHandler<InitializedEventArgs> Initialized;
        public event Action<InitializationStepEventArgs> Initializing;

        public HiddenForm(EventHandler<InitializedEventArgs> initializedHandler, Action<InitializationStepEventArgs> initializingHandler)
        {
            Initialized += initializedHandler;
            Initializing += initializingHandler;
            InitializeComponent();

            //Create context menu.
            m_contextMenu = new ContextMenuStrip() {
                RightToLeft = RightToLeft.Yes
            };

            InitiaizeMenus();
            InitiaizeTrayIcon();

            if (Initialized != null)
            {
                Initialized(this, new InitializedEventArgs());
            }
        }

        private void InitiaizeMenus()
        {
            m_contextMenu.Items.Clear();

            Initializing("Loading Configuration Files...");
            string dummy = ConfigTrayConfiguration.Instance.ToString();

            Initializing("Populating Version MenuItem...");
            CreateVersionItem();

            Initializing("Loading Tools...");
            CreateTools();

            Initializing("Loading Addins...");
            CreateAddIns();

            Initializing("Creating menu items...");
            CreateFileItems();

            //Add configTrayConfig item.
            ToolStripMenuItem editConfigItem = new ToolStripMenuItem("Edit ConfigTray Config");
            editConfigItem.Click += EditConfigMenuItem_Click;
            m_contextMenu.Items.Add(editConfigItem);

            //Add quit item.
            ToolStripMenuItem quitItem = new ToolStripMenuItem("Quit");
            quitItem.Click += QuitMenuItem_Click;
            m_contextMenu.Items.Add(quitItem);
        }

        void EditConfigMenuItem_Click(object sender, EventArgs e)
        {
            using (EditConfig editor = new EditConfig(ConfigTrayConfiguration.Clone()))
            {
                DialogResult result = editor.ShowDialog();
                if (result == DialogResult.OK)
                {
                    MessageBox.Show("Reloading ConfigTray config and refreshing menu items.");

                    ConfigTrayConfiguration configBackup = ConfigTrayConfiguration.Instance;
                    try
                    {
                        ConfigTrayConfiguration.Refresh(editor.ModifiedConfig);
                    }
                    catch (Exception ex)
                    {
                        ConfigTrayConfiguration.Refresh(configBackup);
                    }
                    InitiaizeMenus();
                }
            }
        }

        private void CreateTools()
        {
            bool hasTools = false;

            foreach (UserDefinedTool tool in ConfigTrayConfiguration.Instance.Tools)
            {
                m_contextMenu.Items.Add(new UserDefinedToolMenuItem(tool));

                hasTools = true;
            }

            if (hasTools)
            {
                m_contextMenu.Items.Add(new ToolStripSeparator());
            }
        }

        private void CreateAddIns()
        {
            bool hasAddIns = false;

            foreach (AddIn addin in ConfigTrayConfiguration.Instance.AddIns)
            {
                m_contextMenu.Items.Add(addin.CreateMenuItem());

                hasAddIns = true;
            }

            if (hasAddIns)
            {
                m_contextMenu.Items.Add(new ToolStripSeparator());
            }
        }

        private void CreateFileItems()
        {
            bool hasFiles = false;

            var machineGroups = from file in ConfigTrayConfiguration.Instance.Files
                                group file by file.MachineName into machineGroup
                                select machineGroup;

            if (machineGroups.Count() > 1)
            {
                //Config files found for more than one machine. Build menus per machine.
                foreach (IGrouping<string, ConfigFile> group in machineGroups)
                {
                    ToolStripMenuItem machineMenuItem = new ToolStripMenuItem(group.Key);

                    RemoteExecuteMenuItem remoteExecItem = new RemoteExecuteMenuItem(group.Key);
                    machineMenuItem.DropDownItems.Add(remoteExecItem);
                    machineMenuItem.DropDownItems.Add(new ToolStripSeparator());

                    foreach (ConfigFile file in group)
                    {
                        machineMenuItem.DropDownItems.Add(CreateFileMenuItem(file));

                        hasFiles = true;
                    }

                    m_contextMenu.Items.Add(machineMenuItem);
                }
            }
            else
            {
                //All files are located on one machine.
                foreach (ConfigFile file in ConfigTrayConfiguration.Instance.Files)
                {
                    m_contextMenu.Items.Add(CreateFileMenuItem(file));

                    hasFiles = true;
                }
            }

            if (hasFiles)
            {
                m_contextMenu.Items.Add(new ToolStripSeparator());
            }
        }

        private ToolStripMenuItem CreateFileMenuItem(ConfigFile file)
        {
            ToolStripMenuItem item;
            if (file.Exists)
            {
                item = new ConfigFileMenuItem(file);

                if (ConfigTrayConfiguration.Instance.ShowChangeNotifications)
                {
                    ((ConfigFileMenuItem)item).ExternalChange += ConfigFile_OnExternalChange;
                }
            }
            else
            {
                item = new FileNotFoundMenuItem(file);
            }

            return item;
        }

        private void CreateVersionItem()
        {
            if (ConfigTrayConfiguration.Instance.Version != null)
            {
                foreach (VersionSource version in ConfigTrayConfiguration.Instance.Version)
                {
                    try
                    {
                        VersionMenuItem versionItem = new VersionMenuItem(version);
                        m_contextMenu.Items.Add(versionItem);

                        //If we have found a successful version source, don't try the rest.
                        break;
                    }
                    catch
                    {
                        //If there is an exception thrown while trying to get the version from the versionSource, try the next versionSource.
                        continue;
                    }
                }

                m_contextMenu.Items.Add(new ToolStripSeparator());
            }
        }

        private void InitiaizeTrayIcon()
        {
            TrayIcon.Visible = true;
            TrayIcon.Icon = Resources.TrayIcon;
            TrayIcon.ContextMenuStrip = m_contextMenu;
        }

        private void ConfigFile_OnExternalChange(ConfigFile file, Setting setting)
        {
            if (InvokeRequired)
            {
                Invoke(new DoShowBalloon(ShowBalloon), new object[] { file, setting });
            }
            else
            {
                ShowBalloon(file, setting);
            }
        }

        private void ShowBalloon(ConfigFile file, Setting setting)
        {
            TrayIcon.ShowBalloonTip(3 * 1000,
                    "Config change observed",
                    GetBalloonText(file, setting),
                    ToolTipIcon.Info);
        }

        private static string GetBalloonText(ConfigFile file, Setting setting)
        {
            return string.Format("Config file: {0}\nSetting changed: {1}\nCurrent value: {2}", file.Name, setting.Name, setting.Value);
        }

        private void QuitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void OnClosed(EventArgs e)
        {
            TrayIcon.RemoveIcon();
            base.OnClosed(e);
        }
    }
}
