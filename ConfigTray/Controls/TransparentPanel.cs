using System;
using System.Windows.Forms;
using System.Drawing;

namespace ConfigTray.Controls
{
    public partial class TransparentPanel : Panel
    {
        public TransparentPanel()
        {
            InitializeComponent();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            using (Brush b = new SolidBrush(Color.FromArgb(150, 170, 170, 250)))
            {
                g.FillRectangle(b, ClientRectangle);
            }

        }
    }
}
