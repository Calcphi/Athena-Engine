using System;
using System.Collections.Generic;
using System.Text;

namespace Athena_Engine
{
    public class Node {
        public Types t; 
        public Flags f;
        public Node[] exp = new Node[2];
        public string var;
        public double value;
        public Operators op;
    }
}
