using System;
using System.Globalization;
using Calculator.CountingService;
using Calculator.Web.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Calculator.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MathController : ControllerBase
    {
        private readonly IMathCountingService _mathCountingService;

        public MathController(IMathCountingService mathCountingService)
        {
            _mathCountingService = mathCountingService;
        }

        [HttpPost("calculateinfix")]
        public ActionResult<MathCountResult> CalculateInfix([FromBody] CalculatorMathExpression calculatorMathExpression)
        {
            double result;
            
            try
            {
                result = _mathCountingService.Count(calculatorMathExpression.InfixExpression);
            }
            catch(ApplicationException e)
            {
                return BadRequest($"{e.Message}");
            }

            return new MathCountResult
            {
                Result = result.ToString(CultureInfo.InvariantCulture)
            };
        }
    }
}
