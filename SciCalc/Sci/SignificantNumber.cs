using System.Text.RegularExpressions;
using System.Globalization;

namespace SciCalc;
public partial class Sci
{
    public class SignificantNumber
    {
        public double Value { get; private set; }
        public int SignificantDigits { get; private set; }
        public int DecimalPlaces { get; private set; }

        public SignificantNumber(double value, int significantDigits, int decimalPlaces)
        {
            Value = value;
            SignificantDigits = significantDigits;
            DecimalPlaces = decimalPlaces;
        }

        /// <summary>
        /// Parses a string representation of a number into a SignificantNumber instance.
        /// </summary>
        public static SignificantNumber Parse(string number)
        {
            number = number.Trim().ToLower();

            // Handle special case for zero
            if (number == "0" || number == "0.0" || number == ".0")
                return new SignificantNumber(0, 1, 0);

            double parsedValue;
            int significantDigits, decimalPlaces;

            if (number.Contains('e')) // Scientific notation case
            {
                var parts = number.Split('e');
                var mantissa = parts[0];
                int exponent = int.Parse(parts[1], CultureInfo.InvariantCulture);

                parsedValue = double.Parse(number, CultureInfo.InvariantCulture);
                significantDigits = CountSignificantFigures(mantissa);
                decimalPlaces = CalculateDecimalPlaces(mantissa, exponent);
            }
            else // Regular number
            {
                parsedValue = double.Parse(number, CultureInfo.InvariantCulture);
                significantDigits = CountSignificantFigures(number);
                decimalPlaces = GetDecimalPlaces(number);
            }

            return new SignificantNumber(parsedValue, significantDigits, decimalPlaces);
        }

        public static int CountSignificantFigures(string number)
        {
            // Remove decimal point and leading zeros
            string cleanNumber = number.Replace(".", "").TrimStart('0');
            if (string.IsNullOrEmpty(cleanNumber)) return 1; // Special case for zero

            // Count all digits for numbers with decimal points
            if (number.Contains('.'))
            {
                return Regex.Replace(cleanNumber, @"[^0-9]", "").Length;
            }

            // For integers, count up to the last non-zero digit
            return Regex.Match(cleanNumber, @"^[0-9]*[1-9]").Length;
        }

        private static int CalculateDecimalPlaces(string mantissa, int exponent)
        {
            int mantissaDecimals = GetDecimalPlaces(mantissa);
            return Math.Max(0, mantissaDecimals - exponent);
        }

        private static int GetDecimalPlaces(string number)
        {
            var parts = number.Split('.');
            return parts.Length > 1 ? parts[1].Length : 0;
        }

        public override string ToString()
        {
            if (Value == 0) return "0";

            // Use scientific notation for very large or small numbers
            double absValue = Math.Abs(Value);
            if (absValue < 0.0001 || absValue >= 10000000)
            {
                return Value.ToString($"E{DecimalPlaces}", CultureInfo.InvariantCulture);
            }

            return Value.ToString($"F{DecimalPlaces}", CultureInfo.InvariantCulture);
        }
    }
}