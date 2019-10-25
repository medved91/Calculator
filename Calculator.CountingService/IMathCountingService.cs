namespace Calculator.CountingService
{
    public interface IMathCountingService
    {
        /// <summary>
        /// Подсчет результата инфиксного математического выражения
        /// </summary>
        double Count(string infixMathExpression);
    }
}