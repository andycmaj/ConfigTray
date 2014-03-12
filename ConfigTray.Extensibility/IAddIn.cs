using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ConfigTray.Extensibility
{
    public interface IAddIn
    {
        string Name { get; }

        void RegisterAddin(IConfiguration hostConfig);

        ToolStripMenuItem GetMenuItem();
    }
}
