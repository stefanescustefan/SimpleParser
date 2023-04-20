using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Parser
{
    interface IToken { }
    readonly record struct ValueToken(double Value) : IToken;
    readonly record struct OperatorToken(char Operator) : IToken;
    readonly record struct IdentifierToken(string Identifier) : IToken;

    internal class Lexer
    {
        public Lexer(string expression) {
            this.expression = expression.Trim();
            valueRegex = new Regex(@"\G\d+(\.\d*)?", RegexOptions.Compiled);
            identifierRegex = new Regex(@"\G[A-Za-z]+([A-Za-z]|\d)*", RegexOptions.Compiled);

            currentPos = 0;
        }

        public IToken GetToken()
        {
            return RetrieveNextToken(true);
        }

        public IToken PeekToken()
        {
            return RetrieveNextToken();
        }

        public void Reset()
        {
            currentPos = 0;
        }

        public bool IsEmpty()
        {
            return currentPos >= expression.Length;
        }

        private IToken RetrieveNextToken(bool consume=false)
        {
            while (currentPos < expression.Length && expression[currentPos] == ' ')
                currentPos++;

            if (currentPos >= expression.Length)
                throw new Exception("Lexer: End reached when trying to retrieve token.");

            IToken nextToken;
            if (operations.Contains(expression[currentPos]))
            {
                nextToken = new OperatorToken(expression[currentPos]);
                if (consume)
                    currentPos++;
            }
            else if (valueRegex.IsMatch(expression, currentPos))
            {
                Match match = valueRegex.Match(expression, currentPos);
                nextToken = new ValueToken(Double.Parse(match.Value));

                if (consume)
                    currentPos += match.Length;
            }
            else if (identifierRegex.IsMatch(expression, currentPos))
            {
                Match match = identifierRegex.Match(expression, currentPos);
                nextToken = new IdentifierToken(match.Value);

                if (consume)
                    currentPos += match.Length;
            }
            else
            {
                throw new Exception("Lexer: unrecognized symbol: " + expression[currentPos]);
            }

            return nextToken;
        }

        private int currentPos;
        private string expression;

        private Regex valueRegex;
        private Regex identifierRegex;
        private readonly char[] operations = { '+', '-', '*', '/', '(', ')' };
    }
}
