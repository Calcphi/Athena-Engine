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
                Solver s = new Solver();
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
                PrintTree(nf, "", true);
                Console.WriteLine("\n");
                Console.WriteLine("\n");
                Console.WriteLine("\n");
                Console.WriteLine(s.Solve(nf));

            }


        }
        public static void PrintTree(Node tree, String indent, bool last)
        {
            if(tree == null)
            {
                return;
            }
            Console.WriteLine(indent + "+- "+ tree.t + " " + tree.op + " " + tree.value + " " + tree.var + " " + tree.f + "\n");
            indent += last ? "   " : "|  ";

            for (int i = 0; i <= (tree.exp.Length - 1); i++)
            {
                PrintTree(tree.exp[i], indent, i == (tree.exp.Length - 1));
            }
        }
    }
}
