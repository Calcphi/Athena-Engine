using System;
using System.Collections.Generic;
using System.Text;

namespace Athena_Engine
{
    public class Simplifier
    {
        List<Func<Node, Node>> rules = new List<Func<Node, Node>>();
        int depth = 0;

        public Simplifier(){
            Func<Node, Node> r1 = FirstRule;
            rules.Add(r1);
        }

        public Node Simplify(Node origin)
        {
            
            return SimplifyRecursion(origin);
        }

        public Node SimplifyRecursion(Node n)
        {
            if(n.exp[0] == null || n.exp[1] == null) //If there are no more children stop the recursion right here
            {
                return n;
            }
            foreach (Func<Node, Node> func in rules) //Apply every rule in the current node.
            {
                n = func(n);
            }
            n.exp[0] = SimplifyRecursion(n.exp[0]); //Then do recursion for the left one
            n.exp[1] = SimplifyRecursion(n.exp[1]);// Recursion for the right one;
            return n;


        }

        private Node IncrementPriorityValue(Node n)
        {
            if (n.t == Types.Operator || (n.t == Types.Double && n.priority_value > 0))
            {
                n.priority_value++;
            } else if(!(n.exp[0] == null || n.exp[1] == null)) //check if there are children
            {
                n.exp[0] = IncrementPriorityValue(n.exp[0]);
                n.exp[1] = IncrementPriorityValue(n.exp[1]);
            }
            return n;
        }


        private Node FirstRule(Node n)
        {
            if(n.op != Operators.Subtraction)
            {
                return n;
            }
            n.op = Operators.Addition;
            Node old_r = n.exp[1]; //get the old right node
            n.exp[1] = new Node() { t = Types.Operator, op = Operators.Multiplication, priority_value = n.priority_value + 1 
            };
            old_r = IncrementPriorityValue(old_r);
            n.exp[1].exp[0] = new Node() { t = Types.Double, value = -1 };
            n.exp[1].exp[1] = old_r;
            return n;
        }

        private Node SecondRule(Node n)
        {
            return n;
        }

       
    }
}
