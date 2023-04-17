using System;
using System.Collections.Generic;

namespace Parser
{
    internal class Parser
    {
        public Parser(string expression) 
        {
            _lexer = new Lexer(expression);
        }

        public double Evaluate()
        {
            Stack<char> operatorStack = new Stack<char>();
            Stack<double> valueStack = new Stack<double>();

            ParserState state = ParserState.ExpectOperandOrUnary;

            while (!_lexer.IsEmpty())
            {
                IToken token = _lexer.GetToken();
                
                if (token is ValueToken valToken)
                {
                    HandleValueToken(valueStack, ref state, valToken);
                } 
                else if (token is OperatorToken operatorToken)
                {
                    HandleOperatorToken(operatorStack, valueStack, ref state, operatorToken);
                }
                else
                {
                    throw new Exception("Cannot evaluate an expression containing abstract identifiers");
                }
            }

            while (operatorStack.Count > 0)
            {
                if (operatorStack.Peek() == '(')
                    throw new Exception("Mismatched paranthesis");

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

        private static void HandleOperator(Stack<double> valueStack, char op)
        {
            if (op == 'n')
            {
                if (valueStack.Count < 1)
                    throw new Exception("Not enough operands for unary '-'");

                valueStack.Push(-valueStack.Pop());
            }
            else
            {
                if (valueStack.Count < 2)
                    throw new Exception("Not enough operands for '" + op + "'");

                double b = valueStack.Pop();
                double a = valueStack.Pop();

                switch (op)
                {
                    case '+': valueStack.Push(a + b); break;
                    case '-': valueStack.Push(a - b); break;
                    case '*': valueStack.Push(a * b); break;
                    case '/':
                        {
                            if (b == 0) throw new Exception("Division by zero");
                            
                            valueStack.Push(a / b); break;
                        }
                    default: throw new Exception("Invalid operator: '" + op + "'");
                }
            }
        }

        private static void HandleOperatorToken(Stack<char> operatorStack, Stack<double> valueStack, 
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

        private static void HandleValueToken(Stack<double> valueStack, ref ParserState state, ValueToken token)
        {
            if (state == ParserState.ExpectOperator)
                throw new Exception("Expected operator but found: '" + token.Value + "'");

            valueStack.Push(token.Value);
            state = ParserState.ExpectOperator;
        }

        private enum ParserState
        {
            ExpectOperand,
            ExpectOperandOrUnary,
            ExpectOperator,
        }

        private Lexer _lexer;
    }
}
