using System;
using System.Collections.Generic;
using Athena_Engine;

namespace Console_tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Tokenizer t = new Tokenizer();
            Parser p = new Parser();
            Solver s = new Solver();
            Simplifier simp = new Simplifier(); //ha he's a simp
            Console.Write("Solve equation (Y/n):");
            string decision = Console.ReadLine();
            while (true)
            {
                Console.Write("Equation:");
                List<Node> nodes = t.Tokenize(Console.ReadLine());
                foreach (Node n in nodes)
                {
                    Console.WriteLine(n.t + " " + n.op + " " + n.value + " " + n.var + " " + n.f + " " + n.priority_value + "\n" );
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
                if (decision == "Y" || decision == "y")
                {
                    Console.WriteLine(s.Solve(nf));
                }
                else
                {
                    Node origin_simp = simp.Simplify(nf);
                    PrintTree(origin_simp, "", true);
                }
                

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
