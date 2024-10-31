namespace SciCalc;

public partial class Sci
{
    public class ScientificMathCaculator
    {
        /// <summary>
        /// Adds two SignificantNumbers and returns a new SignificantNumber with correct decimal precision.
        /// </summary>
        public static SignificantNumber Add(SignificantNumber a, SignificantNumber b)
        {
            double sum = a.Value + b.Value;
            int decimalPlaces = Math.Min(a.DecimalPlaces, b.DecimalPlaces);
            return CreateResult(sum, decimalPlaces);
        }

        /// <summary>
        /// Subtracts two SignificantNumbers and returns a new SignificantNumber with correct decimal precision.
        /// </summary>
        public static SignificantNumber Subtract(SignificantNumber a, SignificantNumber b)
        {
            double difference = a.Value - b.Value;
            int decimalPlaces = Math.Min(a.DecimalPlaces, b.DecimalPlaces);
            return CreateResult(difference, decimalPlaces);
        }

        /// <summary>
        /// Multiplies two SignificantNumbers and returns a new SignificantNumber with correct significant figures.
        /// </summary>
        public static SignificantNumber Multiply(SignificantNumber a, SignificantNumber b)
        {
            double product = a.Value * b.Value;
            int significantFigures = Math.Min(a.SignificantDigits, b.SignificantDigits);
            return CreateResultWithSignificantFigures(product, significantFigures);
        }

        /// <summary>
        /// Divides two SignificantNumbers and returns a new SignificantNumber with correct significant figures.
        /// </summary>
        public static SignificantNumber Divide(SignificantNumber a, SignificantNumber b)
        {
            if (Math.Abs(b.Value) < double.Epsilon)
                throw new DivideByZeroException("Division by zero encountered in SignificantNumber division.");

            double quotient = a.Value / b.Value;
            int significantFigures = Math.Min(a.SignificantDigits, b.SignificantDigits);
            return CreateResultWithSignificantFigures(quotient, significantFigures);
        }

        /// <summary>
        /// Rounds a number to the specified number of significant figures.
        /// </summary>
        private static double RoundToSignificantFigures(double number, int significantFigures)
        {
            if (number == 0) return 0;
            double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(number))) - significantFigures + 1);
            return Math.Round(number / scale) * scale;
        }

        /// <summary>
        /// Creates a SignificantNumber with a rounded value based on decimal precision.
        /// </summary>
        private static SignificantNumber CreateResult(double value, int decimalPlaces)
        {
            double roundedValue = Math.Round(value, decimalPlaces);
            return SignificantNumber.Parse(roundedValue.ToString($"F{decimalPlaces}"));
        }

        /// <summary>
        /// Creates a SignificantNumber with a rounded value based on significant figures.
        /// </summary>
        private static SignificantNumber CreateResultWithSignificantFigures(double value, int significantFigures)
        {
            double roundedValue = RoundToSignificantFigures(value, significantFigures);
            return SignificantNumber.Parse(roundedValue.ToString($"G{significantFigures}"));
        }
    }
}
