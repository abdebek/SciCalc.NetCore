using static ScientificCalculator.SignificantFigures;

namespace ScientificCalculator;

public class SignificantFigures
{
    /// <summary>
    /// Represents a number with its significant figures
    /// </summary>
    public class SignificantNumber
    {
        public double Value { get; private set; }
        public int SignificantDigits { get; private set; }

        public SignificantNumber(double value, int significantDigits)
        {
            Value = value;
            SignificantDigits = significantDigits;
        }

        public static SignificantNumber Parse(string number)
        {
            // Remove any leading/trailing whitespace
            number = number.Trim();

            // Handle scientific notation
            if (number.Contains("e") || number.Contains("E"))
            {
                var parts = number.Split(new[] { 'e', 'E' });
                var mantissa = parts[0];
                var significantDigits = CountSignificantFigures(mantissa);
                return new SignificantNumber(double.Parse(number), significantDigits);
            }

            var significantFigures = CountSignificantFigures(number);
            return new SignificantNumber(double.Parse(number), significantFigures);
        }

        public static int CountSignificantFigures(string number)
        {
            // Remove decimal point and leading zeros
            number = number.Replace(".", "").TrimStart('0');

            // Count trailing zeros only if there was a decimal point
            if (number.Contains("."))
                return number.TrimEnd('0').Length;

            return number.TrimEnd('0').Length;
        }
    }

    public class Calculator
    {
        /// <summary>
        /// Adds two numbers following significant figure rules
        /// </summary>
        public static SignificantNumber Add(SignificantNumber a, SignificantNumber b)
        {
            var sum = a.Value + b.Value;
            // In addition/subtraction, result should have same decimal places as least precise number
            var decimalPlacesA = GetDecimalPlaces(a.Value);
            var decimalPlacesB = GetDecimalPlaces(b.Value);
            var resultDecimals = Math.Min(decimalPlacesA, decimalPlacesB);
            var rounded = Math.Round(sum, resultDecimals);
            return new SignificantNumber(rounded, SignificantNumber.CountSignificantFigures(rounded.ToString()));
        }

        /// <summary>
        /// Multiplies two numbers following significant figure rules
        /// </summary>
        public static SignificantNumber Multiply(SignificantNumber a, SignificantNumber b)
        {
            var product = a.Value * b.Value;
            // In multiplication/division, result should have same number of significant figures as least precise number
            var resultSigFigs = Math.Min(a.SignificantDigits, b.SignificantDigits);
            var rounded = RoundToSignificantFigures(product, resultSigFigs);
            return new SignificantNumber(rounded, resultSigFigs);
        }

        /// <summary>
        /// Divides two numbers following significant figure rules
        /// </summary>
        public static SignificantNumber Divide(SignificantNumber a, SignificantNumber b)
        {
            if (Math.Abs(b.Value) < double.Epsilon)
                throw new DivideByZeroException();

            var quotient = a.Value / b.Value;
            var resultSigFigs = Math.Min(a.SignificantDigits, b.SignificantDigits);
            var rounded = RoundToSignificantFigures(quotient, resultSigFigs);
            return new SignificantNumber(rounded, resultSigFigs);
        }

        /// <summary>
        /// Calculates power following significant figure rules
        /// </summary>
        public static SignificantNumber Power(SignificantNumber baseNum, double exponent)
        {
            var result = Math.Pow(baseNum.Value, exponent);
            return new SignificantNumber(result, baseNum.SignificantDigits);
        }

        private static int GetDecimalPlaces(double number)
        {
            var str = number.ToString("G17");
            var decimalIndex = str.IndexOf('.');
            return decimalIndex == -1 ? 0 : str.Length - decimalIndex - 1;
        }

        private static double RoundToSignificantFigures(double number, int significantFigures)
        {
            if (number == 0)
                return 0;

            var scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(number))) + 1);
            return scale * Math.Round(number / scale, significantFigures - 1);
        }
    }
}


public class Program
{
    public static void Main()
    {
        var num1 = SignificantNumber.Parse("12.1034");
        var num2 = SignificantNumber.Parse("5.67");

        var sum = Calculator.Add(num1, num2);
        var product = Calculator.Multiply(num1, num2);
        var quotient = Calculator.Divide(num1, num2);
        var power = Calculator.Power(num1, 2.0);

        Console.WriteLine($"Sum: {sum.Value}");
        Console.WriteLine($"Product: {product.Value}");
        Console.WriteLine($"Quotient: {quotient.Value}");
        Console.WriteLine($"Power: {power.Value}");
    }
}
