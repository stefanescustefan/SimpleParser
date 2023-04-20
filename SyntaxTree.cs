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

    readonly record struct SyntaxNodeValue(double Value): ISyntaxNode;
    readonly record struct SyntaxNodeBinaryOperator(BinaryOperator Operator, ISyntaxNode Operand1, ISyntaxNode Operand2) : ISyntaxNode;

    readonly record struct SyntaxNodeUnaryOperator(UnaryOperator Operator, ISyntaxNode Operand): ISyntaxNode;

    readonly record struct SyntaxNodeIdentifier(string Name): ISyntaxNode;

    readonly record struct SyntaxNodeFunctionCall(SyntaxNodeIdentifier FunctionIdentifier, List<ISyntaxNode> FunctionArguments): ISyntaxNode;

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
