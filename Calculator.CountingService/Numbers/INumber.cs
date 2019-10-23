using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator.CountingService.Numbers
{
    public interface INumber : IToken
    {
        double Value { get; set; }
    }
}
