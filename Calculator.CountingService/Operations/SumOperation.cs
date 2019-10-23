namespace Calculator.CountingService.Operations
{
    public class SumOperation : IOperation
    {
        public string Symbol => "+";
        public TokenPriority Priority => TokenPriority.Medium;
        public double Count(double left, double right) => left + right;
    }
}