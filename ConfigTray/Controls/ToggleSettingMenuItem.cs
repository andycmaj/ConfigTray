using System;
using ConfigTray.Configuration;

namespace ConfigTray.Controls
{
    public class ToggleSettingMenuItem : GenericSettingMenuItem<ToggleSetting>
    {
        public ToggleSettingMenuItem(ToggleSetting setting)
            : base(setting)
        {
        }

        protected override void InitializeMenuItem()
        {
            Checked = Setting.IsOn;
            CheckOnClick = true;
        }

        protected override void OnCheckedChanged(EventArgs e)
        {
            base.OnCheckedChanged(e);

            if (ChangeIsExternal)
            {
                return;
            }

            Setting.IsOn = Checked;

            SettingChangeEventArgs args = new SettingChangeEventArgs {
                ChangedSetting = Setting
            };
            OnUserChangedSetting(args);

            if (!args.WasSuccessful)
            {
                //Rollback
                ChangeIsExternal = true;
                Checked = !Checked;
                Setting.IsOn = !Setting.IsOn;
                ChangeIsExternal = false;
            }
        }

        protected override void RefreshSetting()
        {
            Checked = Setting.IsOn;
        }
    }
}
