using System.Globalization;
using UnityEngine;

namespace NullSave.TOCK.Stats
{

    public static class LogicExtensions
    {

        #region Constants

        public const string BASIC_EXPRESSIONS = "=+-*/^";
        public const string FULL_EXPRESSIONS = "=+-*/^<>()";

        #endregion

        #region Public Methods

        public static float EvaluateSimpleMath(this string equation)
        {
            int open, close;
            string exp;

            equation = equation.Replace(" ", string.Empty);

            // Parentheses
            while (true)
            {
                close = equation.IndexOf(')');
                if (close > -1)
                {
                    open = equation.LastIndexOf('(', close, close + 1);
                    exp = equation.Substring(open + 1, close - open - 1);

                    equation = equation.Substring(0, open) + exp.EvaluateSimpleMath().ToString(CultureInfo.InvariantCulture) + equation.Substring(close + 1);
                }
                else break;
            }

            // Exponents
            equation = HandlePEMDAS(equation, '^');

            // Multiplication
            equation = HandlePEMDAS(equation, '*');

            // Division
            equation = HandlePEMDAS(equation, '/');

            // Addition
            equation = HandlePEMDAS(equation, '+');

            // Subtraction
            equation = HandlePEMDAS(equation, '-');

            return float.Parse(equation, CultureInfo.InvariantCulture);
        }

        #endregion

        #region Private Methods

        private static string HandlePEMDAS(string equation, char op)
        {
            int opLoc;
            int left, right;
            string res = string.Empty;

            while (true)
            {
                // Find sides
                opLoc = equation.IndexOf(op, 1);
                if (opLoc == -1) return equation;
                left = LeftValueStart(equation, opLoc);
                right = RightValueStart(equation, opLoc);

                // Perform math
                switch (op)
                {
                    case '^':
                        res = Mathf.Pow(float.Parse(equation.Substring(left, opLoc - left), CultureInfo.InvariantCulture), float.Parse(equation.Substring(opLoc + 1, right - opLoc), CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture);
                        break;
                    case '*':
                        res = (float.Parse(equation.Substring(left, opLoc - left), CultureInfo.InvariantCulture) * float.Parse(equation.Substring(opLoc + 1, right - opLoc), CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture);
                        break;
                    case '/':
                        res = (float.Parse(equation.Substring(left, opLoc - left), CultureInfo.InvariantCulture) / float.Parse(equation.Substring(opLoc + 1, right - opLoc), CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture);
                        break;
                    case '+':
                        res = (float.Parse(equation.Substring(left, opLoc - left), CultureInfo.InvariantCulture) + float.Parse(equation.Substring(opLoc + 1, right - opLoc), CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture);
                        break;
                    case '-':
                        res = (float.Parse(equation.Substring(left, opLoc - left), CultureInfo.InvariantCulture) - float.Parse(equation.Substring(opLoc + 1, right - opLoc), CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture);
                        break;
                }

                // Replace in equation
                equation = equation.Replace(equation.Substring(left, right - left + 1), res);
            }
        }

        private static int LeftValueStart(string equation, int opLoc)
        {
            int startFrom = opLoc - 1;

            while (startFrom > 0)
            {
                if (BASIC_EXPRESSIONS.Contains(equation.Substring(startFrom, 1)))
                {
                    startFrom += 1;
                    break;
                }
                startFrom -= 1;
            }

            return startFrom;
        }

        private static int RightValueStart(string equation, int opLoc)
        {
            int startFrom = opLoc + 1;

            while (startFrom < equation.Length - 1)
            {
                if (BASIC_EXPRESSIONS.Contains(equation.Substring(startFrom, 1)))
                {
                    startFrom -= 1;
                    break;
                }
                startFrom += 1;
            }

            return startFrom;
        }

        #endregion

    }
}