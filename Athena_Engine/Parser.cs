using System;
using System.Collections.Generic;
using System.Text;

namespace Athena_Engine
{
    public class Parser
    {

        public Node Parse(List<Node> nodes)
        {
            if (nodes.Count == 1) {
                return nodes[0];
            }
            List<Node> operator_list = GetOperationList(nodes);
            //Now lets find the lowest priority operator
            Node first_node = GetLowestPriorityNode(operator_list);
            //now let's find the index to split the both lists
            (List<Node> node_list_left, List<Node> node_list_right) = SplitNodeList(nodes, first_node);

            first_node.exp[0] = ParseRecursion(node_list_left);
            first_node.exp[1] = ParseRecursion(node_list_right);

            return first_node;
        }

        public Node ParseRecursion(List<Node> nodes)
        {
            if (nodes.Count == 1) {
                return nodes[0];
            }
            List<Node> operator_list = GetOperationList(nodes);
            //Now lets find the lowest priority operator
            Node first_node = GetLowestPriorityNode(operator_list);
            //now let's find the index to split the both lists
            (List<Node> node_list_left, List<Node> node_list_right) = SplitNodeList(nodes, first_node);

            first_node.exp[0] = ParseRecursion(node_list_left);
            first_node.exp[1] = ParseRecursion(node_list_right);

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
            //if both have a priority level but they are different
            if (n1.f == Flags.Priority && n2.f == Flags.Priority && n1.priority_value > n2.priority_value)
            {
                newlist.Remove(n1);
            }
            else if (n1.f == Flags.Priority && n2.f == Flags.Priority && n1.priority_value < n2.priority_value)
            {
                newlist.Remove(n2);
            }
            else if (n1.f == Flags.Priority && n2.f == Flags.None) //conditions if one has a priority flag and the other doesn't
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
        public List<Node> GetOperationList(List<Node> nodes)
        {
            List<Node> operator_list = new List<Node>(); //list to store the operators
            for (int i = 0; i <= (nodes.Count - 1); i++)//append all operators 
            {
                if (nodes[i].t == Types.Operator)
                {
                    operator_list.Add(nodes[i]);
                }
            }
            return operator_list;
        }

        public (List<Node> list_split1, List<Node> list_split2) SplitNodeList(List<Node> nodes, Node first_node)
        {
            //First, let's find the low priority index
            int low_priority_index = 0;
            for (int i = 0; i <= (nodes.Count - 1); i++)
            {
                if (first_node == nodes[i])
                {
                    low_priority_index = i;
                }
            }
            //Now we split them
            List<Node> node_split1 = new List<Node>();
            List<Node> node_split2 = new List<Node>();
            for (int i = 0; i <= (nodes.Count - 1); i++)
            {
                if (i < low_priority_index)
                {
                    node_split1.Add(nodes[i]);
                }
                if (i > low_priority_index)
                {
                    node_split2.Add(nodes[i]);
                }
            }
            return (node_split1, node_split2);
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
                case Operators.Exponent:
                    priority = 3;
                    break;
            }
            return priority;
        }
    }
}
