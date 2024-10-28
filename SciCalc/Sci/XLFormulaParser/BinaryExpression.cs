namespace SciCalc;

public partial class Sci
{
    public partial class XLFormulaParser
    {
        private class BinaryExpression : Expression
        {
            private readonly Expression left;
            private readonly string op;
            private readonly Expression right;

            public BinaryExpression(Expression left, string op, Expression right)
            {
                this.left = left;
                this.op = op;
                this.right = right;
            }

            public override object Evaluate(Func<string, object> getCellValue)
            {
                var leftValue = Convert.ToDouble(left.Evaluate(getCellValue));
                var rightValue = Convert.ToDouble(right.Evaluate(getCellValue));

                return op switch
                {
                    "+" => leftValue + rightValue,
                    "-" => leftValue - rightValue,
                    "*" => leftValue * rightValue,
                    "/" => leftValue / rightValue,
                    "^" => Math.Pow(leftValue, rightValue),
                    "=" => Convert.ToDouble(leftValue == rightValue),
                    "<" => Convert.ToDouble(leftValue < rightValue),
                    ">" => Convert.ToDouble(leftValue > rightValue),
                    "<=" => Convert.ToDouble(leftValue <= rightValue),
                    ">=" => Convert.ToDouble(leftValue >= rightValue),
                    "<>" => Convert.ToDouble(leftValue != rightValue),
                    _ => throw new ArgumentException($"Unknown operator: {op}")
                };
            }
        }
    }
}