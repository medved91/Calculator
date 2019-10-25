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

        private IEnumerable<string> ParseToTokens(string infix)
        {
            var tokens = new List<string>();
            var currentSymbolPosition = 0;

            while (currentSymbolPosition < infix.Length)
            {
                var currentSymbol = infix[currentSymbolPosition].ToString();
                
                if (IsNumber(currentSymbol))
                {
                    var fullNumber = GetFullNumber(infix, 
                        currentSymbolPosition, 
                        currentSymbol, 
                        out var nextSymbolPosition);

                    tokens.Add(fullNumber);
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

                var parsedOperationName = GetOperation(infix, 
                    currentSymbolPosition, 
                    currentSymbol, 
                    out var nextSymbolPos);

                tokens.Add(parsedOperationName);
                currentSymbolPosition = nextSymbolPos;
            }

            ValidateTokenSequence(tokens);

            return tokens;
        }

        private string GetOperation(string infix, 
            int currentSymbolPosition, 
            string currentSymbol, 
            out int nextSymbolPos)
        {
            var operationFound = false;
            nextSymbolPos = currentSymbolPosition;
            var parsedOperationName = new StringBuilder();

            while (!operationFound && nextSymbolPos < infix.Length)
            {
                parsedOperationName.Append(currentSymbol);

                nextSymbolPos++;

                var operation = _operations
                    .FirstOrDefault(o => o.Symbol == parsedOperationName.ToString());

                if (operation != null)
                {
                    operationFound = true;
                    continue;
                }

                if (nextSymbolPos < infix.Length) 
                    currentSymbol = infix[nextSymbolPos].ToString();

                if(IsNumber(currentSymbol) || string.IsNullOrWhiteSpace(currentSymbol))
                    throw new ApplicationException($"Неизвестный оператор: {parsedOperationName}");
            }

            var operationName = parsedOperationName.ToString();
            if (!_operations.Any(o => o.Symbol == operationName))
                throw new ApplicationException($"Неизвестный оператор: {operationName}");
           
            return operationName;
        }

        private string GetFullNumber(string infix, 
            int currentSymbolPosition, 
            string currentSymbol,
            out int nextSymbolPosition)
        {
            var fullNumber = new StringBuilder();
            nextSymbolPosition = currentSymbolPosition;
            var numberParsed = false;
            var lastParsedSymbol = currentSymbol;
            var decimalSeparatorFound = false;

            while (!numberParsed && nextSymbolPosition < infix.Length)
            {
                if (IsDecimalSeparator(lastParsedSymbol) && !IsNumber(currentSymbol))
                    throw new ApplicationException($"Некорректный символ: {currentSymbol}");

                if (!IsDecimalSeparator(currentSymbol) && !IsNumber(currentSymbol) && IsNumber(lastParsedSymbol))
                {
                    numberParsed = true;
                    continue;
                }

                if (IsNumber(currentSymbol))
                    fullNumber.Append(currentSymbol);
                else if (IsDecimalSeparator(currentSymbol))
                {
                    decimalSeparatorFound = decimalSeparatorFound
                        ? throw new ApplicationException("Лишний десятичный разделитель")
                        : true;
                    fullNumber.Append(".");
                }

                lastParsedSymbol = currentSymbol;
                nextSymbolPosition++;
                
                if (nextSymbolPosition < infix.Length) 
                    currentSymbol = infix[nextSymbolPosition].ToString();
            }

            return fullNumber.ToString();
        }

        private void ValidateTokenSequence(IEnumerable<string> tokens)
        {
            tokens = tokens.ToArray();

            string previousToken = null;
            var bracketStack = new Stack<string>();
            var lastToken = tokens.Last();

            if(!IsNumber(lastToken) && lastToken != ")")
                throw new ApplicationException("Выражение должно заканчиваться числом или закрывающей скобкой");

            foreach (var token in tokens)
            {
                if(token == "(") bracketStack.Push(token);
                
                if (token == ")")
                {
                    if(!bracketStack.Any())
                        throw new ApplicationException("Неправильная расстановка скобок");

                    bracketStack.Pop();
                }
                
                if (previousToken == null)
                {
                    previousToken = token;
                    continue;
                }

                if(IsNumber(token) && previousToken != "(" && !IsOperation(previousToken))
                    throw new ApplicationException(
                        "Перед числом должна быть открывающая скобка или оператор");
                
                if(IsOperation(token) && !IsNumber(previousToken) && previousToken != ")")
                    throw new ApplicationException(
                        "Перед оператором должно быть число или закрывающая скобка");

                if(IsNumber(token) && IsNumber(previousToken))
                    throw new ApplicationException(
                        "Перед числом должен быть оператор или открывающая скобка");

                previousToken = token;
            }

            if(bracketStack.Any()) 
                throw new ApplicationException("Неправильная расстановка скобок");
        }

        private OperationPriority GetPriority(string token)
            => _operations.FirstOrDefault(o => o.Symbol == token)?.Priority ?? OperationPriority.Low;

        private bool IsOperation(string token)
            => _operations.Any(o => o.Symbol == token);

        private bool IsNumber(string token)
            => double.TryParse(token, out _);

        private bool IsDecimalSeparator(string symbol)
            => symbol == "," || symbol == ".";
    }
}
