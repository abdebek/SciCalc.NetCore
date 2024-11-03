using static SciCalc.Sci;
using SciCalcDemo.SciCalcTests;

CalculatorTests.RunAllFixedPointCalculationTests();
CalculatorTests.RunAllExcelLikePrecisionCalculationTests();

XLFormulaParserTests.RunManuallyCuratedTests();
XLFormulaParserTests.RunDemoWorkbookTests();

// Test Helpers.RoundTo10Digits method rounds to a max of 10 digits the way Excel does
var numbers = new double[] { 507.24691361, 0.0123456789, 10.0123456789 };
foreach (var number in numbers)
{
    Console.WriteLine($"Original Number: {number}, Rounded Number (To 10 Digits): {Helpers.RoundTo10Digits(number)}");
}