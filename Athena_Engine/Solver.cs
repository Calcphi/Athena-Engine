using System;
using System.Collections.Generic;
using System.Text;

namespace Athena_Engine
{
    public class Solver
    {
        private Parser p = new Parser(); 
        //Initialize parser to get some useful functions from it, it's kind of spaghetti but it makes the code more readable

        public double Solve(Node first_node)
        {
            return Solver_Recursion(first_node);
        }

        private double Solver_Recursion(Node node)
        {
            if (node.t != Types.Operator)
            {
                return node.value;
            }
            Node n1 = node.exp[0];
            Node n2 = node.exp[1];
            if(n1.t != Types.Operator && n2.t != Types.Operator) //if both children of node are different than we can finally solve it
            {
                return Solve_Node(node.op, n1.value, n2.value);
            }
            //Now we need to decide which one to continue to look for
            Node nf = Decider(n1, n2);
            //Recursion if one of them if an operator
            if (n1 == nf && n2.t != Types.Operator)
            {
                double n1_cvalue = Solver_Recursion(n1);
                return Solve_Node(node.op, n1_cvalue, n2.value);
            } else if (n2 == nf && n1.t != Types.Operator)
            {
                double n2_cvalue = Solver_Recursion(n2);
                return Solve_Node(node.op, n1.value, n2_cvalue);
            } 
            double n1_fvalue = Solver_Recursion(n1);
            double n2_fvalue = Solver_Recursion(n2);
            return Solve_Node(node.op, n2_fvalue, n2_fvalue);


        }

        private double Solve_Node(Operators o, double n1, double n2)
        {
            double result = 0;
            switch (o)
            {
                case Operators.Addition:
                    result = n1 + n2;
                    break;
                case Operators.Division:
                    if (n2 == 0)
                    {
                        throw new DivideByZeroException("Stop dividing by zero, bitch.");
                    }
                    result = n1 / n2;
                    break;
                case Operators.Multiplication:
                    result = n1 * n2;
                    break;
                case Operators.Subtraction:
                    result = n1 - n2;
                    break;
            }
            return result;       
        }

        private Node Decider(Node n1, Node n2) //this will decide which operation node to use first
        {
            //This should ONLY be used when there is at least one operator in one of the two nodes.
            //Check if at least one of them is a operator
            if(!(n1.t == Types.Operator || n2.t == Types.Operator))
            {
                throw new ArgumentException("None of the two nodes are an operator");
            }
            //Check the types first, to be more economical
            if(n1.t == Types.Operator && n2.t != Types.Operator)
            {
                return n1;
            } else if (n1.t != Types.Operator && n2.t == Types.Operator)
            {
                return n2;
            }
            //Now we will check the priority values and operator values.
            else if (n1.priority_value > n2.priority_value)
            {
                return n1;
            }
            else if (n1.priority_value < n2.priority_value)
            {
                return n2;
            }
            int n1prio = p.GetOperationPriority(n1);
            int n2prio = p.GetOperationPriority(n2);
            if(n1prio == n2prio)
            {                       //default to the left
                return n1;
            }
            else if(n1prio > n2prio) 
            {
                return n1;
            }
            return n2;
        }
    }
}
