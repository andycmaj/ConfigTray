using System;
using System.Runtime.InteropServices;

namespace ConfigTray.Win32Api
{
    /// Win32 API imports.
    /// </summary>
    public static class Win32Api
    {
        /// <summary>
        /// Creates, updates or deletes the taskbar icon.
        /// </summary>
        [DllImport("shell32.Dll")]
        public static extern bool Shell_NotifyIcon(NotifyCommand cmd, [In]ref NotifyIconData data);
    }
}
