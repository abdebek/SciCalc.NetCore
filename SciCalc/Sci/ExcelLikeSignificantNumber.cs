namespace SciCalc;
public partial class Sci
{
    public class ExcelLikeSignificantNumber : SignificantNumber
    {
        public ExcelLikeSignificantNumber(double value, int significantDigits, int decimalPlaces) : base(Helpers.RoundTo15Digits(value), significantDigits, decimalPlaces)
        {
        }
    }
}