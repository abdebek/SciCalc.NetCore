namespace SciCalc;

public partial class Sci
{
    public partial class XLFormulaParser
    {
        private class Token
        {
            public string Type { get; set; }
            public string Value { get; set; }
            public int Position { get; set; }

            public Token(string type, string value, int position)
            {
                Type = type;
                Value = value;
                Position = position;
            }
        }
    }
}
