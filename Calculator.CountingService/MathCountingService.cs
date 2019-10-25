using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Calculator.CountingService.Operations;

namespace Calculator.CountingService
{
    public class MathCountingService : IMathCountingService
    {
        private readonly IMathExpressionParser _mathExpressionParser;
        private readonly IEnumerable<IOperation> _operations;

        public MathCountingService(
            IMathExpressionParser mathExpressionParser,
            IEnumerable<IOperation> operations)
        {
            _mathExpressionParser = mathExpressionParser;
            _operations = operations;
        }

        public double Count(string infixMathExpression)
        {
            var reversePolish = _mathExpressionParser
                .ParseInfixToReversePolish(infixMathExpression);
            return CountReversePolish(reversePolish);
        }

        public double CountReversePolish(IEnumerable<string> reversePolishInput)
        {
            var values = new Stack<double>();
            foreach (var token in reversePolishInput)
            {
                if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out var number))
                {
                    values.Push(number);
                    continue;
                }

                var operation = _operations.First(o => o.Symbol == token);
                if(!values.Any()) 
                    throw new ApplicationException("Некорректное выражение");
                var rightNumber = values.Pop();
                if(!values.Any())
                    throw new ApplicationException("Некорректное выражение");
                var leftNumber = values.Pop();

                var result = operation.Count(leftNumber, rightNumber);
                values.Push(result);
            }
            
            return values.Pop();
        }
    }
}
