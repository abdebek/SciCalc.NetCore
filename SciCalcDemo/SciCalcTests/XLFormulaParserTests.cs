using static SciCalc.Sci;

namespace SciCalcDemo.SciCalcTests;

public static partial class XLFormulaParserTests
{
    private static XLFormulaParser parser = new XLFormulaParser();
    // Define test cases with formula, cell values, and expected result
    private static readonly List<(string formula, Dictionary<string, double> cells, double expected)> testCases = TestData.TestCases;

    public static void RunAllTests()
    {
        foreach (var (formula, cells, expected) in testCases)
        {
            CellData.SetCellValues(cells);
            var result = parser.Evaluate(formula, CellData.GetCellValue);

            Console.WriteLine($"{formula} Result: {result}, Expected: {expected}");

            if (Math.Abs(Convert.ToDouble(result) - expected) > 0.001)
            {
                Console.WriteLine($"Test failed for formula: {formula}");
            }
            else
            {
                Console.WriteLine("Test passed.");
            }

            Console.WriteLine();
        }
    }
}
