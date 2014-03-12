using System.Net;
using System.Xml.Serialization;

namespace ConfigTray.Configuration
{
    [XmlRoot("webpage")]
    public class WebPageVersionSource : VersionSource
    {
        private string m_url;

        [XmlAttribute("url")]
        public string Url
        {
            get {
                return m_url;
            }
            set {
                if (string.IsNullOrEmpty(Version))
                {
                    m_url = value;

                    using (WebClient client = new WebClient())
                    {
                        try
                        {
                            Version = client.DownloadString(m_url);
                        }
                        catch (WebException ex)
                        {
                            Version = ex.Message;
                        }
                    }
                }
            }
        }
    }
}
