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
            functions = new Dictionary<string, Func<List<decimal>, decimal>>(StringComparer.OrdinalIgnoreCase)  // Case insensitive comparison
            {
                {"SUM", values =>
                    values.Any()
                        ? values.Skip(1).Aggregate(values.First(), (acc, val) =>
                            FixedPointCalculator<decimal>.Add(acc, val))
                        : 0m},

                {"SUBTRACT", values =>
                    values.Any()
                        ? values.Skip(1).Aggregate(values.First(), (acc, val) =>
                            FixedPointCalculator<decimal>.Subtract(acc, val))
                        : 0m},

                {"PRODUCT", values =>
                    values.Any()
                        ? values.Skip(1).Aggregate(values.First(), (acc, val) =>
                            FixedPointCalculator<decimal>.Multiply(acc, val))
                        : 0m},

                {"DIVIDE", values => {
                    if (!values.Any()) return 0m;
                    if (values.Skip(1).Any(v => v == 0))
                        throw new DivideByZeroException("Cannot divide by zero");
                    return values.Skip(1).Aggregate(values.First(), (acc, val) =>
                        FixedPointCalculator<decimal>.Divide(acc, val));
                }},

                {"AVERAGE", values => {
                    if (!values.Any()) return 0m;
                    return FixedPointCalculator<decimal>.Divide(
                        values.Aggregate(decimal.Zero, (acc, val) =>
                            FixedPointCalculator<decimal>.Add(acc, val)),
                        decimal.CreateChecked(values.Count));
                }},

                {"COUNT", values => decimal.CreateChecked(values.Count)},

                {"MAX", values => values.Any() ? values.Max() : 0m},

                {"MIN", values => values.Any() ? values.Min() : 0m},

                {"MEDIAN", values => {
                    if (!values.Any()) return 0m;
                    var sorted = values.OrderBy(v => v).ToList();
                    int n = sorted.Count;
                    if (n % 2 == 0)
                    {
                        var mid1 = sorted[n/2 - 1];
                        var mid2 = sorted[n/2];
                        return FixedPointCalculator<decimal>.Divide(
                            FixedPointCalculator<decimal>.Add(mid1, mid2),
                            2m);
                    }
                    return sorted[n/2];
                }},

                {"STDEV", values => {
                    if (values.Count < 2)
                        throw new ArgumentException("Standard deviation requires at least two values");

                    var avg = functions["AVERAGE"](values);
                    var sumSquaredDiff = values.Aggregate(decimal.Zero, (acc, val) => {
                        var diff = FixedPointCalculator<decimal>.Subtract(val, avg);
                        return FixedPointCalculator<decimal>.Add(acc,
                            FixedPointCalculator<decimal>.Multiply(diff, diff));
                    });
                    var variance = FixedPointCalculator<decimal>.Divide(
                        sumSquaredDiff,
                        decimal.CreateChecked(values.Count - 1));
                    return decimal.CreateChecked(Math.Sqrt((double)variance));
                }},

                {"VAR", values => {
                    if (values.Count < 2)
                        throw new ArgumentException("Variance requires at least two values");

                    var avg = functions["AVERAGE"](values);
                    var sumSquaredDiff = values.Aggregate(decimal.Zero, (acc, val) => {
                        var diff = FixedPointCalculator<decimal>.Subtract(val, avg);
                        return FixedPointCalculator<decimal>.Add(acc,
                            FixedPointCalculator<decimal>.Multiply(diff, diff));
                    });
                    return FixedPointCalculator<decimal>.Divide(
                        sumSquaredDiff,
                        decimal.CreateChecked(values.Count - 1));
                }},

                {"ROUND", values => {
                    if (values.Count != 2)
                        throw new ArgumentException("ROUND requires exactly two arguments");
                    return decimal.Round(values[0], (int)values[1]);
                }},

                {"ABS", values => {
                    if (values.Count != 1)
                        throw new ArgumentException("ABS requires exactly one argument");
                    return decimal.Abs(values[0]);
                }},

                {"POWER", values => {
                    if (values.Count != 2)
                        throw new ArgumentException("POWER requires exactly two arguments");
                    return decimal.CreateChecked(Math.Pow((double)values[0], (double)values[1]));
                }},

                {"SQRT", values => {
                    if (values.Count != 1)
                        throw new ArgumentException("SQRT requires exactly one argument");
                    if (values[0] < 0)
                        throw new ArgumentException("Cannot calculate square root of negative number");
                    return decimal.CreateChecked(Math.Sqrt((double)values[0]));
                }},

                {"LOG", values => {
                    if (values.Count == 0 || values.Count > 2)
                        throw new ArgumentException("LOG requires one or two arguments");
                    if (values[0] <= 0)
                        throw new ArgumentException("Cannot calculate logarithm of zero or negative number");

                    return values.Count > 1
                        ? decimal.CreateChecked(Math.Log((double)values[0], (double)values[1]))
                        : decimal.CreateChecked(Math.Log((double)values[0]));
                }},

                {"LOG10", values => {
                    if (values.Count != 1)
                        throw new ArgumentException("LOG10 requires exactly one argument");
                    if (values[0] <= 0)
                        throw new ArgumentException("Cannot calculate logarithm of zero or negative number");
                    return decimal.CreateChecked(Math.Log10((double)values[0]));
                }},

                {"CEILING", values => {
                    if (values.Count != 1)
                        throw new ArgumentException("CEILING requires exactly one argument");
                    return decimal.Ceiling(values[0]);
                }},

                {"FLOOR", values => {
                    if (values.Count != 1)
                        throw new ArgumentException("FLOOR requires exactly one argument");
                    return decimal.Floor(values[0]);
                }},

                {"MOD", values => {
                    if (values.Count != 2)
                        throw new ArgumentException("MOD requires exactly two arguments");
                    if (values[1] == 0)
                        throw new DivideByZeroException("Cannot calculate MOD with zero divisor");
                    return FixedPointCalculator<decimal>.Subtract(
                        values[0],
                        FixedPointCalculator<decimal>.Multiply(
                            decimal.Floor(FixedPointCalculator<decimal>.Divide(values[0], values[1])),
                            values[1]));
                }}
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