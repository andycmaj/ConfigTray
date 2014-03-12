using System;
using System.Diagnostics;
using System.Windows.Forms;
using ConfigTray.Configuration;

namespace ConfigTray.Controls
{
    public class ConfigFileMenuItem : ToolStripMenuItem
    {
        public event Action<ConfigFile, Setting> ExternalChange;

        public ConfigFile File { get; set; }

        public ConfigFileMenuItem(ConfigFile configFile)
        {
            File = configFile;

            Text = File.Name;

            InitializeSettingItems();
        }

        private void InitializeSettingItems()
        {
            CreateOpenFileItem();
            CreateTouchItem();

            DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem titleItem = new ToolStripMenuItem() {
                Text = "Settings",
                Enabled = false,
                Alignment = ToolStripItemAlignment.Left,
                Font = new System.Drawing.Font(Font, System.Drawing.FontStyle.Bold)
            };
            DropDownItems.Add(titleItem);

            foreach (Setting setting in File.Settings)
            {
                if (setting.HasValue)
                {
                    setting.ExternalSettingChange += OnExternalSettingChange;
                }

                try
                {
                    SettingMenuItem item = SettingMenuItemFactory.CreateMenuItem(setting);
                    item.UserChangedSetting += OnUserChangedSetting;

                    DropDownItems.Add(item);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Could not load setting <{0}> for file <{1}>. Ensure configured setting values match.", setting.Name, File.Name));
                }
            }
        }

        private void CreateTouchItem()
        {
            ToolStripMenuItem gotoItem = new ToolStripMenuItem() {
                Text = "Touch file"
            };
            gotoItem.Click += (sender, e) => {
                File.TouchFile();
            };
            DropDownItems.Add(gotoItem);
        }

        private void CreateOpenFileItem()
        {
            ToolStripMenuItem gotoItem = new ToolStripMenuItem() {
                Text = "Open file",
                ToolTipText = File.Path
            };
            gotoItem.Click += (sender, e) => {
                OpenFile();
            };
            DropDownItems.Add(gotoItem);
        }

        private void OnExternalSettingChange(Setting setting)
        {
            if (ExternalChange != null)
            {
                ExternalChange(File, setting);
            }
        }

        private void OnUserChangedSetting(SettingChangeEventArgs e)
        {
            try
            {
                File.SetValue(e.ChangedSetting);
                e.WasSuccessful = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not change setting value.", "Error changing setting value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.WasSuccessful = false;
            }
        }

        private void OpenFile()
        {
            string editorPath = Environment.ExpandEnvironmentVariables(ConfigTrayConfiguration.Instance.EditorPath);

            if (!System.IO.File.Exists(editorPath))
            {
                MessageBox.Show(string.Format("Default editor not found: {0}", editorPath));
                return;
            }

            new Process() {
                StartInfo = new ProcessStartInfo() {
                    Arguments = File.Path,
                    FileName = editorPath
                }
            }.Start();
        }
    }
}
