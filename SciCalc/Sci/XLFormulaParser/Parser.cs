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
            private readonly Dictionary<string, Func<List<decimal>, decimal>> functions;
            private readonly Dictionary<string, Func<List<object>, object>> lookupFunctions;

            public Parser(string formula,
                Dictionary<string, Func<List<decimal>, decimal>> functions,
                Dictionary<string, Func<List<object>, object>> lookupFunctions)
            {
                if (string.IsNullOrWhiteSpace(formula))
                    throw new ArgumentException("Formula cannot be empty", nameof(formula));

                this.functions = functions ?? throw new ArgumentNullException(nameof(functions));
                this.lookupFunctions = lookupFunctions ?? throw new ArgumentNullException(nameof(lookupFunctions));
                this.position = 0;


                // Initialize operators with precedence and associativity
                this.operators = new Dictionary<string, (int precedence, bool rightAssoc)>(StringComparer.OrdinalIgnoreCase)
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

                try
                {
                    this.tokens = Tokenize(formula);
                }
                catch (Exception ex)
                {
                    throw new FormatException($"Error tokenizing formula: {ex.Message}", ex);
                }
            }

            public Expression Parse()
            {
                try
                {
                    var expression = ParseExpression(0);
                    if (position < tokens.Count)
                    {
                        throw new FormatException($"Unexpected token after expression: {tokens[position].Value}");
                    }
                    return expression;
                }
                catch (Exception ex) when (ex is not FormatException)
                {
                    throw new FormatException($"Error parsing formula: {ex.Message}", ex);
                }
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
                if (position >= tokens.Count)
                    throw new FormatException("Unexpected end of formula");

                var token = tokens[position++];

                switch (token.Type)
                {
                    case "NUMBER":
                        if (!decimal.TryParse(token.Value, out decimal number))
                            throw new FormatException($"Invalid number format: {token.Value}");
                        return new NumberExpression(number);

                    case "CELL":
                        if (string.IsNullOrWhiteSpace(token.Value))
                            throw new FormatException("Empty cell reference");
                        return new CellReferenceExpression(token.Value);

                    case "FUNCTION":
                        if (string.IsNullOrWhiteSpace(token.Value))
                            throw new FormatException("Empty function name");
                        if (!ExpectSafe("("))
                            throw new FormatException($"Expected '(' after function {token.Value}");
                        var args = ParseArguments();
                        if (!ExpectSafe(")"))
                            throw new FormatException($"Expected ')' after function arguments in {token.Value}");
                        return new FunctionExpression(token.Value, args, functions, lookupFunctions);

                    case "OPERATOR" when token.Value == "(":
                        var expr = ParseExpression(0);
                        if (!ExpectSafe(")"))
                            throw new FormatException("Unmatched parenthesis");
                        return expr;

                    case "RANGE":
                        var parts = token.Value.Split(':');
                        if (parts.Length != 2)
                            throw new FormatException($"Invalid range format: {token.Value}");
                        if (string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
                            throw new FormatException($"Invalid range reference: {token.Value}");
                        return new RangeExpression(parts[0], parts[1]);

                    default:
                        throw new FormatException($"Unexpected token: {token.Value}");
                }
            }

            private List<Expression> ParseArguments()
            {
                var args = new List<Expression>();
                while (position < tokens.Count && tokens[position].Value != ")")
                {
                    if (args.Any() && tokens[position].Value != ",")
                        throw new FormatException("Expected ',' between function arguments");

                    if (args.Any())
                        position++; // Skip the comma

                    args.Add(ParseExpression(0));
                }
                return args;
            }

            private bool ExpectSafe(string expected)
            {
                if (position >= tokens.Count || tokens[position].Value != expected)
                    return false;
                position++;
                return true;
            }

            private List<Token> Tokenize(string formula)
            {
                var tokens = new List<Token>();
                var position = 0;
                formula = formula.Trim().ToUpper(); // Preserve spaces between tokens

                while (position < formula.Length)
                {
                    var c = formula[position];

                    if (char.IsWhiteSpace(c))
                    {
                        position++;
                        continue;
                    }

                    if (char.IsDigit(c) || c == '.')
                    {
                        var value = "";
                        var start = position;
                        var hasDecimalPoint = false;

                        while (position < formula.Length &&
                               (char.IsDigit(formula[position]) ||
                               (!hasDecimalPoint && formula[position] == '.')))
                        {
                            if (formula[position] == '.')
                                hasDecimalPoint = true;
                            value += formula[position++];
                        }

                        if (value.EndsWith("."))
                            throw new FormatException($"Invalid number format: {value}");

                        tokens.Add(new Token("NUMBER", value, start));
                    }
                    else if (char.IsLetter(c))
                    {
                        var value = "";
                        var start = position;
                        while (position < formula.Length &&
                               (char.IsLetterOrDigit(formula[position]) || formula[position] == ':'))
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
                    else if ("(),".Contains(c))
                    {
                        tokens.Add(new Token("OPERATOR", c.ToString(), position++));
                    }
                    else if (IsOperatorChar(c))
                    {
                        var op = "";
                        var start = position;
                        while (position < formula.Length && IsOperatorChar(formula[position]))
                            op += formula[position++];

                        if (operators.ContainsKey(op))
                            tokens.Add(new Token("OPERATOR", op, start));
                        else
                            throw new FormatException($"Unknown operator: {op}");
                    }
                    else
                    {
                        throw new FormatException($"Invalid character in formula: {c}");
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