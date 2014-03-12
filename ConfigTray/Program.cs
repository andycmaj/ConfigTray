using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Linq;
using NLog;

namespace ConfigTray
{
    static class Program
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //TODO: Command line interface.
            //Something else here.

            Process[] runningInstances = Process.GetProcessesByName("ConfigTray");
            s_logger.Debug(runningInstances.Count());
            if (runningInstances.Count() > 0 && runningInstances.Any(instance => instance.Id != Process.GetCurrentProcess().Id))
            {
                MessageBox.Show("Cannot start more that one instance of ConfigTray.");
                Environment.Exit(5);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new ConfigTrayAppContext());
        }
    }
}
