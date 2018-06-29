using System;

namespace AppCalculate
{
    public class OperationMyDivide : Operation
    {
        public override int Priority { get { return 7; } }

        public override string OperatorsCode { get { return "%"; } }

        public override double GetResult(double operand1, double operand2)
        {
            int oper1 = Convert.ToInt32(operand1);
            int oper2 = Convert.ToInt32(operand2);
            int rez = oper1 % oper2;
            return rez;
        }
    }
}
