using System;
using System.Collections.Generic;
using System.Text;

namespace SampleArchitecture.Core.Utils.ExpressionHelper
{

    internal class Token
    {
        public object value { get; set; }
        public TokenType TokenType { get; set; }


        internal static List<Token> OperatorTokens
        {
            get
            {
                return new List<Token>()
                {
            new Token() { value= "&&", TokenType= TokenType.BinaryOperator },
            new Token() { value= "||", TokenType= TokenType.BinaryOperator },
            new Token() { value= "=", TokenType= TokenType.AssignmentOperator },
            new Token() { value= "!=", TokenType= TokenType.AssignmentOperator },
            new Token() { value= "%", TokenType= TokenType.AssignmentOperator },
            new Token() { value= "(", TokenType= TokenType.OpenBracket },
            new Token() { value= ")", TokenType= TokenType.CloseBracket }
                };

            }
        }

    }
}
