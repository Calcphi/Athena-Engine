using System;
using System.Collections.Generic;
using System.Text;

namespace Athena_Engine
{
    public class Simplifier
    {
        List<Func<Node, Node, Node>> rules = new List<Func<Node, Node, Node>>();

        public Simplifier(){
            Func<Node, Node, Node> r1 = FirstRule;
            rules.Add(r1);
            Func<Node, Node, Node> r2 = SecondRule;
            rules.Add(r2);
            Func<Node, Node, Node> r3 = ThirdRule;
            rules.Add(r3);
            Func<Node, Node, Node> r4 = ForthRule;
            rules.Add(r4);
            Func<Node, Node, Node> r45 = ForthHalfRule;
            rules.Add(r45);
            Func<Node, Node, Node> r5 = FifthRule;
            rules.Add(r5);
        }

        public Node Simplify(Node origin)
        {
            Node old_simplify = SimplifyRecursion(origin, origin);
            while (true)
            {
                Node new_simplify = SimplifyRecursion(old_simplify, old_simplify);
                old_simplify = new_simplify;
                if (new_simplify == old_simplify)
                {
                    break;
                }
            }
            return old_simplify;
        }

        public Node SimplifyRecursion(Node n, Node prev_n)
        {
            if (n == null)
            {
                return n;
            }
            try
            {
                n.exp[0] = SimplifyRecursion(n.exp[0], n); //Then do recursion for the left one
                n.exp[1] = SimplifyRecursion(n.exp[1], n);// Recursion for the right one;
            }
            catch (NullReferenceException) { }
            
            foreach (Func<Node, Node, Node> func in rules) //Apply every rule in the current node.
            {
                n = func(n, prev_n);
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

        private Node FirstRule(Node n, Node prev_n)
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

        private Node SecondRule(Node n, Node prev_n)
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

        private Node ThirdRule(Node n, Node prev_n)
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

        private Node ForthRule(Node n, Node prev_n)
        {
            if (n.t == Types.Variable && (prev_n.t == Types.Operator && prev_n.op != Operators.Multiplication || (prev_n.op == Operators.Multiplication && !(prev_n.exp[0].t == Types.Double || prev_n.exp[1].t == Types.Double))))
            {
                Node old_n = n;
                n = new Node() { t = Types.Operator, op = Operators.Multiplication, priority_value = prev_n.priority_value + 1 };
                n.exp[0] = new Node { t = Types.Double, value = 1 };
                n.exp[1] = old_n;
            }
            return n;
        }

        private bool CheckForVariable(Node n, int depth, int max_depth)
        {
            //ok after rule 4 all variables have an coeficient to the variable, this functions ONLY works after rule 4
            //they can be in their (coef*variable) form;
            if (depth > max_depth || n == null)
            {
                return false;
            }
            if (n.op == Operators.Multiplication && n.exp[1].t == Types.Variable) //variables are always on the right side
            {
                return true;
            } else
            {
                bool t1 = CheckForVariable(n.exp[0], depth++, max_depth);
                bool t2 = CheckForVariable(n.exp[1], depth++, max_depth);
                return t1 || t2;

            }

        }

        private Node ForthHalfRule(Node n, Node prev_n)
        {
            //Ok this rule is a precedent to rule five, because it's a problem if the equation has a coeficient, this is to detect that
            //and seperate the coeficients before the fifth rule is applied. 2* x^2 * 3 * x^3 something like this
            if (n.op == Operators.Multiplication)
            {
                if (n.exp[0] == null)
                {
                    return n;
                }
                //we have two cases both of the children are an multiplication or one children are an exponent and a multiplication.
                // 1st (2* x^2) * (3 * x^3)
                // 2nd 2* x^2 * 3 * x^3

                //we do the first case first
                if (n.exp[0].op == Operators.Multiplication && n.exp[1].op == Operators.Multiplication)
                {
                    //now we need to know if there is an variable on both children 
                    //we use the checkforvariable function
                    if (CheckForVariable(n.exp[0], 0, 2) == true && CheckForVariable(n.exp[1], 0, 2) == true)
                    {
                        Node coef1 = n.exp[0].exp[0]; // once again assuming that coeficients are on the left
                        Node coef2 = n.exp[1].exp[0];
                        Node exponent1 = n.exp[0].exp[1]; // assuming that exponents or variables are on the right
                        Node exponent2 = n.exp[1].exp[1];
                        //we initialize the coeficient side
                        n.exp[0] = new Node { t = Types.Operator, op = Operators.Multiplication, priority_value = n.priority_value+ 2 };
                        n.exp[0].exp[0] = coef1;
                        n.exp[0].exp[1] = coef2;
                        //we initialize the variable side
                        n.exp[1] = new Node { t = Types.Operator, op = Operators.Multiplication, priority_value = n.priority_value++ };
                        n.exp[1].exp[0] = exponent1;
                        n.exp[1].exp[1] = exponent2;
                    }
                }
                // now we go to the 2nd case 2* x^2 * 3 * x^3
                if (n.exp[0].op == Operators.Multiplication && n.exp[1].op == Operators.Exponent)
                {
                    if(CheckForVariable(n.exp[0], 0, 3) == true && CheckForVariable(n.exp[1], 0, 2) == true)
                    {
                        Node coef1 = n.exp[0].exp[0].exp[0];
                        Node coef2 = n.exp[0].exp[1];
                        Node exponent1 = n.exp[0].exp[0].exp[1];
                        Node exponent2 = n.exp[1];
                        n.exp[0] = new Node { t = Types.Operator, op = Operators.Multiplication, priority_value = n.priority_value + 2 };
                        n.exp[0].exp[0] = coef1;
                        n.exp[0].exp[1] = coef2;
                        //we initialize the variable side
                        n.exp[1] = new Node { t = Types.Operator, op = Operators.Multiplication, priority_value = n.priority_value++ };
                        n.exp[1].exp[0] = exponent1;
                        n.exp[1].exp[1] = exponent2;
                    }
                }
            }
            return n;
        }
       
        private Node FifthRule(Node n, Node prev_n)
        {

            if(n.t == Types.Operator && n.op == Operators.Multiplication)//check if node is for multiplication
            { 
                if((n.exp[0].t == Types.Operator && n.exp[0].op == Operators.Exponent) && (n.exp[1].t == Types.Operator && n.exp[1].op == Operators.Exponent))
                {
                    //check if the children are exponents 
                    //Now we assume the the 4th rule is apllied and the variable is always on the left side of the simplification (have to create a rule for that)
                    //also we need to check if node is a variable in them
                    
                    Node left_exponent_base = n.exp[0].exp[0];
                    Node left_exponent_exponent = n.exp[0].exp[1];
                    Node right_exponent_base = n.exp[1].exp[0];
                    Node right_exponent_exponent = n.exp[1].exp[1];
                    if (left_exponent_base.exp[1].t == Types.Variable && right_exponent_base.exp[1].t == Types.Variable)
                    {
                        string var_name1 = left_exponent_base.exp[1].var;
                        string var_name2 = right_exponent_base.exp[1].var;
                        if(var_name1 == var_name2)
                        {
                            //We also assume that the coefient of this variable is 1, a new rule must be applied before
                            Node old_n = n;
                            n = new Node{ t = Types.Operator, op = Operators.Exponent, priority_value = old_n.priority_value }; //setting up exponent

                            //Now we setup the exponent side
                            n.exp[0] = left_exponent_base.exp[1];
                            n.exp[1] = new Node { t = Types.Operator, op = Operators.Addition, priority_value = n.priority_value + 1 };
                            n.exp[1].exp[0] = left_exponent_exponent;
                            n.exp[1].exp[1] = right_exponent_exponent;
                        }
                    }
                }
            } 
            return n;
        }

    }
}
