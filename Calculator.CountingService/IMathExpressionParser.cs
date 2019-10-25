using System.Collections.Generic;

namespace Calculator.CountingService
{
    public interface IMathExpressionParser
    {
        /// <summary>
        /// Конверсия инфиксного математического выражения в обратную польскую нотацию
        /// </summary>
        /// <param name="infixMathExpression">Инфиксное математическое выражение</param>
        /// <returns>Эквивалентная входному инфиксному выражению польская нотация</returns>
        IEnumerable<string> ParseInfixToReversePolish(string infixMathExpression);
    }
}