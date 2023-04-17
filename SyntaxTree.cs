using System.Collections.Generic;

namespace Parser
{
    interface SyntaxNode { }

    enum BinaryOperator
    {
        Plus,
        Minus,
        Times,
        Divide
    }

    enum UnaryOperator
    {
        Minus
    }

    struct SyntaxNodeValue: SyntaxNode
    {
        public SyntaxNodeValue(double v)
        {
            Value = v;
        }

        public double Value;
    }

    struct SyntaxNodeBinaryOperator: SyntaxNode
    {
        public SyntaxNodeBinaryOperator(BinaryOperator op, SyntaxNode operand1, SyntaxNode operand2)
        {
            Operator = op;
            Operand1 = operand1;
            Operand2 = operand2;
        }

        public BinaryOperator Operator;
        public SyntaxNode Operand1;
        public SyntaxNode Operand2;
    }

    struct SyntaxNodeUnaryOperator: SyntaxNode
    {
        public SyntaxNodeUnaryOperator(UnaryOperator op, SyntaxNode operand)
        {
            Operator = op;
            Operand = operand;
        }

        public UnaryOperator Operator;
        public SyntaxNode Operand;
    }

    struct SyntaxNodeIdentifier: SyntaxNode
    {
        public SyntaxNodeIdentifier(string id)
        {
            Identifier = id;
        }

        public string Identifier;
    }

    struct SyntaxItemFunctionCall: SyntaxNode
    {
        public SyntaxItemFunctionCall(SyntaxNodeIdentifier functionIdentifier, List<SyntaxNode> functionArguments)
        {
            FunctionIdentifier = functionIdentifier;
            FunctionArguments = functionArguments;
        }

        SyntaxNodeIdentifier FunctionIdentifier;
        List<SyntaxNode> FunctionArguments;
    }

}
