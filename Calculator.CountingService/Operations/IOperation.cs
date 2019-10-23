namespace Calculator.CountingService.Operations
{
    public interface IOperation : IToken
    {
        double Count(double left, double right);
    }
}