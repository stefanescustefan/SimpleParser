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

    internal class Lexer
    {
        public Lexer(string expression) {
            this.expression = expression.Trim();
            valueRegex = new Regex(@"\G\d+(\.\d*)?", RegexOptions.Compiled);

            currentPos = 0;
        }

        public IToken GetToken()
        {
            IToken nextToken;
            currentPos += RetrieveNextToken(out nextToken);

            return nextToken;
        }

        public IToken PeekToken()
        {
            IToken nextToken;
            RetrieveNextToken(out nextToken);

            return nextToken;
        }

        public bool IsEmpty()
        {
            return currentPos >= expression.Length;
        }

        private int RetrieveNextToken(out IToken nextToken)
        {
            while (currentPos < expression.Length && expression[currentPos] == ' ')
                currentPos++;

            if (currentPos >= expression.Length)
                throw new Exception("End of expression reached");

            if (operations.Contains(expression[currentPos]))
            {
                nextToken = new OperatorToken(expression[currentPos]);
                return 1;
            }
            else if (valueRegex.IsMatch(expression, currentPos))
            {
                Match match = valueRegex.Match(expression, currentPos);
                nextToken = new ValueToken(Double.Parse(match.Value));
                return match.Length;
            }
            else
            {
                throw new Exception("Unrecognized symbol: " + expression[currentPos]);
            }
        }

        private int currentPos;
        private string expression;

        private Regex valueRegex;
        private readonly char[] operations = { '+', '-', '*', '/', '(', ')' };
    }
}
