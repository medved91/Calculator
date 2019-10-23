using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calculator.CountingService.Operations;

namespace Calculator.CountingService
{
    public class MathExpressionParser : IMathExpressionParser
    {
        private readonly IEnumerable<IOperation> _operations;

        public MathExpressionParser(IEnumerable<IOperation> operations)
        {
            _operations = operations;
        }

        public IEnumerable<string> ParseInfixToReversePolish(string infixMathExpression)
        {
            var tokens = ParseToTokens(infixMathExpression);

            var tokenStack = new Stack<string>();
            var outputList = new List<string>();

            foreach (var token in tokens)
            {
                if (IsNumber(token))
                {
                    outputList.Add(token);
                }

                if (token == "(")
                {
                    tokenStack.Push(token);
                }
                
                if (token == ")")
                {
                    while (tokenStack.Count != 0 && tokenStack.Peek() != "(")
                    {
                        outputList.Add(tokenStack.Pop());
                    }

                    tokenStack.Pop();
                }

                if (IsOperation(token))
                {
                    while (tokenStack.Count != 0 && GetPriority(tokenStack.Peek()) >= GetPriority(token))
                    {
                        outputList.Add(tokenStack.Pop());
                    }
                    tokenStack.Push(token);
                }
            }
            while (tokenStack.Count != 0)
            {
                outputList.Add(tokenStack.Pop());
            }

            return outputList;
        }

        private TokenPriority GetPriority(string token) 
            => _operations.FirstOrDefault(o => o.Symbol == token)?.Priority ?? TokenPriority.Low;

        private bool IsOperation(string token) 
            => _operations.Any(o => o.Symbol == token);

        private bool IsNumber(string token)
            => double.TryParse(token, out _);

        private IEnumerable<string> ParseToTokens(string infix)
        {
            var tokens = new List<string>();

            for (var symbolPosition = 0; symbolPosition < infix.Length; symbolPosition++)
            {
                var currentSymbol = infix[symbolPosition].ToString();
                
                if (IsNumber(currentSymbol))
                {
                    var fullNumber = new StringBuilder();
                    var nextSymbolPosition = symbolPosition;
                    while (IsNumber(currentSymbol) && nextSymbolPosition < infix.Length)
                    {
                        fullNumber.Append(currentSymbol);
                        nextSymbolPosition++;
                        currentSymbol = infix[nextSymbolPosition].ToString();
                    }

                    tokens.Add(fullNumber.ToString());
                }

                if(currentSymbol == "(" || currentSymbol == ")")
                {
                    tokens.Add(currentSymbol);
                }

                if(string.IsNullOrWhiteSpace(currentSymbol)) continue;

                var operationFound = false;
                var nextSymbolPos = symbolPosition;
                var parsedOperationName = new StringBuilder();

                while (!operationFound && nextSymbolPos < infix.Length)
                {
                    parsedOperationName.Append(currentSymbol);
                    nextSymbolPos++;
                    currentSymbol = infix[nextSymbolPos].ToString();

                    var operation = _operations.FirstOrDefault(o => o.Symbol == parsedOperationName.ToString());
                    operationFound = operation != null;

                    if(IsNumber(currentSymbol) || string.IsNullOrWhiteSpace(currentSymbol) || currentSymbol == "(" || currentSymbol == ")")
                        throw new ApplicationException($"Недопустимая операция: {parsedOperationName}");
                }

                if(!operationFound) throw new ApplicationException($"Недопустимая операция: {parsedOperationName}");

                tokens.Add(parsedOperationName.ToString());
            }

            return tokens;
        }
    }
}
