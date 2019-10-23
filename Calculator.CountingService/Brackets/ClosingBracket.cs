namespace Calculator.CountingService.Brackets
{
    public class ClosingBracket : IBracket
    {
        public string Symbol => ")";
        public TokenPriority Priority => TokenPriority.Low;
    }
}