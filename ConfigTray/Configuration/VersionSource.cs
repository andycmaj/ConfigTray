
using System.Xml.Serialization;
namespace ConfigTray.Configuration
{
    public abstract class VersionSource
    {
        [XmlIgnore]
        public string Version { get; set; }
    }
}
