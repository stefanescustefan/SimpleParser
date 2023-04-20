using Parser;
using System;

while (true)
{
    string input = Console.ReadLine();
    if (input == "q")
        break;

    Lexer lexer = new Lexer(input);

    try
    {
        ISyntaxNode root = Parser.Parser.BuildSyntaxTree(lexer);
        SyntaxTree.PrintSyntaxTree(root);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
}
