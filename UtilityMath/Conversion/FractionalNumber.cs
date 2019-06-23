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

        public static double Parse(string input)
        {    
            input = (input ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException(nameof(input));
            }

            // standard decimal number (e.g. 1.125)
            if (input.IndexOf('.') != -1 || (input.IndexOf(' ') == -1 && input.IndexOf('/') == -1 && input.IndexOf('\\') == -1))
            {
                return double.Parse(input);
            }

            string[] parts = input.Split(new[] { ' ', '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

            // stand-off fractional (e.g. 7/8)
            if (input.IndexOf(' ') == -1 && parts.Length == 2)
            {
                double num, den;
                if (double.TryParse(parts[0], out num) && double.TryParse(parts[1], out den))
                {
                    return num / den;
                }
                else
                {
                    throw new Exception($"Unable to parse either {parts[0]} or {parts[1]}.");
                }
            }

            // Number and fraction (e.g. 2 1/2)
            if (parts.Length == 3)
            {
                double whole, num, den;
                if (double.TryParse(parts[0], out whole) && double.TryParse(parts[1], out num) && double.TryParse(parts[2], out den))
                {
                    return whole + (num / den);
                }
                else
                {
                    throw new Exception($"Unable to parse either {parts[0]} or {parts[1]} or {parts[2]}.");
                }
            }

            throw new ArgumentException("Unable to parse.");

        }

        public static bool TryParse(string input, out double result)
        {
            bool success = true;
            result = 0;
            try
            {
                result = Parse(input);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                success = false;
            }

            return success;
        }



                //https://stackoverflow.com/questions/14320891/convert-percentage-to-nearest-fraction
        // answered Jan 14 '13 at 16:44    DasKrümelmonster
        public static (int, int) GetFraction(double value, double tolerance = 0.02)
        {
            double f0 = 1 / value;
            double f1 = 1 / (f0 - Math.Truncate(f0));

            int a_t = (int)Math.Truncate(f0);
            int a_r = (int)Math.Round(f0);
            int b_t = (int)Math.Truncate(f1);
            int b_r = (int)Math.Round(f1);
            int c = (int)Math.Round(1 / (f1 - Math.Truncate(f1)));

            if (Math.Abs(1.0 / a_r - value) <= tolerance)
                return (1, a_r);
            else if (Math.Abs(b_r / (a_t * b_r + 1.0) - value) <= tolerance)
                return (b_r, a_t * b_r + 1);
            else
                return (c * b_t + 1, c * a_t * b_t + a_t + c);
        }

    }
}
