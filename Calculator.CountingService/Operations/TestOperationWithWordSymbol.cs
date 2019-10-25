namespace Calculator.CountingService.Operations
{
    public class TestOperationWithWordSymbol : IOperation
    {
        public string Symbol => "test";
        public OperationPriority Priority => OperationPriority.High;
        public double Count(double left, double right) => left + right;
    }
}