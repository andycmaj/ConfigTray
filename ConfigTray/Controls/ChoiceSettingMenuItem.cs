using System;
using System.Windows.Forms;
using ConfigTray.Configuration;

namespace ConfigTray.Controls
{
    public class ChoiceSettingMenuItem : GenericSettingMenuItem<ChoiceSetting>
    {
        private ToolStripMenuItem m_selectedMenuItem;

        public ChoiceSettingMenuItem(ChoiceSetting setting)
            : base(setting)
        {
        }

        protected override void InitializeMenuItem()
        {
            ChangeIsExternal = true;

            foreach (string possibleValue in Setting.PossibleValues)
            {
                ToolStripMenuItem valueItem = new ToolStripMenuItem() {
                    Checked = Setting.Value == possibleValue,
                    CheckOnClick = true,
                    Tag = possibleValue,
                    Text = possibleValue
                };

                if (valueItem.Checked)
                {
                    m_selectedMenuItem = valueItem;
                }

                valueItem.CheckedChanged += ValueItem_OnCheckedChanged;

                DropDownItems.Add(valueItem);
            }

            ChangeIsExternal = false;
        }

        private void ValueItem_OnCheckedChanged(object sender, EventArgs e)
        {
            if (ChangeIsExternal || sender == m_selectedMenuItem)
            {
                //If setting was changed externally or is being unchecked, do not continue.
                return;
            }

            ToolStripMenuItem pendingItem = (ToolStripMenuItem)sender;
            string oldValue = Setting.Value;
            Setting.Value = (string)pendingItem.Tag;

            //Try to update setting in file.
            SettingChangeEventArgs args = new SettingChangeEventArgs {
                ChangedSetting = Setting
            };
            OnUserChangedSetting(args);

            if (args.WasSuccessful)
            {
                base.OnCheckedChanged(e);

                //Uncheck selected choice.
                m_selectedMenuItem.Checked = false;
                m_selectedMenuItem = pendingItem;
            }
            else
            {
                ChangeIsExternal = true;
                pendingItem.Checked = false;
                Setting.Value = oldValue;
                ChangeIsExternal = false;
            }
        }

        protected override void RefreshSetting()
        {
            //Uncheck selected menu item.
            m_selectedMenuItem.Checked = false;

            //Find menu item which represents new Setting and select it.
            foreach (ToolStripMenuItem valueItem in DropDownItems)
            {
                if ((string)valueItem.Tag == Setting.Value)
                {
                    m_selectedMenuItem = valueItem;

                    m_selectedMenuItem.Checked = true;
                }
            }
        }
    }
}
