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
            List<Node> nodes = t.Tokenize("Ola+1");
            foreach (Node n in nodes)
            {
                Console.WriteLine(n.t);
            }

        }
    }
}
