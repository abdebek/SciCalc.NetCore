namespace SciCalcDemo.SciCalcTests;

public static class TestData
{
    // Define test cases with formula, cell values, and expected result
    public static readonly List<(string formula, Dictionary<string, double> cells, double expected)> TestCases = new()
        {
            (
                "SUM(A1:A100)",
                GenerateSequentialCells("A", 1, 100), // Populates A1 to A100 with values 1 through 100
                5050
            ),
            ( "SUM(A1:A2)",
                new Dictionary<string, double> { { "A1", 12.1034 }, { "A2", 5.67 } },
                17.7734
            ),
            (
                "SUBTRACT(A98:A100)",
                GenerateSequentialCells("A", 98, 100), // Populates A98 to A100 with values 98 through 100
                -101
            ),
            (
                "SUBTRACT(A1:A2)",
                new Dictionary<string, double> { { "A1", 12.1034 }, { "A2", 5.67 } },
                6.4334
            ),
            (
                "PRODUCT(A1:A3)",
                new Dictionary<string, double> { { "A1", 1 }, { "A2", 2 }, { "A3", 3 } },
                6
            ),
            (
                "PRODUCT(A1:A2)",
                new Dictionary<string, double> { { "A1", 12.1034 }, { "A2", 5.67 } },
                68.626278
            ),
            (
                "PRODUCT(A1:A2)",
                new Dictionary<string, double> { { "A1", 12.10342345 }, { "A2", 5.67 } },
                68.62641094
            ),
            (
                "PRODUCT(A1:A2)",
                new Dictionary<string, double> { { "A1", 121.10342345 }, { "A2", 5.67 } },
                686.6564109
            ),
            (
                "DIVIDE(A1:A2)",
                new Dictionary<string, double> { { "A1", 12.1034 }, { "A2", 5.67 } },
                2.134638448 // 12.1034 / 5.67 as computed by excel
            ),
            (
                "DIVIDE(B1:B2)",
                new Dictionary<string, double> { { "B1", 0.1234 }, { "B2", 1.1 } },
                0.112181818 // 0.1234 / 1.1 as computed by excel
            ),
            (
                "DIVIDE(A1:A2)",
                new Dictionary<string, double> { { "A1", 12.10342345 }, { "A2", 5.67 } },
                2.134642583
            ),
        };

    // Helper to generate sequential cells with incrementing values
    public static Dictionary<string, double> GenerateSequentialCells(string column, int start, int end)
    {
        var cells = new Dictionary<string, double>();
        for (int i = start; i <= end; i++)
        {
            cells[$"{column}{i}"] = i;
        }
        return cells;
    }
}
