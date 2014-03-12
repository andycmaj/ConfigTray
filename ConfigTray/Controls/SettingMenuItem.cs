using System;
using System.Windows.Forms;
using ConfigTray.Configuration;

namespace ConfigTray.Controls
{
    public abstract class GenericSettingMenuItem<TSetting> : SettingMenuItem
        where TSetting : Setting
    {
        public TSetting Setting
        {
            get;
            set;
        }

        private delegate void DoRefreshSetting();

        protected GenericSettingMenuItem(TSetting setting)
        {
            ChangeIsExternal = true;

            Setting = setting;
            Setting.ExternalSettingChange += OnExternalSettingChange;

            Text = Setting.Description;

            // @TODO: Weirdness when the menu item is agains the bottom of the screen. 
            //  Tooltip shows under cursor, firing MouseLeave for the MenuItem. 
            //  Tooltip then disappears, firing MouseEnter for MenuItem, causing Tooltip to show and so on...
            //ToolTipText = Setting.ValueXPath;
            
            InitializeMenuItem();

            ChangeIsExternal = false;
        }

        protected void OnExternalSettingChange(Setting newSetting)
        {
            Setting = (TSetting)newSetting;

            ChangeIsExternal = true;

            if (Application.OpenForms[0].InvokeRequired)
            {
                Application.OpenForms[0].Invoke(new DoRefreshSetting(RefreshSetting));
            }
            else
            {
                RefreshSetting();
            }

            ChangeIsExternal = false;
        }

        protected abstract void InitializeMenuItem();

        protected abstract void RefreshSetting();
    }

    public abstract class SettingMenuItem : ToolStripMenuItem
    {
        public event Action<SettingChangeEventArgs> UserChangedSetting;

        /// <summary>
        /// Gets or sets a value indicating whether a change in CheckedState
        /// is coming from an external source. True if external, false if user
        /// triggered the change from this app.
        /// </summary>
        /// <remarks>
        /// Use this when changing CheckedState for an observed value. This flag
        /// is used to prevent the OnCheckedChanged event handler from taking
        /// any action unless user actually checks an item.
        /// </remarks>
        protected bool ChangeIsExternal
        {
            get;
            set;
        }

        public SettingMenuItem()
        {
            //DropDownDirection = ToolStripDropDownDirection.Left;
            //RightToLeft = RightToLeft.No;
        }

        protected void OnUserChangedSetting(SettingChangeEventArgs e)
        {
            if (UserChangedSetting != null)
            {
                UserChangedSetting(e);
            }
        }
    }
}
