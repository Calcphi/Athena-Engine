using System;
using System.Collections.Generic;
using System.Text;

namespace Athena_Engine
{
    public class Derivator
    {
        List<Func<Node, Node>> func_list = new List<Func<Node, Node>>(); //this ones are only expected to have one argument in them

        Simplifier simp = new Simplifier(); //ha got'em

        public Derivator()
        {
            //this should be in priority order
            Func<Node, Node> basic_rule = BasicRuleOfDerivation;
            func_list.Add(basic_rule);
        }

        public Node Derivate(Node n)
        {
            //Before derivating the node must have the derivate flag in order to the rules work

            n = simp.Simplify(n); //first we simplyfy this to the maximum
            foreach(Func<Node, Node> func in func_list) //execute every rule
            {
                n = func(n);
            }
            return simp.Simplify(n);

        }

        private Node BasicRuleOfDerivation(Node n)
        {
            //this rule is the most basic one and probably the most important one
            //this rule only applies if there are NO multiplications, divions and additions (substractions are handled by the simplifier).
            //there are a few cases tho: the first one if it is a number, a variable or a exponent with a variable
            

            //This should ONLY be executed if the Node has a flag for derivation
            if(n.f == Flags.Derivate)
            {
                n.f = Flags.None;
                if(n.t == Types.Double)
                {
                    return new Node() { t = Types.Double, value = 0, priority_value = n.priority_value }; //the derivative of a constant is always a constant

                }
                if(n.t == Types.Variable) //the derivate of variable with degree 1 is always 1
                {
                    return new Node() { t = Types.Double, value = 1, priority_value = n.priority_value };
                }
                if(n.op == Operators.Exponent)//this applies to any expression that has an exponent
                {
                   //this MUST be raised to a double not to an expression or variable
                   if(n.exp[1].t == Types.Double)
                    {
                        Node exponent_exponent = n.exp[1];
                        Node base_exponent = n.exp[0];
                        Node base_exponent_derivate = n.exp[0];
                        base_exponent_derivate.f = Flags.Derivate;

                        //this will result in a expression like this exponent*(base^exponent-1)*d(base)
                        n = new Node { t = Types.Operator, op = Operators.Multiplication, priority_value = n.priority_value };

                        n.exp[0] = exponent_exponent;
                        n.exp[1] = new Node() { t = Types.Operator, op = Operators.Multiplication, priority_value = n.priority_value };
                        
                        n.exp[1].exp[0] = new Node() { t = Types.Operator, op = Operators.Exponent, priority_value = n.priority_value + 1 };
                        n.exp[1].exp[0].exp[0] = base_exponent;
                        n.exp[1].exp[0].exp[1] = new Node() { t = Types.Double, value = exponent_exponent.value - 1 };
                        
                        n.exp[1].exp[1] = Derivate(base_exponent_derivate);
                        
                    }
                }
            }
            return n;
        }
    }
}

