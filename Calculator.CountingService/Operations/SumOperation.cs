namespace Calculator.CountingService.Operations
{
    public class SumOperation : IOperation
    {
        public string Symbol => "+";
        public OperationPriority Priority => OperationPriority.Medium;
        public double Count(double left, double right) => left + right;
    }
}