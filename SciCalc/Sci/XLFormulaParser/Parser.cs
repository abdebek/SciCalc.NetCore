namespace SciCalc;

public partial class Sci
{
    public partial class XLFormulaParser
    {
        private class Parser
        {
            private readonly List<Token> tokens;
            private int position;
            private readonly Dictionary<string, (int precedence, bool rightAssoc)> operators;
            private readonly Dictionary<string, Func<List<double>, double>> functions;
            private readonly Dictionary<string, Func<List<object>, object>> lookupFunctions;

            public Parser(string formula, Dictionary<string, Func<List<double>, double>> functions,
                Dictionary<string, Func<List<object>, object>> lookupFunctions)
            {
                this.tokens = Tokenize(formula);
                this.position = 0;
                this.functions = functions;
                this.lookupFunctions = lookupFunctions;

                this.operators = new Dictionary<string, (int precedence, bool rightAssoc)>
            {
                {"^", (4, true)},
                {"*", (3, false)},
                {"/", (3, false)},
                {"+", (2, false)},
                {"-", (2, false)},
                {"=", (1, false)},
                {"<", (1, false)},
                {">", (1, false)},
                {"<=", (1, false)},
                {">=", (1, false)},
                {"<>", (1, false)}
            };
            }

            public Expression Parse()
            {
                return ParseExpression(0);
            }

            private Expression ParseExpression(int precedence)
            {
                Expression left = ParsePrimary();

                while (position < tokens.Count)
                {
                    var token = tokens[position];
                    if (!operators.ContainsKey(token.Value))
                        break;

                    var (nextPrecedence, rightAssoc) = operators[token.Value];
                    if (nextPrecedence < precedence)
                        break;

                    position++;
                    var right = ParseExpression(rightAssoc ? nextPrecedence : nextPrecedence + 1);
                    left = new BinaryExpression(left, token.Value, right);
                }

                return left;
            }

            private Expression ParsePrimary()
            {
                var token = tokens[position++];

                switch (token.Type)
                {
                    case "NUMBER":
                        return new NumberExpression(double.Parse(token.Value));

                    case "CELL":
                        return new CellReferenceExpression(token.Value);

                    case "FUNCTION":
                        Expect("(");
                        var args = ParseArguments();
                        Expect(")");
                        return new FunctionExpression(token.Value, args, functions, lookupFunctions);

                    case "OPERATOR" when token.Value == "(":
                        var expr = ParseExpression(0);
                        Expect(")");
                        return expr;

                    case "RANGE":
                        var parts = token.Value.Split(':');
                        return new RangeExpression(parts[0], parts[1]);

                    default:
                        throw new ArgumentException($"Unexpected token: {token.Value}");
                }
            }

            private List<Expression> ParseArguments()
            {
                var args = new List<Expression>();
                while (position < tokens.Count && tokens[position].Value != ")")
                {
                    args.Add(ParseExpression(0));
                    if (tokens[position].Value == ",")
                        position++;
                }
                return args;
            }

            private void Expect(string expected)
            {
                if (position >= tokens.Count || tokens[position].Value != expected)
                    throw new ArgumentException($"Expected '{expected}'");
                position++;
            }

            private List<Token> Tokenize(string formula)
            {
                var tokens = new List<Token>();
                var position = 0;
                formula = formula.Replace(" ", "").ToUpper();

                while (position < formula.Length)
                {
                    var c = formula[position];

                    if (char.IsDigit(c) || c == '.')
                    {
                        var value = "";
                        var start = position;
                        while (position < formula.Length && (char.IsDigit(formula[position]) || formula[position] == '.'))
                            value += formula[position++];
                        tokens.Add(new Token("NUMBER", value, start));
                    }
                    else if (char.IsLetter(c))
                    {
                        var value = "";
                        var start = position;
                        while (position < formula.Length && (char.IsLetterOrDigit(formula[position]) || formula[position] == ':'))
                        {
                            value += formula[position++];
                            if (formula[position - 1] == ':')
                            {
                                while (position < formula.Length && char.IsLetterOrDigit(formula[position]))
                                    value += formula[position++];
                                tokens.Add(new Token("RANGE", value, start));
                                goto continueLoop;
                            }
                        }

                        if (position < formula.Length && formula[position] == '(')
                            tokens.Add(new Token("FUNCTION", value, start));
                        else
                            tokens.Add(new Token("CELL", value, start));
                    }
                    else if (c == '(' || c == ')' || c == ',')
                    {
                        tokens.Add(new Token("OPERATOR", c.ToString(), position++));
                    }
                    else
                    {
                        var op = "";
                        var start = position;
                        while (position < formula.Length && IsOperatorChar(formula[position]))
                            op += formula[position++];
                        if (operators.ContainsKey(op))
                            tokens.Add(new Token("OPERATOR", op, start));
                        else
                            throw new ArgumentException($"Unknown operator: {op}");
                    }

                continueLoop:;
                }

                return tokens;
            }

            private bool IsOperatorChar(char c)
            {
                return "+-*/^<>=".Contains(c);
            }
        }
    }
}