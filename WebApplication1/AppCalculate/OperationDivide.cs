namespace AppCalculate
{
    public class OperationDivide : Operation
    {
        public override int Priority { get { return 5; } }

        public override string OperatorsCode { get { return "/"; } }

        public override double GetResult(double operand1, double operand2)
        {
            return operand1 / operand2;
        }
    }
}
