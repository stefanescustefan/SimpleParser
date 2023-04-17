using System;
using System.Collections.Generic;

namespace Parser
{
    interface ISyntaxNode { }

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

    struct SyntaxNodeValue: ISyntaxNode
    {
        public SyntaxNodeValue(double v)
        {
            Value = v;
        }

        public double Value;
    }

    struct SyntaxNodeBinaryOperator: ISyntaxNode
    {
        public SyntaxNodeBinaryOperator(BinaryOperator op, ISyntaxNode operand1, ISyntaxNode operand2)
        {
            Operator = op;
            Operand1 = operand1;
            Operand2 = operand2;
        }

        public BinaryOperator Operator;
        public ISyntaxNode Operand1;
        public ISyntaxNode Operand2;
    }

    struct SyntaxNodeUnaryOperator: ISyntaxNode
    {
        public SyntaxNodeUnaryOperator(UnaryOperator op, ISyntaxNode operand)
        {
            Operator = op;
            Operand = operand;
        }

        public UnaryOperator Operator;
        public ISyntaxNode Operand;
    }

    struct SyntaxNodeIdentifier: ISyntaxNode
    {
        public SyntaxNodeIdentifier(string name)
        {
            Name = name;
        }

        public string Name;
    }

    struct SyntaxNodeFunctionCall: ISyntaxNode
    {
        public SyntaxNodeFunctionCall(SyntaxNodeIdentifier functionIdentifier, List<ISyntaxNode> functionArguments)
        {
            FunctionIdentifier = functionIdentifier;
            FunctionArguments = functionArguments;
        }

        public SyntaxNodeIdentifier FunctionIdentifier;
        public List<ISyntaxNode> FunctionArguments;
    }

    class SyntaxTree
    {
        public static void PrintSyntaxTree(in ISyntaxNode root, int indent=0)
        {
            if (root is SyntaxNodeValue value)
            {
                Console.WriteLine("".PadLeft(indent) + value.Value);
            }
            else if (root is SyntaxNodeBinaryOperator binOp)
            {
                Console.WriteLine("".PadLeft(indent) + binOp.Operator);
                PrintSyntaxTree(binOp.Operand1, indent + 4);
                PrintSyntaxTree(binOp.Operand2, indent + 4);
            }
            else if (root is SyntaxNodeUnaryOperator unOp)
            {
                Console.WriteLine("".PadLeft(indent) + unOp.Operator);
                PrintSyntaxTree(unOp.Operand, indent + 4);
            }
            else if(root is SyntaxNodeIdentifier id)
            {
                Console.WriteLine("".PadLeft(indent) + id.Name);
            }
            else if(root is SyntaxNodeFunctionCall funcCall)
            {
                Console.WriteLine("".PadLeft(indent) + funcCall.FunctionIdentifier.Name);
                foreach (ISyntaxNode node in funcCall.FunctionArguments)
                {
                    PrintSyntaxTree(node, indent + 4);
                }
            }
        }
    }
    
}
