using System.Drawing;
using System.Windows.Forms;
using ConfigTray.Configuration;
using ConfigTray.Properties;

namespace ConfigTray.Controls
{
    public class FileNotFoundMenuItem : ToolStripMenuItem
    {
        public ConfigFile File { get; set; }

        public FileNotFoundMenuItem(ConfigFile configFile)
        {
            File = configFile;

            Text = string.Format(File.Name);
            ForeColor = Color.Red;
            Image = Resources.Warning;

            InitializeMenuItem();
        }

        protected void InitializeMenuItem()
        {
            ToolStripMenuItem valueTextBox = new ToolStripMenuItem() {
                AutoSize = true,
                Text = "File not found. Check path.",
                RightToLeft = RightToLeft.No,
                Enabled = false
            };

            DropDownItems.Add(valueTextBox);
        }
    }
}
