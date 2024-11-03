namespace SciCalc;

public partial class Sci
{
    public partial class XLFormulaParser
    {
        private abstract class Expression
        {
            public abstract object Evaluate(Func<string, object> getCellValue);
        }

        private readonly Dictionary<string, Func<List<decimal>, decimal>> functions;
        private readonly Dictionary<string, Func<List<object>, object>> lookupFunctions;

        public XLFormulaParser()
        {
            functions = new Dictionary<string, Func<List<decimal>, decimal>>(StringComparer.OrdinalIgnoreCase)
                        {
                            {"SUM", values => Helpers.Summation(values)},
                            {"SUBTRACT", values => Helpers.Subtraction(values) },
                            {"PRODUCT", values => Helpers.Product(values)},
                            {"DIVIDE", values => Helpers.Division(values)},
                            {"AVERAGE", values => Helpers.Average(values)},
                            {"COUNT", values => values.Count},
                            {"MAX", values => values.Any() ? values.Max() : 0m},
                            {"MIN", values => values.Any() ? values.Min() : 0m},
                            {"MEDIAN", values => Helpers.Median(values)},
                            {"STDEV", values => Helpers.StandardDeviation(values)},
                            {"VAR", values => Helpers.Variance(values)},
                            {"ROUND", values => Helpers.Round(values)},
                            {"ABS", values => Helpers.Absolute(values)},
                            {"POWER", values => Helpers.Power(values)},
                            {"SQRT", values => Helpers.SquareRoot(values)},
                            {"LOG", values => Helpers.Logarithm(values)},
                            {"LOG10", values => Helpers.Log10(values)},
                            {"CEILING", values => Helpers.Ceiling(values)},
                            {"FLOOR", values => Helpers.Floor(values)},
                            {"MOD", values => Helpers.Mod(values)}
                        };

            lookupFunctions = new Dictionary<string, Func<List<object>, object>>(StringComparer.OrdinalIgnoreCase);
        }

        public void AddLookupFunction(string name, Func<List<object>, object> function)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Function name cannot be empty", nameof(name));

            lookupFunctions[name] = function ?? throw new ArgumentNullException(nameof(function));
        }

        public object Evaluate(string formula, Func<string, object> getCellValue)
        {
            if (string.IsNullOrWhiteSpace(formula))
                throw new ArgumentException("Formula cannot be empty", nameof(formula));

            if (getCellValue == null)
                throw new ArgumentNullException(nameof(getCellValue));

            var parser = new Parser(formula, functions, lookupFunctions);
            var expression = parser.Parse();
            return expression.Evaluate(getCellValue);
        }
    }
}
