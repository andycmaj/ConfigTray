using System;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms;

namespace ConfigTray
{
    public class XPathEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            string newValue = string.Empty;
            using (XPathBuilder builder = new XPathBuilder())
            {
                DialogResult result = builder.ShowDialog();
                if (result == DialogResult.Cancel)
                {
                    newValue = (string)value;
                }

                newValue = builder.SelectedXPath;
            }

            return newValue ?? value;
        }
    }
}
