using System;
using System.Windows.Forms;
using ConfigTray.Configuration;

namespace ConfigTray.Controls
{
    public class LiteralSettingMenuItem : GenericSettingMenuItem<LiteralSetting>
    {
        public LiteralSettingMenuItem(LiteralSetting setting)
            : base(setting)
        {
        }

        protected override void InitializeMenuItem()
        {
            ToolStripTextBox valueTextBox = new ToolStripTextBox() {
                AutoSize = false,
                Text = Setting.Value,
                TextBoxTextAlign = HorizontalAlignment.Right
            };
            valueTextBox.TextBox.Width = 250;
            valueTextBox.TextBox.TextAlign = HorizontalAlignment.Right;
            valueTextBox.LostFocus += valueTextBox_LostFocus;

            DropDownItems.Add(valueTextBox);
        }

        void valueTextBox_LostFocus(object sender, EventArgs e)
        {
            if (ChangeIsExternal)
            {
                return;
            }

            string oldValue = Setting.Value;
            Setting.Value = ((ToolStripTextBox)sender).Text;

            SettingChangeEventArgs args = new SettingChangeEventArgs {
                ChangedSetting = Setting
            };
            OnUserChangedSetting(args);

            if (!args.WasSuccessful)
            {
                Setting.Value = oldValue;
                ((ToolStripTextBox)sender).Text = oldValue;
            }
        }

        protected override void RefreshSetting()
        {
            ToolStripTextBox valueTextBox = (ToolStripTextBox)DropDownItems[0];

            valueTextBox.Text = Setting.Value;
        }
    }
}
