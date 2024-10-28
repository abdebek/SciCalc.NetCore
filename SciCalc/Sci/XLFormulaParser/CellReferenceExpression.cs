namespace SciCalc;

public partial class Sci
{
    public partial class XLFormulaParser
    {
        private class CellReferenceExpression : Expression
        {
            private readonly string reference;

            public CellReferenceExpression(string reference)
            {
                this.reference = reference;
            }

            public override object Evaluate(Func<string, object> getCellValue)
            {
                return getCellValue(reference);
            }
        }
    }
}