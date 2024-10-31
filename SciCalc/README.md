# SciCalc - XLFormulaParser

`SciCalc - XLFormulaParser` is a .NET library designed to parse and evaluate Excel-like formulas, including basic mathematical operations, cell references, ranges, and common functions (e.g., `SUM`, `AVERAGE`, `MAX`, etc.). This lightweight parser is ideal for projects that require Excel-like formula processing without depending on Excel itself.

## Features

- **Basic Arithmetic**: Supports operators like `+`, `-`, `*`, `/`, `^`.
- **Comparisons**: Includes operators like `=`, `<>`, `<`, `>`, `<=`, `>=`.
- **Excel Functions**: Evaluate functions such as `SUM`, `AVERAGE`, `COUNT`, `MAX`, `MIN`, `MEDIAN`, `STDEV`, `VAR`, `ROUND`, and more.
- **Cell References and Ranges**: Use cell references (`A1`, `B2`, etc.) and ranges (`A1:B2`) to retrieve and compute values.
- **Custom Functions**: Extend the parser by adding custom functions to suit specific needs.

## Installation

You can install `SciCalc - XLFormulaParser` via NuGet:

```bash
dotnet add package SciCalc
```
Or, add it directly to your .csproj file:
```xml
<PackageReference Include="SciCalc" Version="0.1.0" />
```
## Basic Setup

```csharp
using System;
using using static SciCalc.Sci;

var parser = new XLFormulaParser();

// Define a function to get cell values based on their references
Func<string, object> getCellValue = reference =>
{
    // Return a value based on the cell reference; in practice, retrieve from your data source
    return reference switch
    {
        "A1" => 10,
        "A2" => 20,
        _ => 0
    };
};

// Evaluate a simple formula
string formula = "SUM(A1, A2, 30) / 2";
var result = parser.Evaluate(formula, getCellValue);

Console.WriteLine(result);  // Outputs: 30

```

## Adding Custom Functions
```csharp
parser.AddLookupFunction("CUSTOMFUNC", args =>
{
    // Implement your custom function logic here
    return args.Sum(x => Convert.ToDouble(x)) * 2;
});

// Using the custom function in a formula
string customFormula = "CUSTOMFUNC(A1, A2)";
var customResult = parser.Evaluate(customFormula, getCellValue);
Console.WriteLine(customResult);  // Output will depend on the custom function logic

```

## Contributing
1. Fork the repository.
2. Create a new branch for your feature or bug fix.
3. Commit and push your changes.
4. Open a pull request with a clear description of your changes.

### Reporting Issues
Please open an issue in the GitHub repository if you encounter any bugs or have suggestions.

## License
This project is licensed under the MIT License. See the LICENSE file for details.

