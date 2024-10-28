using System.Text.RegularExpressions;

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
        /// Handles scientific notation and calculates significant figures and decimal places.
        /// </summary>
        /// <param name="number">String representation of the number.</param>
        /// <returns>Parsed SignificantNumber object.</returns>
        public static SignificantNumber Parse(string number)
        {
            number = number.Trim().ToLower();
            double parsedValue = double.Parse(number);
            int significantDigits, decimalPlaces;

            if (number.Contains('e')) // Scientific notation case
            {
                var parts = number.Split('e');
                var mantissa = parts[0];
                int exponent = int.Parse(parts[1]);
                significantDigits = CountSignificantFigures(mantissa);
                decimalPlaces = GetDecimalPlaces(mantissa) - exponent;
            }
            else // Regular number
            {
                significantDigits = CountSignificantFigures(number);
                decimalPlaces = GetDecimalPlaces(number);
            }

            return new SignificantNumber(parsedValue, significantDigits, decimalPlaces);
        }

        /// <summary>
        /// Counts the number of significant figures in a numeric string.
        /// </summary>
        /// <param name="number">String representation of the number.</param>
        /// <returns>Number of significant figures.</returns>
        private static int CountSignificantFigures(string number)
        {
            number = number.TrimStart('0').Replace(".", "");
            if (string.IsNullOrEmpty(number) || number == "0") return 0;

            int nonZeroCount = Regex.Match(number, @"^[1-9][0-9]*").Length;
            int trailingZeroCount = number.Contains('.') ? Regex.Match(number, @"(0*)$").Length : 0;

            return nonZeroCount + trailingZeroCount;
        }

        /// <summary>
        /// Gets the count of decimal places in a numeric string.
        /// </summary>
        /// <param name="number">String representation of the number.</param>
        /// <returns>Number of decimal places.</returns>
        private static int GetDecimalPlaces(string number)
        {
            var parts = number.Split('.');
            return parts.Length > 1 ? parts[1].Length : 0;
        }

        public override string ToString() => Value.ToString($"F{DecimalPlaces}");
    }
}
