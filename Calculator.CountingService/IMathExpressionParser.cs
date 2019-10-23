using System.Collections.Generic;

namespace Calculator.CountingService
{
    public interface IMathExpressionParser
    {
        IEnumerable<string> ParseInfixToReversePolish(string infixMathExpression);
    }
}