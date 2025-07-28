using Jace;
using Microsoft.Maui.Controls;
using System;
using System.Globalization;
using System.Linq;

namespace Calc_App
{
    public partial class MainPage : ContentPage
    {
        string expression = "";
        char[] operators = ['+', '-', '*', '/', ',', '.', '^', '('];

        public MainPage()
        {
            InitializeComponent();
            var culture = CultureInfo.CurrentCulture;

            if (culture.TwoLetterISOLanguageName == "tr" || culture.TwoLetterISOLanguageName =="ar")
            {
                CultureButton.Text = ","; // Turkish uses comma as decimal separator
            }            
        }

        private async void OnButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            string value = button.Text;

            switch (value)
            {
                case "=":
                    if (operators.Contains(expression.Last()) || !IsBracesCountsEqualed(expression))
                        return;
                    try
                    {
                        var engine = new CalculationEngine();
                        var result = engine.Calculate(expression);
                        expression = result.ToString();
                        if (expression == "∞")
                        {
                            //make the result show for 2 seconds then clear it
                            ResultLabel.Text = expression;
                            await Task.Delay(1000);
                            expression = "";
                        }
                    }
                    catch
                    {
                        expression = "Hata";
                        ResultLabel.Text = expression;
                        await Task.Delay(1000);
                        expression = "";
                    }
                    break;
                case "c":
                    expression = "";
                    break;
                case "d":
                    if (!string.IsNullOrEmpty(expression))
                        expression = expression.Remove(expression.Length - 1);
                    break;
                case "+":
                case "-":
                case "*":
                case "/":
                case "^":
                case ",":
                case ".":
                    if (expression.Length == 0 || operators.Contains(expression.Last()))
                        return;
                    expression += value;
                    break;
                case "(":
                    if (expression.Length == 0 || operators.Contains(expression.Last()))
                        expression += value;
                    break;
                case ")":
                    if (expression.Length > 0 && !operators.Contains(expression.Last()) && !IsBracesCountsEqualed(expression))
                        expression += value;
                    break;
                default:
                    expression += value;
                    break;
            }

            ResultLabel.Text = expression;
        }

        private bool IsBracesCountsEqualed(string exp)
        {
            int opened = exp.Count(x => x == '(');
            int closed = exp.Count(x => x == ')');
            return opened == closed;
        }
    }
}
