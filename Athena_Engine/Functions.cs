using System;
using System.Collections.Generic;
using System.Text;

namespace Athena_Engine
{
    class Functions
    {
        public List<(string, Func<string, int, string>)> functions = new List<(string, Func<string, int, string>)>();

        public Functions()
        {
            Func<string, int, string> r = RootFunction;
            functions.Add(("root", r));
        }

        public static string RootFunction(string input, int index)
        {
            return input;
        }

    }
}
