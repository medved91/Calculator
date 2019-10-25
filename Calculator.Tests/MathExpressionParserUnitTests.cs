using System;
using System.Collections.Generic;
using Calculator.CountingService;
using Calculator.CountingService.Operations;
using FluentAssertions;
using Xunit;

namespace Calculator.Tests
{
    public class MathExpressionParserUnitTests
    {
        private readonly MathExpressionParser _mathExpressionParser;
        
        public MathExpressionParserUnitTests()
        {
            var operations = new List<IOperation>
            {
                new SumOperation(), 
                new SubstractOperation(), 
                new MultiplyOperation(), 
                new DivideOperation(), 
                new TestOperationWithWordSymbol()
            };

            _mathExpressionParser = new MathExpressionParser(operations);
        }

        [Theory]
        [InlineData("1 + 1", new [] {"1","1","+"})]
        [InlineData("1+1/1*1-1", new [] {"1","1","1","/","1","*","+","1","-"})]
        [InlineData("1 +       3+4     *5   ", new [] {"1","3","+","4","5","*","+"})]
        [InlineData("1 +(2+3)/2", new [] {"1","2","3","+","2","/","+"})]
        [InlineData("1,3 +(2.2+3)/2", new [] {"1.3","2.2","3","+","2","/","+"})]
        [InlineData("1test2    test  (35test44)", new [] {"1","2","test","35","44","test","test"})]
        [InlineData(" 1 ", new [] {"1"})]
        [InlineData("1test2", new [] {"1","2","test"})]
        public void ShouldParse_WhenNoMistakes(string infixInput, string[] expectedResult)
        {
            var result = _mathExpressionParser.ParseInfixToReversePolish(infixInput);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [InlineData("1.LOL", "Некорректный символ: L")]
        [InlineData("1.23.", "Лишний десятичный разделитель")]
        [InlineData("1.23..", "Лишний десятичный разделитель")]
        [InlineData("1 tes 2", "Неизвестный оператор: tes")]
        [InlineData("1 testt 2", "Неизвестный оператор: t")]
        [InlineData("()4", "Перед числом должна быть открывающая скобка или оператор")]
        [InlineData("(*)", "Перед оператором должно быть число или закрывающая скобка")]
        [InlineData(")()", "Неправильная расстановка скобок")]
        [InlineData("1*2\\222", "Неизвестный оператор: \\")]
        [InlineData("asd1*222", "Неизвестный оператор: asd")]
        [InlineData("1*222a", "Неизвестный оператор: a")]
        [InlineData("1*/", "Выражение должно заканчиваться числом или закрывающей скобкой")]
        public void ShouldThrow_WhenHasMistakes(string infixInput, string exceptionMessage)
        {
            _mathExpressionParser
                .Invoking(p => p.ParseInfixToReversePolish(infixInput))
                .Should().Throw<ApplicationException>()
                .WithMessage(exceptionMessage);
        }
    }
}
