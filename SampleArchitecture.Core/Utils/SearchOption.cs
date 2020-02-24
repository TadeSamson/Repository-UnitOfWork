using SampleArchitecture.Core.Utils.ExpressionHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SampleArchitecture.Core.Utils
{


    public class SearchOption<T>
    {
        public SearchExpression Expression { get; set; }

        public SearchOption<T> ToPascal()
        {
            SearchOption<T> currentSearchOption = this;
            currentSearchOption.Expression= this.ToPascal(currentSearchOption.Expression);
            return currentSearchOption;
        }
        SearchExpression ToPascal(SearchExpression expression)
        {
            SearchOption<T> newSearchOption = new SearchOption<T>();
            if (expression is PropertySearchExpression)
            {
                var propertyExpressionSearch = expression as PropertySearchExpression;
                var asList = propertyExpressionSearch.Property.Split('_').ToList();
                asList = asList.Select(a => a.First().ToString().ToUpper() + a.Substring(1, a.Length - 1)).ToList();
                newSearchOption.Expression = new PropertySearchExpression() 
                { 
                    Property = string.Join("", asList),
                    Operator = propertyExpressionSearch.Operator, 
                    Value = propertyExpressionSearch.Value 
                };
                 
            }

            if (expression is BinarySearchExpression<T>)
            {
                BinarySearchExpression<T> binarySearchExpression = expression as BinarySearchExpression<T>;
                newSearchOption.Expression = new BinarySearchExpression<T>();
                (newSearchOption.Expression as BinarySearchExpression<T>).LeftSearch = ToPascal(binarySearchExpression.LeftSearch);
                (newSearchOption.Expression as BinarySearchExpression<T>).BinaryOperator = binarySearchExpression.BinaryOperator;
                (newSearchOption.Expression as BinarySearchExpression<T>).RightSearch = ToPascal(binarySearchExpression.RightSearch);
            }
            return newSearchOption.Expression;
        }

        public bool Match(T data)
        {
            return this.Match(data, this.Expression);
        }


        //Match function built for repository that doesn't have projection power of search by default. Mearning search will be done after results are received
        bool Match(T data, SearchExpression expression)
        {
            if (expression == null)
                return true;
            if (expression is PropertySearchExpression)
            {
                int tempInt = 0;
                double tempDouble = 0;
                DateTime tempDatetime;
                PropertySearchExpression propertySearchExpression= expression as PropertySearchExpression;
                PropertyInfo pInfo = typeof(T).GetProperty(propertySearchExpression.Property);
                var value=pInfo.GetValue(data);
                switch (propertySearchExpression.Operator)
                {

                    case "=": return (value??"").ToString().Trim().ToLower() == propertySearchExpression.Value.Trim().ToLower();
                    case "!=": return (value ?? "").ToString().Trim().ToLower() != propertySearchExpression.Value.Trim().ToLower();
                    case ">":
                        if (pInfo.GetType() == typeof(int) && int.TryParse(propertySearchExpression.Value, out tempInt))
                            return (int)value > tempInt;
                        if (pInfo.GetType() == typeof(double) && double.TryParse(propertySearchExpression.Value, out tempDouble))
                            return (double)value > tempDouble;
                        if (pInfo.GetType() == typeof(DateTime) && DateTime.TryParse(propertySearchExpression.Value, out tempDatetime))
                            return (DateTime)value > tempDatetime;

                        else return false;
                    case "<":
                        if (pInfo.GetType() == typeof(int) && int.TryParse(propertySearchExpression.Value, out tempInt))
                            return (int)value < tempInt;
                        if (pInfo.GetType() == typeof(double) && double.TryParse(propertySearchExpression.Value, out tempDouble))
                            return (double)value < tempDouble;
                        if (pInfo.GetType() == typeof(DateTime) && DateTime.TryParse(propertySearchExpression.Value, out tempDatetime))
                            return (DateTime)value < tempDatetime;
                        else return false;
                    case "%": return (value ?? "").ToString().Trim().ToLower().Contains(propertySearchExpression.Value.Trim().ToLower());
                    default: return false;
                }

            }

            else
            {
                BinarySearchExpression<T> binarySearchExpression = expression as BinarySearchExpression<T>;
                switch (binarySearchExpression.BinaryOperator)
                {

                    case "||": return Match(data, binarySearchExpression.LeftSearch) || Match(data, binarySearchExpression.RightSearch);
                    case "&&": return Match(data, binarySearchExpression.LeftSearch) && Match(data, binarySearchExpression.RightSearch);
                    default: return false;
                }
            }
        }


        private Stack<object> searchStack = new Stack<object>();
        public static SearchExpression DeserializeSearchExpression(string searchExpresssion)
        {
            if (string.IsNullOrEmpty(searchExpresssion))
                return null;
            Stack<Token> result = GetExpressionTree(searchExpresssion);
            Stack<Token> tokenList = new Stack<Token>();
            foreach (Token t in result)
                tokenList.Push(t);
            SearchExpression expression = GetSearchExpression(tokenList);
            return expression;
        }

        static ExpressionType GetExpressionType(Token leftHand, Token rightHand, Token operatorToken)
        {
            if (operatorToken.TokenType == TokenType.AssignmentOperator)
            {
                if (leftHand.TokenType == TokenType.Propertystring && rightHand.TokenType == TokenType.Propertystring)
                    return ExpressionType.member;
            }
            else if (operatorToken.TokenType == TokenType.BinaryOperator)
            {
                // if ((leftHand.TokenType == TokenType.PropertyExpression || leftHand.TokenType == TokenType.BinaryExpression) && (rightHand.TokenType == TokenType.PropertyExpression || rightHand.TokenType == TokenType.BinaryExpression))
                if ((leftHand.TokenType == TokenType.Expresssion) && (rightHand.TokenType == TokenType.Expresssion))
                    return ExpressionType.binary;
            }
            return ExpressionType.error;
        }

       static  SearchExpression GetSearchExpression(Stack<Token> tokenList)
        {
            Stack<Token> output = new Stack<Token>();
            bool error = false;
            while ((tokenList.Count > 1 || tokenList.Count == 1 && tokenList.Peek().TokenType != TokenType.Expresssion))
            {
                Token token = tokenList.Pop();
                if (token.TokenType == TokenType.BinaryOperator || token.TokenType == TokenType.AssignmentOperator)
                {
                    Token rightHand = output.Pop();
                    Token leftHand = output.Pop();
                    ExpressionType expressionType = GetExpressionType(leftHand, rightHand, token);
                    if (expressionType == ExpressionType.member)
                    {
                        Token propertyExpressionToken = new Token();
                        PropertySearchExpression propertySearchExpression = new PropertySearchExpression();
                        propertySearchExpression.Property = leftHand.value as string;
                        propertySearchExpression.Operator = (string)token.value;
                        propertySearchExpression.Value = (string)rightHand.value;
                        propertyExpressionToken.value = propertySearchExpression;
                        propertyExpressionToken.TokenType = TokenType.Expresssion;
                        tokenList.Push(propertyExpressionToken);
                        while (output.Count > 0)
                            tokenList.Push(output.Pop());
                    }

                    else if (expressionType == ExpressionType.binary && leftHand.TokenType == TokenType.Expresssion && rightHand.TokenType == TokenType.Expresssion)
                    {
                        Token binaryExpressionToken = new Token();
                        BinarySearchExpression<T> binarySearchExpression = new BinarySearchExpression<T>();
                        binarySearchExpression.BinaryOperator = token.value as string;
                        binarySearchExpression.LeftSearch = leftHand.value as SearchExpression;
                        binarySearchExpression.RightSearch = rightHand.value as SearchExpression;
                        binaryExpressionToken.TokenType = TokenType.Expresssion;
                        binaryExpressionToken.value = binarySearchExpression;
                        tokenList.Push(binaryExpressionToken);
                        while (output.Count > 0)
                            tokenList.Push(output.Pop());
                    }

                    else
                    {
                        error = true;
                        break;
                    }


                }
                else
                    output.Push(token);
            }


            if (error || tokenList.Count > 1)
                return null;
            return tokenList.Pop().value as SearchExpression;

        }

       static  Stack<Token> GetExpressionTree(string expression)
        {
            Stack<Token> operatorsTokenStack = new Stack<Token>();
            Stack<Token> outputToken = new Stack<Token>();
            Token previousMatchProperty = new Token();
            previousMatchProperty.value = "";
            foreach (char c in expression)
            {
                if (string.IsNullOrWhiteSpace(c.ToString()))
                    continue;
                Token currentToken = Token.OperatorTokens.Find(ot => (string)ot.value == c.ToString());
                string previousMatchValue = previousMatchProperty.value as string;
                char previousMatchCharacter = string.IsNullOrEmpty(previousMatchValue) ? '\0' : (previousMatchValue)[previousMatchValue.Length - 1];
                currentToken = currentToken == null ? Token.OperatorTokens.Find(ot => (string)ot.value == previousMatchCharacter + c.ToString()) : currentToken;
                if (currentToken != null)
                {
                    previousMatchProperty.value = (currentToken.value as string).Length > 1 ? previousMatchValue.Substring(0, previousMatchValue.Length - 1) : previousMatchValue;
                    previousMatchProperty = new Token();
                    if (outputToken.Count() > 0 && string.IsNullOrEmpty(outputToken.Peek().value as string))
                        outputToken.Pop();

                    //abort parsing if previous match text before seeing a token isn't a valid text e.g an expression of this type "name.=tade" will abort after reading name. because name.= is not a valid text
                    // no need to checking for properties or values as a mere string, some properties for db object like mongo can be chained e.g nextOfKin.firstname
                    /*else if (outputToken.Count() > 0 && !(outputToken.Peek().value as string).IsValidTextToken())
                       break; */

                    if (currentToken.TokenType == TokenType.CloseBracket)
                    {
                        Token topToken = null;
                        while ((topToken = operatorsTokenStack.Pop()).TokenType != TokenType.OpenBracket)
                            outputToken.Push(topToken);
                    }

                    else
                    {
                        while (operatorsTokenStack.Count > 0 && (int)operatorsTokenStack.Peek().TokenType >= (int)currentToken.TokenType)
                        {
                            outputToken.Push(operatorsTokenStack.Pop());
                        }
                        operatorsTokenStack.Push(currentToken);
                    }


                }
                else
                {
                    if (string.IsNullOrEmpty((string)previousMatchProperty.value))
                    {
                        Token propertystringToken = previousMatchProperty;
                        outputToken.Push(propertystringToken);
                    }
                    previousMatchProperty.value = ((string)previousMatchProperty.value) + c;
                }


            }

            while (operatorsTokenStack.Count > 0)
                outputToken.Push(operatorsTokenStack.Pop());
            return outputToken;
        }



    }

    public abstract class SearchExpression
    {
        public abstract string SearchType { get; }
    }


   

    public class PropertySearchExpression:SearchExpression
    {
        public string Property{get;set;}
        public string Operator{get;set;}
        public string Value{get;set;}
        public override string SearchType
        {
            get
            {
                return "Property Search";
            }
        }
    }
    public class BinarySearchExpression<T>:SearchExpression
    {
        public SearchExpression LeftSearch { get; set; }
        public string BinaryOperator { get; set; }
        public SearchExpression RightSearch { get; set; }
        public override string SearchType
        {
            get { return "Binary Search"; }
        }
    }
}
