using static SciCalc.Sci;

namespace SciCalcDemo.SciCalcTests;

public static class XLFormulaParserTests
{
    private static XLFormulaParser parser = new XLFormulaParser();

    // Dictionary to hold test cases
    private static readonly Dictionary<string, (Func<string, object> getCellValue, double expectedResult)> testCases = new()
        {
            // Test 1: Sum 1 + 2 + 3 + ... + 100
            {
                "SUM(A1:A100)",
                (
                    cell => Convert.ToDouble(cell.Substring(1)),
                    5050 // Expected result for sum of first 100 natural numbers
                )
            },

            //Test 2: Subtract 98 - 99 - 100
            {
                "SUBTRACT(A98:A100)",
                (
                    cell => Convert.ToDouble(cell.Substring(1)),
                    -101 // Expected result for 98 - 99 - 100
                )
            },

            // Test 3: Product 1 * 2 * 3
            {
                "PRODUCT(A1:A3)",
                (
                    cell => Convert.ToDouble(cell.Substring(1)),
                    6 // Expected result for 1 * 2 * 3
                )
            }

            // Add more test cases as needed
        };

    public static void RunAllTests()
    {
        foreach (var (formula, (getCellValue, expected)) in testCases)
        {
            var result = parser.Evaluate(formula, getCellValue);
            Console.WriteLine($"Testing {formula}: Result = {result}, Expected = {expected}");

            if (Math.Abs(Convert.ToDouble(result) - expected) < 0.0001)
            {
                Console.WriteLine("Test Passed");
            }
            else
            {
                Console.WriteLine("Test Failed");
            }
        }
    }
}
