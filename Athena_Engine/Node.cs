using System;
using System.Collections.Generic;
using System.Text;

namespace Athena_Engine
{
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
    public class Node {
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
        public Types t; 
        public Flags f;
        public Node[] exp = new Node[2];
        public string var;
        public double value;
        public Operators op;
        public int priority_value;

        public static bool operator ==(Node n1, Node n2)
        {
            if(ReferenceEquals(null, n1)) //check if n1 is null
            {
                return ReferenceEquals(null, n2); //if n1 is null check if n2 is also null
            }
            else
            {
                if (ReferenceEquals(null, n2)) //check if n2 is null
                {
                    return false; //n1 is clearly not null then it's not equal
                }
                else
                {
                    //Now let's test equality instead of identity
                    bool t = n1.t == n2.t;
                    bool op = n1.op == n2.op;
                    bool var = n1.var == n2.var;
                    bool value = n1.value == n2.value;

                    bool left_side = n1.exp[0] == n2.exp[0];
                    bool right_side = n1.exp[1] == n2.exp[1];

                    return t && op && var && value  && left_side && right_side;

                }
            }
        }

        public static bool operator !=(Node obj1, Node obj2)
        {
            return !(obj1 == obj2); //this is the opposite of equality, this saves time implementing the opposite
        }
    }
}
