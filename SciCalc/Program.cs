using System.Text.RegularExpressions;
using static ScientificCalculator.SignificantFigures;

namespace ScientificCalculator;

public class SignificantFigures
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
        /// Parses the specified number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        public static SignificantNumber Parse(string number)
        {
            number = number.Trim().ToLower();

            // Handle scientific notation
            if (number.Contains('e'))
            {
                var parts = number.Split('e');
                var mantissa = parts[0];
                var exponent = int.Parse(parts[1]);
                var significantDigits = CountSignificantFigures(mantissa);
                var value = double.Parse(number);
                var decimalPlaces = GetDecimalPlaces(mantissa) - exponent;
                return new SignificantNumber(value, significantDigits, decimalPlaces);
            }

            var sigFigs = CountSignificantFigures(number);
            var decPlaces = GetDecimalPlaces(number);
            return new SignificantNumber(double.Parse(number), sigFigs, decPlaces);
        }

        /// <summary>
        /// Counts the significant figures.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        public static int CountSignificantFigures(string number)
        {
            number = number.Trim().ToLower();
            if (number.Contains('e'))
                number = number.Split('e')[0];

            // Remove decimal point
            number = number.Replace(".", "");

            // Remove leading zeros
            number = number.TrimStart('0');

            // Match all significant digits
            var match = Regex.Match(number, @"^([1-9][0-9]*?)(0*)$");
            if (match.Success)
            {
                var nonZero = match.Groups[1].Value;
                var trailingZeros = match.Groups[2].Value;

                // If original number had decimal point, include trailing zeros
                if (number.Contains('.'))
                    return nonZero.Length + trailingZeros.Length;

                return nonZero.Length;
            }

            return number.Length;
        }

        private static int GetDecimalPlaces(string number)
        {
            var parts = number.Split('.');
            return parts.Length > 1 ? parts[1].Length : 0;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Value.ToString($"F{DecimalPlaces}");
        }
    }

    public class Calculator
    {
        /// <summary>
        /// Adds the specified a.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static SignificantNumber Add(SignificantNumber a, SignificantNumber b)
        {
            var sum = a.Value + b.Value;
            // For addition/subtraction, result should have same number of decimal places
            // as the least precise number
            var resultDecimals = Math.Min(a.DecimalPlaces, b.DecimalPlaces);
            var rounded = Math.Round(sum, resultDecimals);

            // Format the number to get correct string representation
            var resultStr = rounded.ToString($"F{resultDecimals}");
            return SignificantNumber.Parse(resultStr);
        }

        /// <summary>
        /// Subtracts the specified a.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static SignificantNumber Subtract(SignificantNumber a, SignificantNumber b)
        {
            var diff = a.Value - b.Value;
            var resultDecimals = Math.Min(a.DecimalPlaces, b.DecimalPlaces);
            var rounded = Math.Round(diff, resultDecimals);
            var resultStr = rounded.ToString($"F{resultDecimals}");
            return SignificantNumber.Parse(resultStr);
        }

        /// <summary>
        /// Multiplies the specified a.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static SignificantNumber Multiply(SignificantNumber a, SignificantNumber b)
        {
            var product = a.Value * b.Value;
            // For multiplication/division, result should have same number of significant figures
            // as the least precise number
            var resultSigFigs = Math.Min(a.SignificantDigits, b.SignificantDigits);
            var rounded = RoundToSignificantFigures(product, resultSigFigs);
            var resultStr = rounded.ToString($"G{resultSigFigs}");
            return SignificantNumber.Parse(resultStr);
        }

        /// <summary>
        /// Divides the specified a.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        /// <exception cref="System.DivideByZeroException"></exception>
        public static SignificantNumber Divide(SignificantNumber a, SignificantNumber b)
        {
            try
            {
                if (Math.Abs(b.Value) < double.Epsilon)
                    throw new DivideByZeroException();

                var quotient = a.Value / b.Value;
                var resultSigFigs = Math.Min(a.SignificantDigits, b.SignificantDigits);
                var rounded = RoundToSignificantFigures(quotient, resultSigFigs);
                var resultStr = rounded.ToString($"G{resultSigFigs}");
                return SignificantNumber.Parse(resultStr);
            }
            catch (DivideByZeroException)
            {
                return new SignificantNumber(double.NaN, 0, 0);
            }
        }

        private static double RoundToSignificantFigures(double number, int significantFigures)
        {
            if (number == 0)
                return 0;

            var order = Math.Floor(Math.Log10(Math.Abs(number)));
            var scale = Math.Pow(10, order - significantFigures + 1);
            return Math.Round(number / scale) * scale;
        }
    }
}


public class Program
{
    public static void Main()
    {
        var tests = new[]
        {
            ("1.23", "4.56"),        // Same decimal places
            ("12.1034", "5.67"),     // Different decimal places
            ("1.234e3", "5.67"),     // Scientific notation
            ("1200", "1.23"),        // Trailing zeros
            ("0.00120", "1.23"),     // Leading zeros
            ("0.00120", "0.000123"), // Very small numbers
        };

        foreach (var (n1, n2) in tests)
        {
            var num1 = SignificantNumber.Parse(n1);
            var num2 = SignificantNumber.Parse(n2);

            Console.WriteLine($"{n1} + {n2} = {Calculator.Add(num1, num2).Value}");
            Console.WriteLine($"{n1} - {n2} = {Calculator.Subtract(num1, num2).Value}");
            Console.WriteLine($"{n1} × {n2} = {Calculator.Multiply(num1, num2).Value}");
            Console.WriteLine($"{n1} ÷ {n2} = {Calculator.Divide(num1, num2).Value}");
            Console.WriteLine();
        }
    }
}
