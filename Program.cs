using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Parser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                string input = Console.ReadLine();
                if (input == "q")
                    break;

                Lexer lexer = new Lexer(input);

                try
                {
                    ISyntaxNode root = Parser.BuildSyntaxTree(lexer);
                    SyntaxTree.PrintSyntaxTree(root);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            
        }
    }
}
