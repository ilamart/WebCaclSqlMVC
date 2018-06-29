using System;
using System.Collections.Generic;
using System.Linq;

namespace AppCalculate
{
    public class StringCalc
    {
        private static Operation[] _operations =
        {
            #region Init array
            new OperationPlus(),
            new OperationMinus(),
            new OperationMultiply(),
            new OperationDivide(),
            new OperationExpone(),
            new OperationMyDivide()
            #endregion
        };

        public double Result { get; private set; }

        public StringCalc()
        {
        }

        public double DoCalculation(string input)
        {
            var rezult = ConvertNotation(input);
            return Calculation(rezult);
        }

        private string ConvertNotation(string input)
        {
            string output = string.Empty;
            Stack<char> operStack = new Stack<char>();

            string availableOperators = string.Empty;
            availableOperators += "()";
            foreach (Operation elem in _operations)
            {
                if (availableOperators.IndexOf(elem.OperatorsCode) == -1)
                    availableOperators += elem.OperatorsCode;
            }

            for (int i = 0; i < input.Length; i++)
            {
                if (IsDelimeter(input[i]))
                    continue;

                if (input[i] == '-' && ((i > 0 && !Char.IsDigit(input[i - 1])) || i == 0))
                {
                    i++;
                    output += "-";
                }

                if (Char.IsDigit(input[i]))
                {
                    while (!IsDelimeter(input[i]) && !(availableOperators.IndexOf(input[i]) != -1))
                    {
                        output += input[i];
                        i++;
                        if (i == input.Length) break;
                    }
                    output += " ";
                    i--;
                }

                if (availableOperators.IndexOf(input[i]) == -1)
                    continue;

                if (input[i] == '(')
                {
                    operStack.Push(input[i]);
                }
                else if (input[i] == ')')
                {
                    char s = operStack.Pop();
                    while (s != '(')
                    {
                        output += s.ToString() + ' ';
                        s = operStack.Pop();
                    }
                }
                else
                {
                    if (operStack.Count > 0)
                    {
                        if (GetPriority(input[i].ToString()) <= GetPriority(operStack.Peek().ToString()))
                            output += operStack.Pop().ToString() + " ";
                    }
                    operStack.Push(char.Parse(input[i].ToString()));
                }
            }

            while (operStack.Count > 0)
                output += operStack.Pop() + " ";

            return output;
        }

        private static int GetPriority(string s)
        {
            return _operations.FirstOrDefault(o => o.OperatorsCode == s)?.Priority ?? 0;
        }

        private static bool IsDelimeter(char c)
        {
            return " =".IndexOf(c) != -1;
        }

        private double Calculation(string output)
        {
            var list = new List<string>(output.Split(' '));
            foreach (var s in list)
            {
                Console.WriteLine(s + " ");
            }

            for (int i = 0; i < list.Count; i++)
            {
                foreach (Operation item in _operations)
                {
                    if (item.OperatorsCode == list[i])
                    {
                        var DubResult = item.GetResult(double.Parse(list[i - 2]), double.Parse(list[i - 1])).ToString();
                        list[i - 2] = DubResult.ToString();
                        for (int j = i - 1; j < list.Count - 2; j++)
                            list[j] = list[j + 2];
                        list.RemoveRange(list.Count - 2, 2);
                        i -= 2;
                    }
                }
            }
            return double.Parse(list[0]);
        }
    }
}