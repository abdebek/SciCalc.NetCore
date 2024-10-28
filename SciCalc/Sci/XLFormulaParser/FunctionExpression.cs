namespace SciCalc;

public partial class Sci
{
    public partial class XLFormulaParser
    {
        private class FunctionExpression : Expression
        {
            private readonly string name;
            private readonly List<Expression> arguments;
            private readonly Dictionary<string, Func<List<double>, double>> functions;
            private readonly Dictionary<string, Func<List<object>, object>> lookupFunctions;

            public FunctionExpression(string name, List<Expression> arguments,
                Dictionary<string, Func<List<double>, double>> functions,
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

            private List<double> FlattenToNumericList(List<object> args)
            {
                var result = new List<double>();
                foreach (var arg in args)
                {
                    if (arg is double d)
                    {
                        result.Add(d);
                    }
                    else if (arg is List<object> list)
                    {
                        result.AddRange(list.Select(item => Convert.ToDouble(item)));
                    }
                }
                return result;
            }
        }
    }
}