using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace ConfigTray
{
    public partial class RemoteExecForm : Form
    {
        private string _machine;

        public RemoteExecForm(string machineName)
        {
            _machine = machineName;

            Text = "Execute remote command on " + _machine;

            InitializeComponent();
        }

        private void ExecButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CommandTextBox.Text))
            {
                MessageBox.Show("Enter a command first.");
                return;
            }

            string psExecPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PSExec\\psexec.exe");
            string arguments = BuildArguments();

            Process psExec = new Process {
                StartInfo = new ProcessStartInfo(psExecPath, arguments) {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            psExec.Start();
            psExec.WaitForExit();

            string output;
            if (psExec.ExitCode == 0)
            {
                using (StreamReader outStream = psExec.StandardOutput)
                {
                    output = outStream.ReadToEnd().Trim();
                }
            }
            else
            {
                OutputTextBox.ForeColor = Color.Red;
                using (StreamReader errorStream = psExec.StandardError)
                {
                    output = errorStream.ReadToEnd().Trim();
                }
            }

            OutputTextBox.Text = string.IsNullOrEmpty(output) ? "No output." : output;
        }

        private string BuildArguments()
        {
            return string.Format("-i \\\\{0} {1}", _machine, CommandTextBox.Text);
        }
    }
}
