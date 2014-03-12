using System;
using System.Windows.Forms;
using ConfigTray.Extensibility;

namespace TestAddIn
{
    //[AddIn("TestAddIn", Description = "Test AddIn for ConfigTray", Publisher = "Andrew Cunningham", Version = "0.1")]
    public class AddIn : IAddIn
    {
        #region IAddIn Members

        public void RegisterAddin(IConfiguration hostConfig)
        {
            
        }

        public ToolStripMenuItem GetMenuItem()
        {
            ToolStripMenuItem item = new ToolStripMenuItem("Test Menu Item");
            item.Click += item_Click;
            return item;
        }

        void item_Click(object sender, EventArgs e)
        {
            MessageBox.Show("SUP? from another dimension.");
        }

        public string Name
        {
            get
            {
                return "Test Addin.";
            }
        }

        #endregion
    }
}
