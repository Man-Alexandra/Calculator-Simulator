using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;


namespace Calculator
{
    internal class AritmeticOperations
    {
        private bool awaitingNumberAfterRadical = false;
        private bool awaitingNumberAfterPower = false;
        private bool awaitingNumberAfterFraction = false;

        public string ProcessInput(string currentText, string buttonContent)
        {
            if (IsNumber(buttonContent))
            {
                return HandleNumberInput(currentText, buttonContent);
            }

            return buttonContent switch
            {
                "%" or "+" or "-" or "*" or "/" => AppendOperator(currentText, buttonContent),
                "1/x" => StartFraction(currentText),
                "x²" => StartPower(currentText),
                "√x" => StartRadical(currentText),
                "+/-" => ToggleSign(currentText),
                "." => AppendDecimalPoint(currentText),
                "CE" => ClearEntry(currentText),
                "C" => "0",
                "=" => EvaluateExpression(currentText),
                _ => currentText
            };
        }

        private bool IsNumber(string input) => double.TryParse(input, out _);

        private string HandleNumberInput(string currentText, string buttonContent)
        {
            if (awaitingNumberAfterRadical)
            {
                awaitingNumberAfterRadical = false;
                return currentText + buttonContent + ")";
            }
            if (awaitingNumberAfterPower)
            {
                awaitingNumberAfterPower = false;
                return currentText + buttonContent + "²)";
            }
            if (awaitingNumberAfterFraction)
            {
                return HandleFractionNumber(currentText, buttonContent);
            }

            return currentText + buttonContent;
        }

        private string HandleFractionNumber(string currentText, string buttonContent)
        {
            if (buttonContent == "0")
            {
                MessageBox.Show("Error: Division by zero is not allowed!");
                awaitingNumberAfterFraction = false;
                return currentText.TrimEnd('1', '/', '(');
            }

            awaitingNumberAfterFraction = false;
            return currentText + buttonContent + ")";
        }

        private string AppendOperator(string currentText, string buttonContent)
        {
            // Dacă expresia curentă este goală, nu adăugăm operatorul
            if (string.IsNullOrEmpty(currentText))
            {
                MessageBox.Show("Please enter a number first.");
                return currentText; // Returnăm expresia curentă fără să adăugăm operator
            }
            // Dacă expresia se termină deja cu un operator, nu mai adăuga altul
            if (currentText.EndsWith("+") || currentText.EndsWith("-") || currentText.EndsWith("*") || currentText.EndsWith("/"))
            {
                MessageBox.Show("Cannot add operator after another operator!");
                return currentText; // returnează expresia curentă fără a adăuga un alt operator
            }

            // Dacă expresia este validă și nu se termină cu un operator, adăugăm operatorul
            string newText = currentText + buttonContent;

            // Evaluează expresia intermediară
             return EvaluateIntermediateExpression(newText);
            //return newText;
        }
        private string EvaluateIntermediateExpression(string expression)
        {
            try
            {
                // Înlocuirea simbolurilor pentru a evalua expresia
                expression = expression.Replace("√(", "Math.Sqrt(");
                expression = Regex.Replace(expression, @"(\d+)²", "Math.Pow($1, 2)");

                // Adăugăm .0 pentru a manipula ca pe un double
                //expression = Regex.Replace(expression, @"(\d+)", "$1.0");

                // Verificăm dacă expresia este validă
                if (IsValidExpression(expression))
                {
                    // Evaluarea expresiei
                    var result = CSharpScript.EvaluateAsync<double>(expression, options: ScriptOptions.Default.AddReferences(typeof(Math).Assembly).AddImports("System")).Result;

                    // Formatarea rezultatului
                    if (result % 1 == 0)
                        return result.ToString("F0");
                    else
                        return result.ToString("F4");
                }
                else
                {
                    return expression; // Returnăm expresia incompletă pentru a o permite editării
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return "0";
            }
        }

        // Funcția pentru a verifica dacă expresia este validă
        private bool IsValidExpression(string expression)
        {
            // Verificăm dacă expresia conține operatori și numere valide
            // Verificăm dacă expresia nu se termină cu un operator
            if (string.IsNullOrEmpty(expression) || "+-*/%".Contains(expression[expression.Length - 1]))
                return false;

            // Verificăm dacă există paranteze neînchise
            int openParentheses = expression.Count(c => c == '(');
            int closeParentheses = expression.Count(c => c == ')');
            if (openParentheses != closeParentheses)
                return false;

            // Altfel, expresia este validă
            return true;
        }
        private string StartFraction(string currentText)
        {
            awaitingNumberAfterFraction = true;
            return currentText + "1/(";
        }

        private string StartPower(string currentText)
        {
            awaitingNumberAfterPower = true;
            return currentText + "(";
        }

        private string StartRadical(string currentText)
        {
            awaitingNumberAfterRadical = true;
            return currentText + "√(";
        }

        private string ToggleSign(string currentText) =>
            currentText.StartsWith("-") ? currentText[1..] : "-" + currentText;

        private string AppendDecimalPoint(string currentText) =>
            currentText.Contains(".") ? currentText : currentText + ".";

        private string ClearEntry(string currentText)
        {
            if (string.IsNullOrEmpty(currentText))
                return "0";

            while (currentText.Length > 0 && !"%+-*/".Contains(currentText[^1]))
            {
                currentText = currentText[..^1];
            }

            return string.IsNullOrEmpty(currentText) ? "0" : currentText;
        }

        
        private string EvaluateExpression(string expression)
        {
            try
            {
                expression = expression.Replace("√(", "Math.Sqrt(");
                expression = System.Text.RegularExpressions.Regex.Replace(expression, @"(\d+)²", "Math.Pow($1, 2)");
                expression = Regex.Replace(expression, @"(\d+)", "$1.0");

                MessageBox.Show("Expression: " + expression);


                var result = CSharpScript.EvaluateAsync<double>(expression, options: ScriptOptions.Default.AddReferences(typeof(Math).Assembly).AddImports("System")).Result;

                if (result % 1 == 0)
                    return result.ToString("F0");
                else
                    return result.ToString("F4");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return "0";
            }
        }

        // Digit grouping
        public string ApplyDigitGrouping(string expression)
        {
            Match match = Regex.Match(expression, @"(\d+)(?!.*\d)");
            if (match.Success)
            {
                string number = match.Value;
                string formattedNumber = FormatWithDigitGrouping(number);
                return expression.Substring(0, match.Index) + formattedNumber;
            }
            return expression;
        }

        private string FormatWithDigitGrouping(string number)
        {
            if (double.TryParse(number, out double value))
            {
                return value.ToString("#,##0", System.Globalization.CultureInfo.InvariantCulture).Replace(",", ".");
            }
            return number;
        }
    }
}
