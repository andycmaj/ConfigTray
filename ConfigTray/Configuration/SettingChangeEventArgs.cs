using System;

namespace ConfigTray.Configuration
{
    public class SettingChangeEventArgs
    {
        public Setting ChangedSetting { get; set; }

        public bool WasSuccessful { get; set; }
    }
}
