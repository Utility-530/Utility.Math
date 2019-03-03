using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityMath.Conversion
{
    //	StackOverFlow  answered Dec 16 '12 at 17:58 Brad Christie
    /// <summary>
    /// For parsing fractionals as strings
    /// </summary>
    public static class FractionalNumber
    {

        public static bool TryParse(string input, out double result)
        {
            input = (input ?? String.Empty).Trim();
            if (String.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException("input");
            }

            // standard decimal number (e.g. 1.125)
            if (input.IndexOf('.') != -1 || (input.IndexOf(' ') == -1 && input.IndexOf('/') == -1 && input.IndexOf('\\') == -1))
            {
                return (Double.TryParse(input, out result));
            }

            String[] parts = input.Split(new[] { ' ', '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

            // stand-off fractional (e.g. 7/8)
            if (input.IndexOf(' ') == -1 && parts.Length == 2)
            {
                Double num, den;
                if (Double.TryParse(parts[0], out num) && Double.TryParse(parts[1], out den))
                {
                    result = num / den;
                    return true;
                }
            }

            // Number and fraction (e.g. 2 1/2)
            if (parts.Length == 3)
            {
                Double whole, num, den;
                if (Double.TryParse(parts[0], out whole) && Double.TryParse(parts[1], out num) && Double.TryParse(parts[2], out den))
                {
                    result = whole + (num / den);
                    return true;
                }
            }

            // Bogus / unable to parse
            result = Double.NaN;
            return false;
        }

        public static double Parse(string a)
        {
            var x = TryParse(a, out double val);
            return val;
        }
    }
}
