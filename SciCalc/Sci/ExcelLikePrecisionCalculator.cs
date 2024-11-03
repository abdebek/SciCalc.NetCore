namespace SciCalc;

public partial class Sci
{
    public static class ExcelLikePrecisionCalculator
    {
        private const int EXCEL_PRECISION = ExcelConstants.EXCEL_COMPUTATION_PRECISION;

        /// <summary>
        /// Calculates the number of decimal places needed after rounding to significant digits
        /// </summary>
        public static int CalculateAdjustedDecimalPlaces(double value, int significantDigits)
        {
            if (value == 0) return 0;

            int exponent = (int)Math.Floor(Math.Log10(Math.Abs(value)));
            return Math.Max(0, significantDigits - 1 - exponent);
        }

        /// <summary>
        /// Performs addition with Excel-like precision
        /// </summary>
        public static SignificantNumber Add(SignificantNumber a, SignificantNumber b)
        {
            double result = a.Value + b.Value;
            int significantDigits = Math.Min(Math.Max(a.SignificantDigits, b.SignificantDigits), EXCEL_PRECISION);
            result = Helpers.RoundToSpecificDigits(result, significantDigits);
            int decimalPlaces = CalculateAdjustedDecimalPlaces(result, significantDigits);
            return new SignificantNumber(result, significantDigits, decimalPlaces);
        }

        /// <summary>
        /// Performs subtraction with Excel-like precision
        /// </summary>
        public static SignificantNumber Subtract(SignificantNumber a, SignificantNumber b)
        {
            double result = a.Value - b.Value;
            int significantDigits = Math.Min(Math.Max(a.SignificantDigits, b.SignificantDigits), EXCEL_PRECISION);
            result = Helpers.RoundToSpecificDigits(result, significantDigits);
            int decimalPlaces = CalculateAdjustedDecimalPlaces(result, significantDigits);
            return new SignificantNumber(result, significantDigits, decimalPlaces);
        }

        /// <summary>
        /// Performs multiplication with Excel-like precision
        /// </summary>
        public static SignificantNumber Multiply(SignificantNumber a, SignificantNumber b)
        {
            double result = a.Value * b.Value;
            int significantDigits = Math.Min(Math.Min(a.SignificantDigits, b.SignificantDigits), EXCEL_PRECISION);
            result = Helpers.RoundToSpecificDigits(result, significantDigits);
            int decimalPlaces = CalculateAdjustedDecimalPlaces(result, significantDigits);
            return new SignificantNumber(result, significantDigits, decimalPlaces);
        }

        /// <summary>
        /// Performs division with Excel-like precision
        /// </summary>
        public static SignificantNumber Divide(SignificantNumber a, SignificantNumber b)
        {
            if (b.Value == 0)
                throw new DivideByZeroException("Cannot divide by zero.");

            double result = a.Value / b.Value;
            int significantDigits = Math.Min(Math.Min(a.SignificantDigits, b.SignificantDigits), EXCEL_PRECISION);
            result = Helpers.RoundToSpecificDigits(result, significantDigits);
            int decimalPlaces = CalculateAdjustedDecimalPlaces(result, significantDigits);
            return new SignificantNumber(result, significantDigits, decimalPlaces);
        }
    }
}