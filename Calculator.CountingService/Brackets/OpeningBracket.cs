namespace Calculator.CountingService.Brackets
{
    public class OpeningBracket : IBracket
    {
        public string Symbol => "(";
        public TokenPriority Priority => TokenPriority.Low;
    }
}