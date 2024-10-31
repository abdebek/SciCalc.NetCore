# SciCalc

A library that supports excel-like basic mathematical computations. Scientific computations that require the rules of significant figures are also supported.

## SciCalc - XLFormulaParser

`SciCalc - XLFormulaParser` is a .NET library designed to parse and evaluate Excel-like formulas, including basic mathematical operations, cell references, ranges, and common functions (e.g., `SUM`, `AVERAGE`, `MAX`, etc.). This lightweight parser is ideal for projects that require Excel-like formula processing without depending on Excel itself.

### Features of SciCalc - XLFormulaParser

- **Basic Arithmetic**: Supports operators like `+`, `-`, `*`, `/`, `^`.
- **Comparisons**: Includes operators like `=`, `<>`, `<`, `>`, `<=`, `>=`.
- **Excel Functions**: Evaluate functions such as `SUM`, `AVERAGE`, `COUNT`, `MAX`, `MIN`, `MEDIAN`, `STDEV`, `VAR`, `ROUND`, and more.
- **Cell References and Ranges**: Use cell references (`A1`, `B2`, etc.) and ranges (`A1:B2`) to retrieve and compute values.
- **Custom Functions**: Extend the parser by adding custom functions to suit specific needs.

## SciCalcDemo Project

**SciCalcDemo** is a demo application that showcases the capabilities of the `XLFormulaParser` library, built to parse and evaluate Excel-like formulas. This simple calculator demo provides a GUI for users to input formulas, see calculations in real-time, and explore the power of `XLFormulaParser` in a straightforward environment.

### Features of SciCalcDemo

- **Formula Input**: Enter Excel-style formulas directly into the app.
- **Formula Parsing and Evaluation**: Supports mathematical operations, cell references, ranges, and common Excel functions.

Please, refer to the SciCalcDemo project folder for details.

## License

Copyright © 2024 Waanfeetan LLC

This code is licensed under the MIT License. See the LICENSE file for more details.
