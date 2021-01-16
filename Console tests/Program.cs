using System;
using System.Collections.Generic;
using Athena_Engine;

namespace Console_tests
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Equation:");
                Tokenizer t = new Tokenizer();
                List<Node> nodes = t.Tokenize(Console.ReadLine());
                foreach (Node n in nodes)
                {
                    Console.WriteLine(n.t + " " + n.op + " " + n.value + " " + n.var + "\n" );
                }
                Console.WriteLine("\n");
            }


        }
    }
}
