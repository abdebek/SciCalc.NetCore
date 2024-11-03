namespace SciCalc;

public partial class Sci
{
    public class Helpers
    {
        /// <summary>
        /// Rounds a number to a specific number of significant digits.
        /// </summary>
        public static T RoundToSpecificDigits<T>(T value, int numberOfDigits) where T : IConvertible
        {
            // Convert the input to double for rounding purposes
            double doubleValue = Convert.ToDouble(value);

            // Handle edge case for zero
            if (doubleValue == 0)
            {
                return (T)Convert.ChangeType(0, typeof(T));
            }

            // Determine the scale (or "order of magnitude") of the number
            int scale = (int)Math.Floor(Math.Log10(Math.Abs(doubleValue)));

            // Calculate the factor needed to shift the decimal point to get the required significant figures
            double factor = Math.Pow(10, numberOfDigits - scale - 1);

            // Perform the rounding by shifting, rounding, and then shifting back
            double roundedValue = Math.Round(doubleValue * factor, MidpointRounding.AwayFromZero) / factor;

            // Convert back to the original type and return
            return (T)Convert.ChangeType(roundedValue, typeof(T));
        }


        /// <summary>
        /// Rounds a number to exactly 10 digits i.e., ExcelConstants.EXCEL_DISPLAY_PRECISION
        /// </summary>
        public static T RoundTo10Digits<T>(T value) where T : IConvertible
        {
            return RoundToSpecificDigits(value, ExcelConstants.EXCEL_DISPLAY_PRECISION);
        }

        /// <summary>
        /// Rounds a number to exactly 15 digits i.e., ExcelConstants.EXCEL_COMPUTATION_PRECISION
        /// </summary>
        public static T RoundTo15Digits<T>(T value) where T : IConvertible
        {
            return RoundToSpecificDigits(value, ExcelConstants.EXCEL_COMPUTATION_PRECISION);
        }

        /// <summary>
        /// Summations the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static decimal Summation(List<decimal> values)
        {
            double total = values.Aggregate(0.0, (acc, val) => acc + (double)val);
            total = Helpers.RoundTo15Digits(total);
            return Helpers.RoundTo15Digits((decimal)total);
        }

        /// <summary>
        /// Products the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static decimal Product(List<decimal> values)
        {
            // If the list is empty, return 1 (neutral element for multiplication).
            if (values == null || values.Count == 0)
            {
                return 0m;
            }

            // Start with the first value
            decimal total = Helpers.RoundTo15Digits(values[0]);

            // Multiply each subsequent value
            for (int i = 1; i < values.Count; i++)
            {
                total = Helpers.RoundTo15Digits(decimal.Multiply(total, Helpers.RoundTo15Digits(values[i])));
            }

            // Apply rounding to 10 significant digits after all multiplications
            return Helpers.RoundTo15Digits(total);
        }

        /// <summary>
        /// Subtractions the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">SUBTRACT requires at least two arguments</exception>
        public static decimal Subtraction(List<decimal> values)
        {
            if (values.Count < 2)
                throw new ArgumentException("SUBTRACT requires at least two arguments");
            double total = values.Skip(1).Aggregate((double)values.First(), (acc, val) => acc - (double)val); total = Helpers.RoundTo15Digits(total);

            return Helpers.RoundTo15Digits((decimal)total);
        }

        /// <summary>
        /// Divisions the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">DIVIDE requires at least two arguments</exception>
        /// <exception cref="System.DivideByZeroException">Cannot divide by zero</exception>
        public static decimal Division(List<decimal> values)
        {
            if (values.Count < 2)
                throw new ArgumentException("DIVIDE requires at least two arguments");
            if (values.Skip(1).Any(v => v == 0))
                throw new DivideByZeroException("Cannot divide by zero");
            double total = values.Skip(1).Aggregate((double)values.First(), (acc, val) => acc / (double)val);

            return Helpers.RoundTo15Digits((decimal)total);
        }

        /// <summary>
        /// Pows the specified base value.
        /// </summary>
        /// <param name="baseValue">The base value.</param>
        /// <param name="exponent">The exponent.</param>
        /// <returns></returns>
        public static decimal Pow(decimal baseValue, int exponent)
        {
            if (exponent == 0)
                return 1m;

            decimal result = 1m;
            decimal factor = baseValue;

            int absExponent = Math.Abs(exponent);

            // Exponentiation by squaring for efficiency
            while (absExponent > 0)
            {
                if ((absExponent % 2) == 1)
                {
                    result *= factor;
                }
                factor *= factor;
                absExponent /= 2;
            }

            // If the exponent is negative, take the reciprocal
            return exponent < 0 ? 1m / result : result;
        }


        /// <summary>
        /// Means the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static decimal Mean(List<decimal> values)
        {
            if (values.Count == 0)
                return 0;
            decimal total = Helpers.Summation(values);
            return Helpers.RoundTo15Digits(total / values.Count);
        }

        /// <summary>
        /// Medians the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static decimal Median(List<decimal> values)
        {
            if (!values.Any())
                return 0m;
            var sorted = values.OrderBy(v => v).ToList();
            int n = sorted.Count;
            if (n % 2 == 0)
            {
                var mid1 = sorted[n / 2 - 1];
                var mid2 = sorted[n / 2];
                return Helpers.RoundTo15Digits((mid1 + mid2) / 2);
            }
            return sorted[n / 2];
        }

        /// <summary>
        /// Variances the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">VARIANCE requires at least two arguments</exception>
        public static decimal Variance(List<decimal> values)
        {
            if (values.Count < 2)
                throw new ArgumentException("VARIANCE requires at least two arguments");
            decimal mean = Helpers.Mean(values);
            decimal sumOfSquares = values.Aggregate(0m, (acc, val) => acc + (val - mean) * (val - mean));
            return Helpers.RoundTo15Digits(sumOfSquares / (values.Count - 1));
        }

        /// <summary>
        /// Standards the deviation.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static decimal StandardDeviation(List<decimal> values)
        {
            return Helpers.RoundTo15Digits((decimal)Math.Sqrt((double)Helpers.Variance(values)));
        }

        /// <summary>
        /// Averages the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static decimal Average(List<decimal> values)
        {
            return Helpers.Mean(values);
        }

        /// <summary>
        /// Mods the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">MOD requires exactly two arguments</exception>
        /// <exception cref="System.DivideByZeroException">Cannot calculate MOD with zero divisor</exception>
        public static decimal Mod(List<decimal> values)
        {
            if (values.Count != 2)
                throw new ArgumentException("MOD requires exactly two arguments");
            if (values[1] == 0)
                throw new DivideByZeroException("Cannot calculate MOD with zero divisor");

            return Helpers.RoundTo15Digits(values[0] % values[1]);
        }

        /// <summary>
        /// Rounds the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">ROUND requires exactly two arguments</exception>
        public static decimal Round(List<decimal> values)
        {
            if (values.Count != 2)
                throw new ArgumentException("ROUND requires exactly two arguments");
            return decimal.Round(values[0], (int)values[1]);
        }

        /// <summary>
        /// Absolutes the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">ABS requires exactly one argument</exception>
        public static decimal Absolute(List<decimal> values)
        {
            if (values.Count != 1)
                throw new ArgumentException("ABS requires exactly one argument");
            return decimal.Abs(values[0]);
        }

        /// <summary>
        /// Powers the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">POWER requires exactly two arguments</exception>
        public static decimal Power(List<decimal> values)
        {
            if (values.Count != 2)
                throw new ArgumentException("POWER requires exactly two arguments");
            return (decimal)Math.Pow((double)values[0], (double)values[1]);
        }

        /// <summary>
        /// Squares the root.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// SQRT requires exactly one argument
        /// or
        /// Cannot calculate square root of a negative number
        /// </exception>
        public static decimal SquareRoot(List<decimal> values)
        {
            if (values.Count != 1)
                throw new ArgumentException("SQRT requires exactly one argument");
            if (values[0] < 0)
                throw new ArgumentException("Cannot calculate square root of a negative number");
            return (decimal)Math.Sqrt((double)values[0]);
        }

        /// <summary>
        /// Logarithms the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// LOG requires one or two arguments
        /// or
        /// Cannot calculate logarithm of zero or a negative number
        /// </exception>
        public static decimal Logarithm(List<decimal> values)
        {
            if (values.Count == 0 || values.Count > 2)
                throw new ArgumentException("LOG requires one or two arguments");
            if (values[0] <= 0)
                throw new ArgumentException("Cannot calculate logarithm of zero or a negative number");

            return values.Count == 2
                ? (decimal)Math.Log((double)values[0], (double)values[1])
                : (decimal)Math.Log((double)values[0]);
        }

        /// <summary>
        /// Log10s the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// LOG10 requires exactly one argument
        /// or
        /// Cannot calculate logarithm of zero or a negative number
        /// </exception>
        public static decimal Log10(List<decimal> values)
        {
            if (values.Count != 1)
                throw new ArgumentException("LOG10 requires exactly one argument");
            if (values[0] <= 0)
                throw new ArgumentException("Cannot calculate logarithm of zero or a negative number");
            return (decimal)Math.Log10((double)values[0]);
        }

        /// <summary>
        /// Ceilings the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">CEILING requires exactly one argument</exception>
        public static decimal Ceiling(List<decimal> values)
        {
            if (values.Count != 1)
                throw new ArgumentException("CEILING requires exactly one argument");
            return decimal.Ceiling(values[0]);
        }

        /// <summary>
        /// Floors the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">FLOOR requires exactly one argument</exception>
        public static decimal Floor(List<decimal> values)
        {
            if (values.Count != 1)
                throw new ArgumentException("FLOOR requires exactly one argument");
            return decimal.Floor(values[0]);
        }
    }
}