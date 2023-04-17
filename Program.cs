using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Parser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string input = Console.ReadLine();
            Lexer lexer = new Lexer(input);
            while(!lexer.IsEmpty())
            {
                Console.WriteLine(lexer.GetToken());
            }

            Console.ReadKey();
        }
    }
}
