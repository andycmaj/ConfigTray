using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace ConfigTray.Configuration
{
    [XmlRoot("tool")]
    public class UserDefinedTool
    {
        /// <summary>
        /// Gets or sets the name of the tool displayed on the menu item.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the path to the executable tool.
        /// </summary>
        [XmlAttribute("path")]
        public string ExecutablePath { get; set; }

        public void Start()
        {
            //TODO: Add command line params.
            if (!File.Exists(ExecutablePath))
            {
                throw new FileNotFoundException("Path to tool is incorrect");
            }

            Process process = new Process() {
                StartInfo = new ProcessStartInfo(ExecutablePath)
            };

            process.Start();
        }
    }
}
