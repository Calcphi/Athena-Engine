using System;
using System.Collections.Generic;
using System.Text;

namespace Athena_Engine
{
    public class Simplifier
    {
        List<Func<Node, Node>> rules = new List<Func<Node, Node>>();

        public Simplifier(){
            Func<Node, Node> r1 = FirstRule;
            rules.Add(r1);
            Func<Node, Node> r2 = SecondRule;
            rules.Add(r2);
            Func<Node, Node> r3 = ThirdRule;
            rules.Add(r3);
        }

        public Node Simplify(Node origin)
        {
            Node old_simplify = SimplifyRecursion(origin);
            while (true)
            {
                Node new_simplify = SimplifyRecursion(old_simplify);
                old_simplify = new_simplify;
                if (new_simplify == old_simplify)
                {
                    break;
                }
            }
            return old_simplify;
        }

        public Node SimplifyRecursion(Node n)
        {
            if(n.exp[0] == null || n.exp[1] == null) //If there are no more children stop the recursion right here
            {
                return n;
            }
            n.exp[0] = SimplifyRecursion(n.exp[0]); //Then do recursion for the left one
            n.exp[1] = SimplifyRecursion(n.exp[1]);// Recursion for the right one;
            foreach (Func<Node, Node> func in rules) //Apply every rule in the current node.
            {
                n = func(n);
            }
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

        private Node DecrementPriorityValue(Node n)
        {
            if (n.t == Types.Operator || (n.t == Types.Double && n.priority_value > 0))
            {
                n.priority_value--;
            }
            else if (!(n.exp[0] == null || n.exp[1] == null)) //check if there are children
            {
                n.exp[0] = DecrementPriorityValue(n.exp[0]);
                n.exp[1] = DecrementPriorityValue(n.exp[1]);
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
            if (n.exp[0] == null || n.exp[1] == null) //If there are no more children dont apply it.
            {
                return n;
            }
            if (n.op == Operators.Multiplication && n.exp[1].op == Operators.Division) //check if current node is an multiplication and second operand is a division 
            {
                //Check the image of the second rule at https://github.com/Calcphi/Athena-Engine/issues/7
                //to understand it better
                Node left_mp = n.exp[0];
                left_mp = IncrementPriorityValue(left_mp);

                Node right_mp = n.exp[1].exp[0];
                right_mp = IncrementPriorityValue(right_mp);

                Node right_div = n.exp[1].exp[1];
                right_div = DecrementPriorityValue(right_div);

                n.op = Operators.Division; //change current operator
                //The numerator will be a multiplication
                n.exp[0] = new Node() { t = Types.Operator, op = Operators.Multiplication, priority_value = n.priority_value + 1 };
                n.exp[0].exp[0] = left_mp;
                n.exp[0].exp[1] = right_mp;

                //Now we replace the denominator
                n.exp[1] = right_div;

            }
            return n;
        }

        private Node ThirdRule(Node n)
        {
            if (n.op == Operators.Division && n.exp[0].op == Operators.Division)
            {
                //Check the image of the third rule at https://github.com/Calcphi/Athena-Engine/issues/7

                Node div_num = DecrementPriorityValue(n.exp[0].exp[0]);

                Node div_deno1 = n.exp[0].exp[1];
                Node div_deno2 = IncrementPriorityValue(n.exp[1]);

                //After getting all the node numerators and denominators, we apply the third rule

                n.exp[0] = div_num; //replace by the numerator

                n.exp[1] = new Node() { t = Types.Operator, op = Operators.Multiplication, priority_value = n.priority_value + 1 };

                n.exp[1].exp[0] = div_deno1; //replace the denominator
                n.exp[1].exp[1] = div_deno2; //replace the denominator
                
                
            }
            return n;
        }

       
    }
}
