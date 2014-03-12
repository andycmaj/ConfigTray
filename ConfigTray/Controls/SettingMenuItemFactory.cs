using System;
using ConfigTray.Configuration;

namespace ConfigTray.Controls
{
    internal static class SettingMenuItemFactory
    {
        public static SettingMenuItem CreateMenuItem(Setting setting)
        {
            if (!setting.HasValue)
            {
                return CreateMenuItem<SettingErrorMenuItem>(setting);
            }
            else if (setting is ToggleSetting)
            {
                return CreateMenuItem<ToggleSettingMenuItem>(setting);
            }
            else if (setting is ChoiceSetting)
            {
                return CreateMenuItem<ChoiceSettingMenuItem>(setting);
            }
            else if (setting is LiteralSetting)
            {
                return CreateMenuItem<LiteralSettingMenuItem>(setting);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static TMenuItem CreateMenuItem<TMenuItem>(Setting setting)
        {
            return (TMenuItem)Activator.CreateInstance(typeof(TMenuItem), setting);
        }
    }
}
