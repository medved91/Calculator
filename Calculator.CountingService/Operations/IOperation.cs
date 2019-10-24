namespace Calculator.CountingService.Operations
{
    public interface IOperation
    {
        string Symbol { get; }
        OperationPriority Priority { get; }
        double Count(double left, double right);
    }
}