using System;
using System.Xml.Serialization;
using System.ComponentModel;

namespace ConfigTray.Configuration
{
    [XmlRoot("toggle")]
    public class ToggleSetting : Setting
    {
        #region Properties

        [XmlElement("trueValue")]
        [Category(Constants.CATEGORY_TOGGLE), Description("Enabled value. The string that represents the enabled state for the toggle setting."), Browsable(true), ReadOnly(false)]
        public string TrueValue
        {
            get;
            set;
        }

        [XmlElement("falseValue")]
        [Category(Constants.CATEGORY_TOGGLE), Description("Disabled value. The string that represents the disabled state for the toggle setting."), Browsable(true), ReadOnly(false)]
        public string FalseValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the setting's current value is the trueValue.
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public bool IsOn
        {
            get
            {
                if (Value != TrueValue && Value != FalseValue)
                {
                    throw new InvalidOperationException("Value in TrayConfig configuration does not match trueValue or falseValue");
                }
                return Value == TrueValue;
            }

            set
            {
                Value = value ? TrueValue : FalseValue;
            }
        }

        #endregion
    }
}
