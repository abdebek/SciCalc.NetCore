using static SciCalc.Sci;

namespace SciCalcDemo.SciCalcTests;

public static class CalculatorTests
{
    private static readonly (string, string)[] testCases =
    [
        ("1.23", "4.56"),        // Same decimal places
        ("12.1034", "5.67"),     // Different decimal places
        ("1.234e3", "5.67"),     // Scientific notation
        ("1200", "1.23"),        // Trailing zeros
        ("0.00120", "1.23"),     // Leading zeros
        ("0.00120", "0.000123"), // Very small numbers
        ("405.123456789123", "102.123456789123") // Very large precision: 507.2469136, 303, 41372.60783, 3.966997099
    ];

    public static void RunAllFixedPointCalculationTests()
    {
        foreach (var (n1, n2) in testCases)
        {
            var num1 = Convert.ToDouble(n1);
            var num2 = Convert.ToDouble(n2);

            Console.WriteLine($"{n1} + {n2} = {RoundTo10Digits(FixedPointCalculator<double>.Add(num1, num2))}");
            Console.WriteLine($"{n1} - {n2} = {RoundTo10Digits(FixedPointCalculator<double>.Subtract(num1, num2))}");
            Console.WriteLine($"{n1} × {n2} = {RoundTo10Digits(FixedPointCalculator<double>.Multiply(num1, num2))}");
            Console.WriteLine($"{n1} ÷ {n2} = {RoundTo10Digits(FixedPointCalculator<double>.Divide(num1, num2))}");
            Console.WriteLine();
        }
    }

    public static void RunAllExcelLikePrecisionCalculationTests()
    {
        foreach (var (n1, n2) in testCases)
        {
            var num1 = SignificantNumber.Parse(n1);
            var num2 = SignificantNumber.Parse(n2);
            if (num1?.Value != null && num2 != null)
            {
                Console.WriteLine($"{n1} + {n2} = {RoundTo10Digits(ExcelLikePrecisionCalculator.Add(num1, num2).Value)}");
                Console.WriteLine($"{n1} - {n2} = {RoundTo10Digits(ExcelLikePrecisionCalculator.Subtract(num1, num2).Value)}");
                Console.WriteLine($"{n1} × {n2} = {RoundTo10Digits(ExcelLikePrecisionCalculator.Multiply(num1, num2).Value)}");
                Console.WriteLine($"{n1} / {n2} = {RoundTo10Digits(ExcelLikePrecisionCalculator.Divide(num1, num2).Value)}");
            }
            Console.WriteLine();
        }
    }

    private static T RoundTo10Digits<T>(T value) where T : IConvertible
    {
        return Helpers.RoundTo10Digits(value);
    }
}