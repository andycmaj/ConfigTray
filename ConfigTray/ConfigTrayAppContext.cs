using System;
using System.Windows.Forms;

namespace ConfigTray
{
    public class ConfigTrayAppContext : ApplicationContext
    {
        private Splash _splash;

        public ConfigTrayAppContext()
        {
            _splash = Splash.ShowSplash();
            Application.DoEvents();

            base.MainForm = new HiddenForm(OnMenusInitialized, OnInitializing) { Visible = false };
            base.MainForm.Hide();
        }

        private void OnMenusInitialized(object sender, InitializedEventArgs e)
        {
            Splash.CloseSplash();
        }

        private void OnInitializing(InitializationStepEventArgs e)
        {
            Splash.SetStatus(e.Status);
        }

        protected override void OnMainFormClosed(object sender, EventArgs e)
        {
            if (sender is Splash)
            {
            }
            else if (sender is HiddenForm)
            {
                base.OnMainFormClosed(sender, e);
            }
        }
    }

    public class InitializedEventArgs : EventArgs
    {
    }

    public class InitializationStepEventArgs : EventArgs
    {
        public string Status { get; set; }

        public static implicit operator InitializationStepEventArgs(string statusUpdate)
        {
            return new InitializationStepEventArgs { Status = statusUpdate };
        }
    }
}
