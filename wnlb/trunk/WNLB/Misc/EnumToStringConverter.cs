using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace WNLB.Misc
{
    public class EnumToStringConverter : EnumConverter
    {

        public EnumToStringConverter(Type type)
            : base(type)
        {
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
        {

            if (destType == typeof(string))
            {
                string stringValue = (string)base.ConvertTo(context, culture, value, destType);
                stringValue = SplitString(stringValue);
                return stringValue;
            }

            return base.ConvertTo(context, culture, value, destType);
        }

        public string SplitString(string stringValue)
        {

            StringBuilder buf = new StringBuilder(stringValue);
            bool lastWasUpper = true;
            int lastSpaceIndex = -1;

            for (int i = 1; i < buf.Length; i++)
            {
                bool isUpper = char.IsUpper(buf[i]);
                if (isUpper & !lastWasUpper)
                {
                    buf.Insert(i, ' ');
                    lastSpaceIndex = i;
                }

                if (!isUpper && lastWasUpper)
                {
                    if (lastSpaceIndex != i - 2)
                    {
                        buf.Insert(i - 1, ' ');
                        lastSpaceIndex = i - 1;
                    }
                }

                lastWasUpper = isUpper;
            }

            return buf.ToString();
        }
    }
}
