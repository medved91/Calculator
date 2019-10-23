namespace Calculator.CountingService
{
    public interface IToken
    {
        string Symbol { get; }
        TokenPriority Priority { get; }
    }
}