using System.Data;
using static SciCalc.Sci;

namespace SciCalcDemo.SciCalcTests
{
    public static partial class XLFormulaParserTests
    {
        private static readonly XLFormulaParser parser = new XLFormulaParser();
        private static readonly DataTable dtTable = DemoWorkbook.ReadExcel();
        private static readonly Dictionary<int, string> operations = new()
        {
            { 0, "SUM" },
            { 1, "SUBTRACT" },
            { 2, "PRODUCT" },
            { 3, "DIVIDE" }
        };

        public static void DisplayDemoWorkbookData() => DemoWorkbook.DisplayData(dtTable);

        public static void RunDemoWorkbookTests()
        {
            var testCases = GenerateTestCasesFromWorkbook();
            RunTestCases(testCases);
        }

        public static void RunManuallyCuratedTests()
        {
            RunTestCases(TestData.TestCases);
        }

        private static List<(string formula, Dictionary<string, double> cells, double expected)> GenerateTestCasesFromWorkbook()
        {
            var testCases = new List<(string formula, Dictionary<string, double> cells, double expected)>();

            foreach (DataRow row in dtTable.Rows)
            {
                int rowIndex = dtTable.Rows.IndexOf(row);

                foreach (var operation in operations)
                {
                    testCases.Add(CreateTestCase(row, rowIndex, operation.Key, operation.Value));
                }
            }

            return testCases;
        }

        private static (string formula, Dictionary<string, double> cells, double expected) CreateTestCase(DataRow row, int rowIndex, int colOffset, string operation)
        {
            string formula = $"{operation}(A{rowIndex}:B{rowIndex})";
            double expected = Convert.ToDouble(row[colOffset + 2]);
            var cells = new Dictionary<string, double>
            {
                { $"A{rowIndex}", Convert.ToDouble(row[0]) },
                { $"B{rowIndex}", Convert.ToDouble(row[1]) }
            };

            return (formula, cells, expected);
        }

        private static void RunTestCases(IEnumerable<(string formula, Dictionary<string, double> cells, double expected)> testCases)
        {
            foreach (var testCase in testCases)
            {
                var (formula, cells, expected) = testCase;
                EvaluateAndDisplayResult(formula, cells, expected);
            }
        }

        private static void EvaluateAndDisplayResult(string formula, Dictionary<string, double> cells, double expected)
        {
            CellData.SetCellValues(cells);
            var result = parser.Evaluate(formula, CellData.GetCellValue);
            double errorPercentage = CalculateErrorPercentage(result, expected);

            DisplayTestResult(formula, result, expected, errorPercentage);
            CheckTestSuccess(result, expected, formula);
        }

        private static double CalculateErrorPercentage(object result, double expected) =>
            expected == 0 ? 0 : Math.Abs((Convert.ToDouble(result) - expected) / expected) * 100;

        private static void DisplayTestResult(string formula, object result, double expected, double errorPercentage)
        {
            Console.WriteLine($"{formula} Result: {result}, Expected: {expected}, Error Percentage: {errorPercentage}%.");
        }

        private static void CheckTestSuccess(object result, double expected, string formula)
        {
            if (Math.Abs(Convert.ToDouble(result) - expected) > 0) // Success threshold: > 0.1e-6
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
