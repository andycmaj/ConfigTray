using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;
using ConfigTray.Extensibility;
using NLog;

namespace ConfigTray.Configuration
{
    [XmlRoot("addin")]
    public class AddIn
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        [XmlAttribute("class")]
        public string ClassName { get; set; }

        [XmlAttribute("path")]
        public string Path { get; set; }

        public ToolStripMenuItem CreateMenuItem()
        {
            if (!File.Exists(Path))
            {
                s_logger.Debug("AddIn not found at specified path: {0}.", Path);
                throw new FileNotFoundException(Path);
            }

            Assembly pluginAssembly = Assembly.LoadFile(Path);

            IAddIn addIn = pluginAssembly.CreateInstance(ClassName, true) as IAddIn;
            if (addIn == null)
            {
                s_logger.Debug("AddIn not Loaded: {0}.", addIn.Name);
                throw new InvalidCastException("AddIn cannot be loaded as it does not implement IAddIn interface.");
            }

            s_logger.Debug("AddIn Loaded: {0}.", addIn.Name);

            addIn.RegisterAddin(ConfigTrayConfiguration.Instance);

            return addIn.GetMenuItem();
        }
    }
}
