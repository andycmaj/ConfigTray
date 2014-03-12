using System;
using System.Windows.Forms;
using ConfigTray.Configuration;

namespace ConfigTray.Controls
{
    public class UserDefinedToolMenuItem : ToolStripMenuItem
    {
        public UserDefinedTool Tool { get; set; }

        public UserDefinedToolMenuItem(UserDefinedTool tool)
        {
            Tool = tool;

            Text = tool.Name;
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Tool.Start();
        }
    }
}
