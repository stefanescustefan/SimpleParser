using System;
using System.Collections.Generic;

namespace Parser
{
    internal class Parser
    {
        public static ISyntaxNode BuildSyntaxTree(Lexer lexer)
        {
            Stack<char> operatorStack = new Stack<char>();
            Stack<ISyntaxNode> valueStack = new Stack<ISyntaxNode>();

            ParserState state = ParserState.ExpectOperandOrUnary;

            while (!lexer.IsEmpty())
            {
                IToken token = lexer.GetToken();
                
                if (token is ValueToken valToken)
                {
                    HandleValueToken(valueStack, ref state, valToken);
                } 
                else if (token is OperatorToken operatorToken)
                {
                    HandleOperatorToken(operatorStack, valueStack, ref state, operatorToken);
                }
                else if (token is IdentifierToken identifierToken)
                {
                    HandleIdentifierToken(valueStack, ref state, identifierToken);
                }
            }

            while (operatorStack.Count > 0)
            {
                if (operatorStack.Peek() == '(')
                    throw new Exception("Too many left parantheses");

                HandleOperator(valueStack, operatorStack.Pop());
            }

            return valueStack.Peek();
        }

        private static int Precedence(char op)
        {
            switch (op) 
            {
                case '+': case '-': return 0;
                case '*': case '/': return 1;
                case 'n': return 2;
                default: throw new Exception("Invalid operator passed to Precedence(): " + op);
            }
        }

        private static void HandleOperator(Stack<ISyntaxNode> valueStack, char op)
        {
            if (op == 'n')
            {
                if (valueStack.Count < 1)
                    throw new Exception("Not enough operands for unary '-'");

                valueStack.Push(new SyntaxNodeUnaryOperator(UnaryOperator.Minus, valueStack.Pop()));
            }
            else
            {
                if (valueStack.Count < 2)
                    throw new Exception("Not enough operands for '" + op + "'");

                ISyntaxNode b = valueStack.Pop();
                ISyntaxNode a = valueStack.Pop();

                BinaryOperator binaryOp;
                switch (op)
                {
                    case '+': binaryOp = BinaryOperator.Plus; break;
                    case '-': binaryOp = BinaryOperator.Minus; break;
                    case '*': binaryOp = BinaryOperator.Times; break;
                    case '/': binaryOp = BinaryOperator.Divide; break;
                    default: throw new Exception("Invalid operator: '" + op + "'");
                }

                valueStack.Push(new SyntaxNodeBinaryOperator(binaryOp, a, b));
            }
        }

        private static void HandleOperatorToken(Stack<char> operatorStack, Stack<ISyntaxNode> valueStack, 
            ref ParserState state, OperatorToken token)
        {
            if (token.Operator == '(')
            {
                if (state == ParserState.ExpectOperator)
                    throw new Exception("Expected operator but found: '('");

                operatorStack.Push(token.Operator);
                state = ParserState.ExpectOperandOrUnary;
            }
            else if (token.Operator == ')')
            {
                if (state != ParserState.ExpectOperator)
                    throw new Exception("Expected operand but found: ')'");

                while (operatorStack.Count > 0 && operatorStack.Peek() != '(')
                {
                    HandleOperator(valueStack, operatorStack.Pop());
                }

                if (operatorStack.Count == 0 || operatorStack.Peek() != '(')
                    throw new Exception("Too many right parantheses");
                else
                    operatorStack.Pop();
            }
            else if (token.Operator == '-' && state == ParserState.ExpectOperandOrUnary)
            {
                operatorStack.Push('n');
                state = ParserState.ExpectOperand;
            }
            else
            {
                if (state != ParserState.ExpectOperator)
                    throw new Exception("Expected operand but found: '" + token.Operator + "'");

                while (operatorStack.Count > 0 && operatorStack.Peek() != '(' &&
                Precedence(operatorStack.Peek()) >= Precedence(token.Operator))
                {
                    HandleOperator(valueStack, operatorStack.Pop());
                }

                operatorStack.Push(token.Operator);
                state = ParserState.ExpectOperandOrUnary;
            }
        }

        private static void HandleValueToken(Stack<ISyntaxNode> valueStack, ref ParserState state, ValueToken token)
        {
            if (state == ParserState.ExpectOperator)
                throw new Exception("Expected operator but found: '" + token.Value + "'");

            valueStack.Push(new SyntaxNodeValue(token.Value));
            state = ParserState.ExpectOperator;
        }

        private static void HandleIdentifierToken(Stack<ISyntaxNode> valueStack, ref ParserState state, IdentifierToken token)
        {
            if (state == ParserState.ExpectOperator)
                throw new Exception("Expected operator but found: '" + token.Identifier + "'");

            valueStack.Push(new SyntaxNodeIdentifier(token.Identifier));
            state = ParserState.ExpectOperator;
        }

        private enum ParserState
        {
            ExpectOperand,
            ExpectOperandOrUnary,
            ExpectOperator,
        }
    }
}
