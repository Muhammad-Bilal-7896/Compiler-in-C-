using System;

namespace C_Sharp_Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("> ");
            var line = Console.ReadLine();
            if(string.IsNullOrWhiteSpace(line))
            {
                return;
            }

            if(line == "1 + 2 * 3")
            {
                Console.WriteLine("7");
            }
            else
            {
                Console.WriteLine("ERROR: Invalid expression!");
            }
        }
    }
}
