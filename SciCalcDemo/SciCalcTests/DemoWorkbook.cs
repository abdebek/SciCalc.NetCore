using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;

namespace SciCalcDemo.SciCalcTests;

public class DemoWorkbook
{
    public static DataTable ReadExcel()
    {
        DataTable dtTable = new DataTable();
        List<string> rowList = new List<string>();
        ISheet sheet;
        // Open the Excel file containing the test data copied from the original Excel computation results
        using (var stream = new FileStream("C:\\Users\\A\\source\\repos\\SciCalc\\SciCalcDemo\\assets\\XLMathCopy.xlsx", FileMode.Open))
        {
            stream.Position = 0;
            XSSFWorkbook xssWorkbook = new XSSFWorkbook(stream);
            sheet = xssWorkbook.GetSheetAt(0);
            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;
            for (int j = 0; j < cellCount; j++)
            {
                ICell cell = headerRow.GetCell(j);
                if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
                {
                    dtTable.Columns.Add(cell.ToString());
                }
            }
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null) continue;
                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                    {
                        if (!string.IsNullOrEmpty(row.GetCell(j).ToString()) && !string.IsNullOrWhiteSpace(row.GetCell(j).ToString()))
                        {
                            rowList.Add(row.GetCell(j).ToString());
                        }
                    }
                }
                if (rowList.Count > 0)
                    dtTable.Rows.Add(rowList.ToArray());
                rowList.Clear();
            }
        }

        return dtTable;
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