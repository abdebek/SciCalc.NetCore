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