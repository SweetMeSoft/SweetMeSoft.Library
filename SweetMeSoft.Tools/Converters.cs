using System.Globalization;

namespace SweetMeSoft.Tools
{
    public class Converters
    {
        public static double StringToDouble(string number)
        {
            if (number == null)
            {
                return 0;
            }

            number = number.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            return double.Parse(number);
        }

        public static float StringToFloat(string number)
        {
            if (number == null)
            {
                return 0;
            }

            number = number.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            return float.Parse(number);
        }

        public static decimal StringToDecimal(string number)
        {
            if (number == null)
            {
                return 0;
            }

            number = number.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            return decimal.Parse(number);
        }

        public static int StringToInt(string number)
        {
            if (number == null)
            {
                return 0;
            }

            number = number.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            if (number.Contains(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator))
            {
                number = number[..number.IndexOf(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)];
            }

            return int.Parse(number);
        }

        public static string IntToString(int number)
        {
            return number.ToString("0.#0", CultureInfo.InvariantCulture);
        }

        public static string DecimalToString(decimal number)
        {
            return number.ToString("0.#0", CultureInfo.InvariantCulture);
        }
    }
}
