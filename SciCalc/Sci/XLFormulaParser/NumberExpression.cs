namespace SciCalc;

public partial class Sci
{
    public partial class XLFormulaParser
    {
        private class NumberExpression : Expression
        {
            private readonly double value;

            public NumberExpression(double value)
            {
                this.value = value;
            }

            public override object Evaluate(Func<string, object> getCellValue)
            {
                return value;
            }
        }
    }
}