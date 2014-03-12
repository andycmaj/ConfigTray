using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ConfigTray.Configuration;

namespace ConfigTray.Controls
{
    public class VersionMenuItem : ToolStripMenuItem
    {
        public VersionSource Source { get; set; }

        public VersionMenuItem(VersionSource verisonSource)
        {
            Source = verisonSource;

            InitializeMenuItem();
        }

        private void InitializeMenuItem()
        {
            Font = new Font(Font, FontStyle.Bold);
            RightToLeft = RightToLeft.No;
            ToolTipText = "Click to copy to clipboard";

            Text = Source.Version;
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Regex captureRegex = new Regex(ConfigTrayConfiguration.Instance.VersionCaptureRegex);

            Match match = captureRegex.Match(Source.Version);

            string versionNum = string.Format("{0:d2}.{1:d2}.{2:d4}.{3:d4}",
                int.Parse(match.Groups["Version"].Value),
                int.Parse(match.Groups["Milestone"].Value),
                int.Parse(match.Groups["BuildNum"].Value),
                int.Parse(match.Groups["Date"].Value));

            Clipboard.SetText(versionNum);
        }
    }
}
