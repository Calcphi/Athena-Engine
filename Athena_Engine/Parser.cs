using System;
using System.Collections.Generic;
using System.Text;

namespace Athena_Engine
{
    public class Parser
    {

        public Node Parse(List<Node> nodes)
        {
            List<Node> operator_list = new List<Node>(); //list to store the operators
            List <int> operator_index = new List<int>(); //list to store the operators index
            for (int i = 0; i <= (nodes.Count - 1); i++)//append all operators 
            {
                if (nodes[i].t == Types.Operator)
                {
                    operator_list.Add(nodes[i]);
                }
            }
            //Now lets find the lowest priority operator
            int low_priority_index = 0;
            Node first_node = GetLowestPriorityNode(operator_list);
            return first_node;
        }

        public Node GetLowestPriorityNode(List<Node> node_list)
        {
            List<Node> newlist = node_list;
            Node n1 = node_list[0];
            int op1 = GetOperationPriority(n1);
            if (node_list.Count == 1)
            {
                return node_list[0];
            }
            Node n2 = node_list[1];
            int op2 = GetOperationPriority(n2);

            if (n1.f == Flags.Priority && n2.f == Flags.None) //conditions if one has a priority flag and the other doesn't
            {
                newlist.Remove(n1);
            }
            else if (n2.f == Flags.Priority && n1.f == Flags.None)
            {
                newlist.Remove(n2);
            }
            else if (n1.op == n2.op) //if they are the same type remove the left one
            {
                newlist.Remove(n1);
            }
            else if (op1 == op2) //if they have the same weight remove the one on the left
            {
                newlist.Remove(n1);
            }
            else if (op1 > op2) //remove if the left operator has more weight
            {
                newlist.Remove(n1);
            }
            else if (op1 < op2) //remove if the right operator has more weight
            {
                newlist.Remove(n2);
            }
            return GetLowestPriorityNode(newlist);
        }

        public int GetOperationPriority(Node n)
        {
            int priority = 0;
            switch (n.op)
            {
                case Operators.Addition:
                    priority = 1;
                    break;

                case Operators.Subtraction:
                    priority = 1;
                    break;

                case Operators.Multiplication:
                    priority = 2;
                    break;

                case Operators.Division:
                    priority = 2;
                    break;
            }
            return priority;
        }
    }
}
