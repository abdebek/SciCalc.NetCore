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
    ];

    public static void RunAllTests()
    {
        foreach (var (n1, n2) in testCases)
        {
            var num1 = Convert.ToDouble(n1);
            var num2 = Convert.ToDouble(n2);

            Console.WriteLine($"{n1} + {n2} = {FixedPointCalculator<double>.Add(num1, num2)}");
            Console.WriteLine($"{n1} - {n2} = {FixedPointCalculator<double>.Subtract(num1, num2)}");
            Console.WriteLine($"{n1} × {n2} = {FixedPointCalculator<double>.Multiply(num1, num2)}");
            Console.WriteLine($"{n1} ÷ {n2} = {FixedPointCalculator<double>.Divide(num1, num2)}");
            Console.WriteLine();
        }
    }
}