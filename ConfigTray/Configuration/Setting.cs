using System;
using System.Xml.Serialization;
using System.Xml.XPath;
using NLog;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.Serialization;

namespace ConfigTray.Configuration
{
    [Serializable]
    public abstract class Setting : ISerializable
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        #region Events

        /// <summary>
        /// Occurs when the Setting's value has been changed by an external source.
        /// </summary>
        public event Action<Setting> ExternalSettingChange;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Setting's name.
        /// </summary>
        [XmlAttribute("name")]
        [Category(Constants.CATEGORY_COMMON), Description("Setting name. This is the unique setting identifier that is used to reference a setting in a file."), Browsable(true), ReadOnly(false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Setting's display text.
        /// </summary>
        [XmlAttribute("desc")]
        [Category(Constants.CATEGORY_COMMON), Description("Setting description. This is how a setting will be displayed."), Browsable(true), ReadOnly(false)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the XPath location of the Setting in its config file.
        /// </summary>
        [XmlElement("xpath")]
        [Category(Constants.CATEGORY_COMMON), Description("Setting XPath. XPath expression to find the setting in a configuration file."), Browsable(true), ReadOnly(false)]
        [Editor(typeof(XPathEditor), typeof(UITypeEditor))]
        public string ValueXPath { get; set; }

        /// <summary>
        /// Gets or sets the namespace url to use when evaluating the xpath expression.
        /// </summary>
        [XmlAttribute("ns")]
        [Category(Constants.CATEGORY_COMMON), Description("Setting XML namespace. Used if the XML node containing the setting is part of an XML namespace."), Browsable(true), ReadOnly(false)]
        public string XmlNamespace { get; set; }

        /// <summary>
        /// Gets or sets the current value of a Setting in its config file.
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether the Setting has successfully loaded a value from a config file.
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public bool HasValue { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Copy an abstract Setting and apply set its current value from a config file.
        /// </summary>
        /// <param name="setting">The abstract setting with no concrete value.</param>
        /// <param name="file">The config file in which to look for the Setting's value.</param>
        /// <returns>A concrete instance of a Setting with a value from a specific config file.</returns>
        public static Setting Load(Setting setting, ConfigFile file)
        {
            //Copy Setting properties from generic setting.
            Setting newSetting = (Setting)setting.MemberwiseClone();

            try
            {
                //Set setting value from config file.
                newSetting.Value = file.GetValue(newSetting);

                if (newSetting.Value != null)
                {
                    newSetting.HasValue = true;
                }

                s_logger.Info("Setting found: {0} -> {1}", setting.Name, newSetting.Value);
            }
            catch (XPathException ex)
            {
                s_logger.ErrorException("Could not find setting value. Make sure xpath expression is correct.", ex);
                return newSetting;
            }
            catch (Exception ex)
            {
                s_logger.ErrorException("Could not find setting value.", ex);
                return newSetting;
            }

            //Subscribe to config file's change event.
            file.ExternalFileChange += newSetting.OnExternalFileChange;

            return newSetting;
        }

        private void OnExternalFileChange(ConfigFile changedFile)
        {
            //Get value for this setting from changed file.
            string newValue = changedFile.GetValue(this, true);

            //Check if value has changed.
            if (newValue == Value)
            {
                //Setting value has not changed.
                return;
            }

            s_logger.Info("Config file change observed: {0}", changedFile.Name);

            //Update setting value.
            Value = newValue;

            //Fire event to notify of value change.
            if (ExternalSettingChange != null)
            {
                ExternalSettingChange(this);
            }
        }

        public override string ToString()
        {
            return Description;
        }

        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //Never called?
        }

        #endregion
    }
}
