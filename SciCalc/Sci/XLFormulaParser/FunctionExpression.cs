using System.Globalization;
namespace SciCalc;

public partial class Sci
{
    public partial class XLFormulaParser
    {
        private class FunctionExpression : Expression
        {
            private readonly string name;
            private readonly List<Expression> arguments;
            private readonly Dictionary<string, Func<List<decimal>, decimal>> functions;
            private readonly Dictionary<string, Func<List<object>, object>> lookupFunctions;

            public FunctionExpression(string name, List<Expression> arguments,
                Dictionary<string, Func<List<decimal>, decimal>> functions,
                Dictionary<string, Func<List<object>, object>> lookupFunctions)
            {
                this.name = name;
                this.arguments = arguments;
                this.functions = functions;
                this.lookupFunctions = lookupFunctions;
            }

            public override object Evaluate(Func<string, object> getCellValue)
            {
                var evaluatedArgs = arguments.Select(arg => arg.Evaluate(getCellValue)).ToList();
                if (functions.ContainsKey(name))
                {
                    var numericArgs = FlattenToNumericList(evaluatedArgs);
                    return functions[name](numericArgs);
                }
                else if (lookupFunctions.ContainsKey(name))
                {
                    return lookupFunctions[name](evaluatedArgs);
                }
                throw new ArgumentException($"Unknown function: {name}");
            }

            private List<decimal> FlattenToNumericList(List<object> evaluatedArgs)
            {
                var numericList = new List<decimal>();
                foreach (var arg in evaluatedArgs)
                {
                    if (arg == null)
                    {
                        continue;
                    }

                    if (arg is IEnumerable<object> list)
                    {
                        numericList.AddRange(FlattenToNumericList(list.ToList()));
                        continue;
                    }

                    decimal? value = ConvertToDecimal(arg);
                    if (value.HasValue)
                    {
                        numericList.Add(value.Value);
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid argument type - {arg.GetType()}. Expected decimal, numeric string, or list of numeric values.");
                    }
                }
                return numericList;
            }

            private decimal? ConvertToDecimal(object value)
            {
                try
                {
                    switch (value)
                    {
                        case decimal decimalValue:
                            return decimalValue;
                        case int intValue:
                            return Convert.ToDecimal(intValue);
                        case float floatValue:
                            return Convert.ToDecimal(floatValue);
                        case double doubleValue:
                            return Convert.ToDecimal(doubleValue);
                        case string strValue:
                            if (decimal.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedDecimal))
                            {
                                return parsedDecimal;
                            }
                            if (double.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedDouble))
                            {
                                return Convert.ToDecimal(parsedDouble);
                            }
                            return null;
                        default:
                            return null;
                    }
                }
                catch (OverflowException)
                {
                    return null;
                }
            }
        }
    }
}