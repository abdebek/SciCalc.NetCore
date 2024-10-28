using System.Text.RegularExpressions;

namespace SciCalc;

public partial class Sci
{
    public partial class XLFormulaParser
    {
        private class RangeExpression : Expression
        {
            private readonly string startCell;
            private readonly string endCell;

            public RangeExpression(string startCell, string endCell)
            {
                this.startCell = startCell;
                this.endCell = endCell;
            }

            public override object Evaluate(Func<string, object> getCellValue)
            {
                var values = new List<object>();
                var (startCol, startRow) = ParseCellReference(startCell);
                var (endCol, endRow) = ParseCellReference(endCell);

                for (int row = startRow; row <= endRow; row++)
                {
                    for (var col = startCol; CompareColumns(col, endCol) <= 0; col = IncrementColumn(col))
                    {
                        values.Add(getCellValue($"{col}{row}"));
                    }
                }

                return values;
            }

            private (string col, int row) ParseCellReference(string cell)
            {
                var match = Regex.Match(cell, @"([A-Z]+)(\d+)");
                return (match.Groups[1].Value, int.Parse(match.Groups[2].Value));
            }

            private int CompareColumns(string col1, string col2)
            {
                return string.Compare(col1, col2, StringComparison.Ordinal);
            }

            private string IncrementColumn(string column)
            {
                var chars = column.ToCharArray();
                int i = chars.Length - 1;

                while (i >= 0)
                {
                    if (chars[i] < 'Z')
                    {
                        chars[i]++;
                        return new string(chars);
                    }
                    chars[i] = 'A';
                    i--;
                }

                return "A" + new string(chars);
            }
        }
    }
}