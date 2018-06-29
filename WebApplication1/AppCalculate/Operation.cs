using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCalculate
{
    public abstract class Operation
    {
        public abstract string OperatorsCode { get; }

        public abstract int Priority { get; }

        public abstract double GetResult(double operand1, double operand2);
    }
}