namespace Calculator.CountingService.Numbers
{
    public class Number : INumber
    {
        public string Symbol { get; }
        public TokenPriority Priority => TokenPriority.Low;
        public double Value { get; set; }
    }
}
