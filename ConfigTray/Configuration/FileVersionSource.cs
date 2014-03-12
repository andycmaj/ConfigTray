using System.IO;
using System.Xml.Serialization;

namespace ConfigTray.Configuration
{
    [XmlRoot("file")]
    public class FileVersionSource : VersionSource
    {
        private string m_path;

        [XmlAttribute("path")]
        public string Path
        {
            get
            {
                return m_path;
            }
            set
            {
                if (string.IsNullOrEmpty(Version))
                {
                    if (!File.Exists(value))
                    {
                        throw new FileNotFoundException("Version file does not exist at specified path.");
                    }

                    m_path = value;

                    Version = File.ReadAllText(m_path);
                }
            }
        }
    }
}
