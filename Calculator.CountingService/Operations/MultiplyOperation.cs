namespace Calculator.CountingService.Operations
{
    public class MultiplyOperation : IOperation
    {
        public string Symbol => "*";
        public TokenPriority Priority => TokenPriority.High;
        public double Count(double left, double right) => left * right;
    }
}