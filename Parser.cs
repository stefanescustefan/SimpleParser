using System;
using System.Collections.Generic;

namespace Parser
{
    internal class Parser
    {
        private enum OperatorType
        {
            Plus,
            Minus,
            Times,
            Divide,

            UnaryMinus,

            LeftParanthesis,
            RightParanthesis,

            FuncName
        }
        private static OperatorType GetOperatorType(char op)
        {
            return op switch
            {
                '+' => OperatorType.Plus,
                '-' => OperatorType.Minus,
                '*' => OperatorType.Times,
                '/' => OperatorType.Divide,
                '(' => OperatorType.LeftParanthesis,
                ')' => OperatorType.RightParanthesis,
                _ => throw new ArgumentException("Invalid operator: " + op)
            };
        }

        private readonly record struct Operator
        {
            public Operator(OperatorType type)
            {
                if (type == OperatorType.FuncName)
                    throw new ArgumentException("A function operator must have a name!");

                Type = type;
                FuncName = null;
            }

            public Operator(String identifier)
            {
                Type = OperatorType.FuncName;
                FuncName = identifier;
            }

            public string GetFuncName()
            {
                if (Type != OperatorType.FuncName)
                    throw new FieldAccessException("Only function operators have names!");

                return FuncName;
            }

            readonly public OperatorType Type;
            readonly private string FuncName;
        }
        
        public static ISyntaxNode BuildSyntaxTree(Lexer lexer)
        {
            Stack<Operator> operatorStack = new Stack<Operator>();
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
                if (operatorStack.Peek().Type == OperatorType.LeftParanthesis)
                    throw new Exception("Parser: Too many left parantheses");

                HandleOperator(valueStack, operatorStack.Pop());
            }

            return valueStack.Peek();
        }

        private static int Precedence(OperatorType opType)
        {
            switch (opType) 
            {
                case OperatorType.Plus: case OperatorType.Minus: return 0;
                case OperatorType.Times: case OperatorType.Divide: return 1;
                case OperatorType.UnaryMinus: return 2;
                default: throw new ArgumentException("Parser: Invalid operator type passed to Precedence(): " + opType);
            }
        }

        private static void HandleOperator(Stack<ISyntaxNode> valueStack, Operator op)
        {
            if (op.Type == OperatorType.UnaryMinus)
            {
                if (valueStack.Count < 1)
                    throw new Exception("Parser: Not enough operands for unary '-'");

                valueStack.Push(new SyntaxNodeUnaryOperator(UnaryOperator.Minus, valueStack.Pop()));
            }
            else
            {
                if (valueStack.Count < 2)
                    throw new Exception("Parser: Not enough operands for '" + op + "'");

                ISyntaxNode b = valueStack.Pop();
                ISyntaxNode a = valueStack.Pop();

                BinaryOperator binaryOp;
                switch (op.Type)
                {
                    case OperatorType.Plus: binaryOp = BinaryOperator.Plus; break;
                    case OperatorType.Minus: binaryOp = BinaryOperator.Minus; break;
                    case OperatorType.Times: binaryOp = BinaryOperator.Times; break;
                    case OperatorType.Divide: binaryOp = BinaryOperator.Divide; break;
                    default: throw new Exception("Parser: Invalid operator: '" + op + "'");
                }

                valueStack.Push(new SyntaxNodeBinaryOperator(binaryOp, a, b));
            }
        }

        private static void HandleOperatorToken(Stack<Operator> operatorStack, Stack<ISyntaxNode> valueStack, 
            ref ParserState state, OperatorToken token)
        {
            if (token.Operator == '(')
            {
                if (state == ParserState.ExpectOperator)
                    throw new Exception("Parser: Expected operator but found: '('");

                operatorStack.Push(new Operator(OperatorType.LeftParanthesis));
                state = ParserState.ExpectOperandOrUnary;
            }
            else if (token.Operator == ')')
            {
                if (state != ParserState.ExpectOperator)
                    throw new Exception("Parser: Expected operand but found: ')'");

                while (operatorStack.Count > 0 && operatorStack.Peek().Type != OperatorType.LeftParanthesis)
                {
                    HandleOperator(valueStack, operatorStack.Pop());
                }

                if (operatorStack.Count == 0 || operatorStack.Peek().Type != OperatorType.LeftParanthesis)
                    throw new Exception("Parser: Too many right parantheses");
                else
                    operatorStack.Pop();
            }
            else if (token.Operator == '-' && state == ParserState.ExpectOperandOrUnary)
            {
                operatorStack.Push(new Operator(OperatorType.UnaryMinus));
                state = ParserState.ExpectOperand;
            }
            else
            {
                if (state != ParserState.ExpectOperator)
                    throw new Exception("Parser: Expected operand but found: '" + token.Operator + "'");

                while (operatorStack.Count > 0 && operatorStack.Peek().Type != OperatorType.LeftParanthesis &&
                Precedence(operatorStack.Peek().Type) >= Precedence(GetOperatorType(token.Operator)))
                {
                    HandleOperator(valueStack, operatorStack.Pop());
                }

                operatorStack.Push(new Operator(GetOperatorType(token.Operator)));
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
                throw new Exception("Parser: Expected operator but found: '" + token.Identifier + "'");

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
