using System.Numerics;
using System.Globalization;

namespace SciCalc;
public partial class Sci
{
    /// <summary>
    /// Provides fixed-point arithmetic operations for high-precision calculations across multiple numeric types.
    /// Supports scientific notation (e.g., 1.23e-4).
    /// </summary>
    /// <typeparam name="T">The numeric type used for calculations.</typeparam>
    public static class FixedPointCalculator<T> where T : INumber<T>
    {
        /// <summary>
        /// Adds two numbers of type <typeparamref name="T"/> with fixed-point precision.
        /// </summary>
        public static T Add(T a, T b)
        {
            int scale = GetMaxScale(a, b);
            long scaledA = ScaleToLong(a, scale);
            long scaledB = ScaleToLong(b, scale);
            return UnScaleFromLong(scaledA + scaledB, scale);
        }

        /// <summary>
        /// Subtracts one number from another with fixed-point precision.
        /// </summary>
        public static T Subtract(T a, T b)
        {
            int scale = GetMaxScale(a, b);
            long scaledA = ScaleToLong(a, scale);
            long scaledB = ScaleToLong(b, scale);
            return UnScaleFromLong(scaledA - scaledB, scale);
        }

        /// <summary>
        /// Multiplies two numbers of type <typeparamref name="T"/> with fixed-point precision.
        /// </summary>
        public static T Multiply(T a, T b)
        {
            int scaleA = GetScale(a);
            int scaleB = GetScale(b);
            long scaledA = ScaleToLong(a, scaleA);
            long scaledB = ScaleToLong(b, scaleB);
            var result = UnScaleFromLong(scaledA * scaledB, scaleA + scaleB);
            var resultScale = GetScale(result);

            return resultScale <= 9 ? result : T.CreateChecked(Math.Round(Convert.ToDecimal(result), 9, MidpointRounding.AwayFromZero));
        }

        /// <summary>
        /// Divides one number by another using decimal precision.
        /// </summary>
        /// <exception cref="DivideByZeroException">Thrown when <paramref name="b"/> is zero.</exception>
        public static T Divide(T a, T b)
        {
            if (b == T.Zero)
            {
                throw new DivideByZeroException("Cannot divide by zero.");
            }

            var result = Convert.ToDecimal(a) / Convert.ToDecimal(b);
            var resultScale = GetDecimalScale(result);

            return resultScale <= 9 ? T.CreateChecked(result) : T.CreateChecked(Math.Round(result, 9, MidpointRounding.AwayFromZero));
        }

        /// <summary>
        /// Gets the number of decimal places for a decimal value.
        /// </summary>
        /// <param name="value">The decimal value.</param>
        /// <returns>The number of decimal places.</returns>
        private static int GetDecimalScale(decimal value)
        {
            // This assumes the decimal representation, 
            // extracting the scale from the bits directly.
            int[] bits = decimal.GetBits(value);
            return (bits[3] >> 16) & 0x7F;
        }

        /// <summary>
        /// Determines the scale factor (number of decimal places) required to preserve the precision of a given value.
        /// Handles scientific notation.
        /// </summary>
        private static int GetScale(T value)
        {
            string valueStr = value.ToString()!;

            // Handle scientific notation
            if (valueStr.Contains('e', StringComparison.OrdinalIgnoreCase))
            {
                string[] parts = valueStr.Split('e', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2) return 0;

                // Get the mantissa's decimal places
                int mantissaScale = parts[0].Contains('.') ? parts[0].Split('.')[1].Length : 0;

                // Parse the exponent
                if (int.TryParse(parts[1], out int exponent))
                {
                    // Adjust scale based on exponent
                    return Math.Max(0, mantissaScale - exponent);
                }
                return mantissaScale;
            }

            // Handle regular decimal numbers
            if (value is decimal decimalValue)
            {
                return GetDecimalScale(decimalValue);
            }
            else if (value is double || value is float)
            {
                return valueStr.Contains('.') ? valueStr.Split('.')[1].Length : 0;
            }

            return 0;
        }

        /// <summary>
        /// Computes the maximum scale factor (decimal places) needed to preserve precision between two values.
        /// </summary>
        private static int GetMaxScale(T a, T b)
        {
            return Math.Max(GetScale(a), GetScale(b));
        }

        /// <summary>
        /// Scales a value up to an integer representation using the given scale factor.
        /// Handles scientific notation.
        /// </summary>
        private static long ScaleToLong(T value, int scale)
        {
            string valueStr = value.ToString()!;

            if (valueStr.Contains('e', StringComparison.OrdinalIgnoreCase))
            {
                // Parse the number in scientific notation
                if (double.TryParse(valueStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double doubleValue))
                {
                    decimal decimalValue = (decimal)doubleValue;
                    return (long)(decimalValue * (decimal)Math.Pow(10, scale));
                }
            }

            decimal regularValue = Convert.ToDecimal(value);
            return (long)(regularValue * (decimal)Math.Pow(10, scale));
        }

        /// <summary>
        /// Unscales a long value to the target type by the given scale factor.
        /// </summary>
        private static T UnScaleFromLong(long value, int scale)
        {
            decimal scaledValue = (decimal)value / (decimal)Math.Pow(10, scale);
            return T.CreateChecked(scaledValue);
        }
    }
}