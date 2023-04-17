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

                Parser parser = new Parser(input);
                try
                {
                    Console.WriteLine("=" + parser.Evaluate());
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            
        }
    }
}
