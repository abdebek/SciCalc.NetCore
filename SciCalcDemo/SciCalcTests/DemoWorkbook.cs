using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;

namespace SciCalcDemo.SciCalcTests;

public class DemoWorkbook
{
    private static readonly string ExcelFilePath = Path.Combine(AppContext.BaseDirectory, "assets", "XLMathCopy.xlsx");

    public static DataTable ReadExcel()
    {
        DataTable dataTable = new DataTable();

        using var stream = new FileStream(ExcelFilePath, FileMode.Open, FileAccess.Read);
        using var workbook = new XSSFWorkbook(stream);
        ISheet sheet = workbook.GetSheetAt(0);

        if (sheet == null)
            throw new Exception("No sheet found in Excel file.");

        AddColumns(sheet, dataTable);
        AddRows(sheet, dataTable);

        return dataTable;
    }

    private static void AddColumns(ISheet sheet, DataTable dataTable)
    {
        IRow headerRow = sheet.GetRow(0);
        int cellCount = headerRow.LastCellNum;

        for (int j = 0; j < cellCount; j++)
        {
            ICell cell = headerRow.GetCell(j);
            if (cell != null && !string.IsNullOrWhiteSpace(cell.ToString()))
            {
                dataTable.Columns.Add(cell.ToString());
            }
        }
    }

    private static void AddRows(ISheet sheet, DataTable dataTable)
    {
        List<string> rowData = new List<string>();
        int cellCount = sheet.GetRow(0).LastCellNum;

        for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
        {
            IRow row = sheet.GetRow(i);
            if (row == null || row.Cells.All(cell => cell.CellType == CellType.Blank))
                continue;

            rowData.Clear();

            for (int j = row.FirstCellNum; j < cellCount; j++)
            {
                ICell cell = row.GetCell(j);
                rowData.Add(cell?.ToString() ?? string.Empty);
            }

            dataTable.Rows.Add(rowData.ToArray());
        }
    }

    public static void DisplayData(DataTable table)
    {
        foreach (DataRow row in table.Rows)
        {
            foreach (DataColumn col in table.Columns)
            {
                Console.WriteLine("{0} = {1}", col.ColumnName, row[col]);
            }
            Console.WriteLine("-------------------------------------------");
        }
    }
}
