namespace SciCalc;

public partial class Sci
{
    public partial class XLFormulaParser
    {

        private abstract class Expression
        {
            public abstract object Evaluate(Func<string, object> getCellValue);
        }

        private readonly Dictionary<string, Func<List<double>, double>> functions;
        private readonly Dictionary<string, Func<List<object>, object>> lookupFunctions;

        public XLFormulaParser()
        {
            functions = new Dictionary<string, Func<List<double>, double>>
        {
            {"SUM", values => values.Sum()},
            {"SUBTRACT", values => values.Skip(1).Aggregate(values.First(), (acc, val) => acc - val) },
            {"AVERAGE", values => values.Average()},
            {"COUNT", values => values.Count},
            {"MAX", values => values.Max()},
            {"MIN", values => values.Min()},
            {"MEDIAN", values => {
                var sorted = values.OrderBy(v => v).ToList();
                int n = sorted.Count;
                return n % 2 == 0
                    ? (sorted[n/2 - 1] + sorted[n/2]) / 2
                    : sorted[n/2];
            }},
            {"STDEV", values => {
                double avg = values.Average();
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                return Math.Sqrt(sum / (values.Count - 1));
            }},
            {"VAR", values => {
                double avg = values.Average();
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                return sum / (values.Count - 1);
            }},
            {"ROUND", values => Math.Round(values[0], (int)values[1])},
            {"ABS", values => Math.Abs(values[0])},
            {"POWER", values => Math.Pow(values[0], values[1])},
            {"SQRT", values => Math.Sqrt(values[0])},
            {"LOG", values => values.Count > 1 ? Math.Log(values[0], values[1]) : Math.Log(values[0])},
            {"LOG10", values => Math.Log10(values[0])},
            {"CEILING", values => Math.Ceiling(values[0])},
            {"FLOOR", values => Math.Floor(values[0])},
            {"MOD", values => values[0] % values[1]},
            {"PRODUCT", values => values.Aggregate(1.0, (acc, val) => acc * val)}
        };

            lookupFunctions = new Dictionary<string, Func<List<object>, object>>();
        }

        public void AddLookupFunction(string name, Func<List<object>, object> function)
        {
            lookupFunctions[name.ToUpper()] = function;
        }

        public object Evaluate(string formula, Func<string, object> getCellValue)
        {
            var parser = new Parser(formula, functions, lookupFunctions);
            var expression = parser.Parse();
            return expression.Evaluate(getCellValue);
        }
    }
}