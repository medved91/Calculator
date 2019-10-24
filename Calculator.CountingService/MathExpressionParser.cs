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
                    continue;
                }

                if (token == "(")
                {
                    tokenStack.Push(token);
                    continue;
                }
                
                if (token == ")")
                {
                    while (tokenStack.Count != 0 && tokenStack.Peek() != "(")
                    {
                        outputList.Add(tokenStack.Pop());
                    }

                    tokenStack.Pop();
                    continue;
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

        private OperationPriority GetPriority(string token) 
            => _operations.FirstOrDefault(o => o.Symbol == token)?.Priority ?? OperationPriority.Low;

        private bool IsOperation(string token) 
            => _operations.Any(o => o.Symbol == token);

        private bool IsNumber(string token)
            => double.TryParse(token, out _);

        private bool IsDecimalSeparator(string symbol)
            => symbol == "," || symbol == ".";

        private IEnumerable<string> ParseToTokens(string infix)
        {
            var tokens = new List<string>();
            var currentSymbolPosition = 0;

            while (currentSymbolPosition < infix.Length)
            {
                var currentSymbol = infix[currentSymbolPosition].ToString();
                
                if (IsNumber(currentSymbol))
                {
                    var fullNumber = new StringBuilder();
                    var nextSymbolPosition = currentSymbolPosition;
                    var numberParsed = false;
                    var lastParsedSymbol = currentSymbol;

                    while (!numberParsed && nextSymbolPosition < infix.Length)
                    {
                        if (IsDecimalSeparator(lastParsedSymbol) && !IsNumber(currentSymbol))
                            throw new ApplicationException($"Некорректный символ: {currentSymbol}");
                        if (!IsDecimalSeparator(currentSymbol) && !IsNumber(currentSymbol) && IsNumber(lastParsedSymbol))
                        {
                            numberParsed = true;
                            continue;
                        }

                        if(IsNumber(currentSymbol))
                            fullNumber.Append(currentSymbol);
                        else
                        if(IsDecimalSeparator(currentSymbol))
                            fullNumber.Append(".");
                        else
                            throw new ApplicationException($"Некорректный символ: {currentSymbol}");
                        
                        lastParsedSymbol = currentSymbol;
                        nextSymbolPosition++;
                        if(nextSymbolPosition < infix.Length) currentSymbol = infix[nextSymbolPosition].ToString();
                    }

                    tokens.Add(fullNumber.ToString());
                    currentSymbolPosition = nextSymbolPosition;
                    continue;
                }

                if(currentSymbol == "(" || currentSymbol == ")")
                {
                    tokens.Add(currentSymbol);
                    currentSymbolPosition++;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(currentSymbol))
                {
                    currentSymbolPosition++;
                    continue;
                }

                var operationFound = false;
                var nextSymbolPos = currentSymbolPosition;
                var parsedOperationName = new StringBuilder(currentSymbol);

                while (!operationFound && nextSymbolPos < infix.Length)
                {
                    nextSymbolPos++;

                    var operation = _operations.FirstOrDefault(o => o.Symbol == parsedOperationName.ToString());
                    
                    if (operation != null)
                    {
                        operationFound = true;
                        continue;
                    }

                    if(nextSymbolPos<infix.Length) currentSymbol = infix[nextSymbolPos].ToString();
                    parsedOperationName.Append(currentSymbol);
                    
                    if(IsNumber(currentSymbol) || string.IsNullOrWhiteSpace(currentSymbol) || currentSymbol == "(" || currentSymbol == ")")
                        throw new ApplicationException($"Не найдена операция, начинающаяся на: {parsedOperationName}");
                }

                tokens.Add(parsedOperationName.ToString());
                currentSymbolPosition = nextSymbolPos;
            }

            return tokens;
        }
    }
}
