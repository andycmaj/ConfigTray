using System;
using System.Windows.Forms;
using System.Threading;

namespace ConfigTray
{
    public partial class Splash : Form
    {
        private static Thread _splashLauncher;
        private static Splash _splashScreen;

        private delegate void DoSetStatus(string status, int percentComplete);

        public Splash()
        {
            InitializeComponent();
            progressBar1.Style = ProgressBarStyle.Blocks;
            progressBar1.Enabled = true;
        }

        public static Splash ShowSplash()
        {
            if (_splashLauncher == null)
            {
                //Show the form in a new thread
                _splashLauncher = new Thread(LaunchSplash);
                _splashLauncher.IsBackground = true;
                _splashLauncher.Start();
            }

            return _splashScreen;
        }

        private static void LaunchSplash()
        {
            if (_splashScreen == null)
            {
                _splashScreen = new Splash();

                //Create new message pump
                Application.Run(_splashScreen);
            }
        }

        public static void SetStatus(string currentStatus)
        {
            //Wait for splash to completely initialize.
            ShowSplash();
            while (_splashScreen == null)
            {
                Thread.Sleep(50);
            }

            _splashScreen.SetStatusInternal(currentStatus);
        }

        public static void CloseSplash()
        {
            //Need to get the thread that launched the form, so we need to use invoke.
            MethodInvoker mi = CloseSplashDown;
            _splashScreen.Invoke(mi);
        }

        private static void CloseSplashDown()
        {
            Application.ExitThread();
        }

        private void SetStatusInternal(string status)
        {
            try
            {
                Invoke(new DoSetStatus((s, p) => { Status.Text = s; progressBar1.Value = p; }), status, progressBar1.Value + 20);
            }
            catch (InvalidOperationException)
            {

            }
        }
    }
}
