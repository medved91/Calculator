using System;
using System.Collections.Generic;
using System.Linq;
using Calculator.CountingService.Operations;

namespace Calculator.CountingService
{
    public class CountingService : ICountingService
    {
        private readonly IMathExpressionParser _mathExpressionParser;
        private readonly IEnumerable<IOperation> _operations;

        public CountingService(
            IMathExpressionParser mathExpressionParser,
            IEnumerable<IOperation> operations)
        {
            _mathExpressionParser = mathExpressionParser;
            _operations = operations;
        }

        public double Count(string infixMathExpression)
        {
            var reversePolish = _mathExpressionParser.ParseInfixToReversePolish(infixMathExpression);
            return CountReversePolish(reversePolish);
        }

        public double CountReversePolish(IEnumerable<string> reversePolishInput)
        {
            var values = new Stack<double>();
            foreach (var token in reversePolishInput)
            {
                if (double.TryParse(token, out var number))
                {
                    values.Push(number);
                    continue;
                }

                var operation = _operations.FirstOrDefault(o => o.Symbol == token);
                var rightNumber = values.Pop();
                var leftNumber = values.Pop();

                if(operation == null) throw new ApplicationException($"Неизвестная операция: {token}");

                var result = operation.Count(leftNumber, rightNumber);
                values.Push(result);
            }

            if (values.Count > 1) throw new ApplicationException("Чисел введено больше, чем операций");

            return values.Pop();
        }
    }
}
