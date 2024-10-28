namespace SciCalcDemo.SciCalcTests;

public static class CellData
{
    private static readonly Dictionary<string, double> cellValues = new();

    // Populates cell values for each test case
    public static void SetCellValues(Dictionary<string, double> values)
    {
        cellValues.Clear();  // Clear previous values
        foreach (var kvp in values)
        {
            cellValues[kvp.Key] = kvp.Value;
        }
    }

    // Retrieves a cell's value, throws an exception if the cell doesn't exist
    public static string GetCellValue(string cellAddress)
    {
        return cellValues.TryGetValue(cellAddress, out var value)
            ? value.ToString()
            : throw new ArgumentException($"Cell '{cellAddress}' not found in cell values.");
    }
}