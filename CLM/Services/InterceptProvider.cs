using CLM.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CLM.Services
{
    public class InterceptProvider : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(System.Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return this;
            else
                return null;
        }

        public string Format(String format, Object obj, IFormatProvider provider)
        {
            // Display information about method call.
            if (!this.Equals(provider))
                return null;

            // Set default format specifier             
            if (string.IsNullOrEmpty(format))
                format = "N";

            string numericString = obj.ToString();

            if (obj is int && format.ToUpper().Equals("U"))
            {
                return String.Format("{0,9:N0}-{1}", obj, StringManipulations.GetDigit((int)obj));
            }

            if (obj is string && format.ToUpper().Equals("I"))
            {
                return Regex.Replace(obj.ToString(),"-.*","").Replace(".","");
            }

            // If this is a byte and the "R" format string, format it with Roman numerals.
            if (obj is int && format.ToUpper().Equals("R"))
            {
                return StringManipulations.ToRomanNumeral((int)obj);
            }

            // Use default for all other formatting.
            if (obj is IFormattable)
                return ((IFormattable)obj).ToString(format, CultureInfo.CreateSpecificCulture("es-CL"));
            else
                return obj.ToString();
        }
    }
}
