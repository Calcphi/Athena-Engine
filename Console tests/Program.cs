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
                Parser p = new Parser();
                List<Node> nodes = t.Tokenize(Console.ReadLine());
                foreach (Node n in nodes)
                {
                    Console.WriteLine(n.t + " " + n.op + " " + n.value + " " + n.var + " " + n.f + "\n" );
                }
                Console.WriteLine("\n");
                Console.WriteLine("\n");
                Console.WriteLine("\n");
                Console.WriteLine("\n");
                Node nf = p.Parse(nodes);
                Console.WriteLine(nf.t + " " + nf.op + " " + nf.value + " " + nf.var + " " + nf.f + "\n");
                Console.WriteLine("\n");
            }


        }
    }
}
