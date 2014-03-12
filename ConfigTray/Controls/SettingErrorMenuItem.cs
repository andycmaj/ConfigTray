using System.Drawing;
using System.Windows.Forms;
using ConfigTray.Configuration;
using ConfigTray.Properties;

namespace ConfigTray.Controls
{
    public class SettingErrorMenuItem : SettingMenuItem
    {
        private Setting _setting;

        public SettingErrorMenuItem(Setting setting)
        {
            _setting = setting;

            Text = _setting.Description;
            ForeColor = Color.Red;
            Image = Resources.Warning;

            InitializeMenuItem();
        }

        protected void InitializeMenuItem()
        {
            ToolStripMenuItem valueTextBox = new ToolStripMenuItem() {
                AutoSize = true,
                Text = "Value not found. Check setting xpath.",
                RightToLeft = RightToLeft.No,
                Enabled = false
            };

            DropDownItems.Add(valueTextBox);
        }
    }
}
