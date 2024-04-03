using System.Globalization;

namespace SweetMeSoft.Tools;

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
            number = number.Substring(0, number.IndexOf(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
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

    public static bool StringToBool(string stringCellValue)
    {
        if (string.IsNullOrEmpty(stringCellValue))
        {
            throw new ArgumentNullException(nameof(stringCellValue));
        }

        if (stringCellValue.ToLower() is "true" or "1" or "t")
        {
            return true;
        }

        if (stringCellValue.ToLower() is "false" or "0" or "f")
        {
            return false;
        }

        throw new ArgumentException("String value is not a boolean value");
    }
}