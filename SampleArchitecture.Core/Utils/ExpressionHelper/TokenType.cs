using System;
using System.Collections.Generic;
using System.Text;

namespace SampleArchitecture.Core.Utils.ExpressionHelper
{
    internal enum TokenType { Propertystring, Expresssion, BinaryOperator = 8, AssignmentOperator = 9, OpenBracket = 10, CloseBracket }
}
