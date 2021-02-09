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
                Console.Write(OrganizeTree(nf));
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
        
     
        public static string OrganizeTree(Node n) //O Diogo é Rei e fez isto sozinho
        {
            string parcel = "";
            switch (n.t)
            {
                case Types.Double:
                    parcel = Convert.ToString(n.value);

                    break;
                case Types.Operator:

                    switch (n.op)
                    {

                        case Operators.Addition:

                            parcel = "+";
                            break;
                        case Operators.Subtraction:

                            parcel = "-";
                            break;
                        case Operators.Multiplication:

                            parcel = "*";
                            break;
                        case Operators.Division:

                            parcel = "/";
                            break;
                        case Operators.Exponent:

                            parcel = "^";
                            break;

                    }


                    break;

                case Types.Variable:
                    parcel = Convert.ToString(n.var);

                    break;






            }

            if (n.exp[0] == null && n.exp[1] == null)
            {

                return parcel;

            }


            string ParcelLeft = "";
            string ParcelRight = "";

            ParcelLeft = OrganizeTree(n.exp[0]);
            ParcelRight = OrganizeTree(n.exp[1]);



            string Equation = ParcelLeft + parcel + ParcelRight;

            return Equation;
        }
    }
}
