using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCalculate
{
    public class OperationExpone : Operation
    {
        public override int Priority { get { return 6; } }

        public override string OperatorsCode { get { return "^"; } }

        public override double GetResult(double operand1, double operand2)
        {
            for (int i = 0; i < (operand2 - 1); i++)
            {
                operand1 *= operand1;
            }
            return operand1;
        }
    }
}
