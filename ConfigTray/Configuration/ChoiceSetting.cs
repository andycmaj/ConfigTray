using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.ComponentModel;

namespace ConfigTray.Configuration
{
    [XmlRoot("choice")]
    public class ChoiceSetting : Setting
    {
        [XmlArray("values")]
        [XmlArrayItem("value", typeof(string))]
        [Category(Constants.CATEGORY_CHOICE), Description("List of possible values for this setting."), Browsable(true), ReadOnly(false)]
        public Collection<string> PossibleValues { get; set; }
    }
}
