using System.Collections.Generic;
using Calculator.CountingService;
using Calculator.CountingService.Operations;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Calculator.Tests
{
    public class MathCountingServiceUnitTests
    {
        private readonly MathCountingService _mathCountingService;
        private readonly IMathExpressionParser _fakeMathExpressionParser;
        
        public MathCountingServiceUnitTests()
        {
            var operations = new List<IOperation>
            {
                new SumOperation(), 
                new SubstractOperation(), 
                new MultiplyOperation(), 
                new DivideOperation(), 
                new TestOperationWithWordSymbol()
            };

            _fakeMathExpressionParser = A.Fake<IMathExpressionParser>();
            _mathCountingService = new MathCountingService(_fakeMathExpressionParser, operations);
        }

        [Theory]
        [InlineData("1 + 1", new[] { "1", "1", "+" }, 2)]
        [InlineData("1+1/1*1-1", new[] { "1", "1", "1", "/", "1", "*", "+", "1", "-" }, 1)]
        [InlineData("1 +       3+4     *5   ", new[] { "1", "3", "+", "4", "5", "*", "+" }, 24)]
        [InlineData("1 +(2+3)/2", new[] { "1", "2", "3", "+", "2", "/", "+" }, 3.5)]
        [InlineData("1,3 +(2.2+3)/2", new[] { "1.3", "2.2", "3", "+", "2", "/", "+" }, 3.9000000000000004)]
        [InlineData("1test2    test  (35test44)", new[] { "1", "2", "test", "35", "44", "test", "test" }, 82)]
        [InlineData(" 1 ", new[] { "1" }, 1)]
        [InlineData("1test2", new[] { "1", "2", "test" }, 3)]
        public void ShouldCount_WhenCorrectInput(string infixInput, string[] parsedInfix, double expectedResult)
        {
            A.CallTo(() => _fakeMathExpressionParser.ParseInfixToReversePolish(infixInput)).Returns(parsedInfix);

            var result = _mathCountingService.Count(infixInput);

            result.Should().Be(expectedResult);
        }
    }
}
