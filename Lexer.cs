using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Parser
{
    interface IToken {}

    struct ValueToken : IToken
    {
        public ValueToken(double v)
        {
            this.v = v;
        }

        public override string ToString()
        {
            return "Value: " + v.ToString();
        }

        public double v;
    }

    struct OperatorToken : IToken
    {
        public OperatorToken(char c)
        {
            op = c;
        }

        public override string ToString()
        {
            return "Operator: " + op;
        }

        public char op;
    }

    struct IdentifierToken : IToken
    {
        public IdentifierToken(string id)
        {
            identifier = id;
        }

        public override string ToString()
        {
            return "Identifier: " + identifier;
        }

        public string identifier;
    }

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

        public bool IsEmpty()
        {
            return currentPos >= expression.Length;
        }

        private IToken RetrieveNextToken(bool consume=false)
        {
            while (currentPos < expression.Length && expression[currentPos] == ' ')
                currentPos++;

            if (currentPos >= expression.Length)
                throw new Exception("End of expression reached");

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
                throw new Exception("Unrecognized symbol: " + expression[currentPos]);
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
