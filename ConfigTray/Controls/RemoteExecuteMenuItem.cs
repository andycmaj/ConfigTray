using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ConfigTray.Controls
{
    public class RemoteExecuteMenuItem : ToolStripMenuItem
    {
        private string _machineName;

        public RemoteExecuteMenuItem(string machineName)
        {
            _machineName = machineName;

            Text = "Execute remote command";

            Initialize();
        }

        private void Initialize()
        {
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            RemoteExecForm form = new RemoteExecForm(_machineName);
            form.Show();
        }
    }
}
