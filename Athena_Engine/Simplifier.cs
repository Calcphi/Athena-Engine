using System;
using System.Collections.Generic;
using System.Text;

namespace Athena_Engine
{
    public class Simplifier
    {
        List<Func<Node, Node, Node>> rules = new List<Func<Node, Node, Node>>();
        Solver s = new Solver();
        bool applied_forth_rule = false;

        public Simplifier()
        {
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
            Func<Node, Node, Node> despaghetti = Despaghettifier;
            rules.Add(despaghetti);            
            Func<Node, Node, Node> canonical = CananonicalOrder;
            rules.Add(canonical);
        }

        public Node Simplify(Node origin)
        {
            applied_forth_rule = false;
            Node old_simplify = SimplifyRecursion(origin, origin);
            old_simplify = SolveWherePossible(old_simplify);
            applied_forth_rule = true;
            while (true)
            {
                Node new_simplify = SimplifyRecursion(old_simplify, old_simplify);
                new_simplify = SolveWherePossible(new_simplify);
                if (new_simplify == old_simplify)
                {
                    old_simplify = new_simplify;
                    break;
                }
                old_simplify = new_simplify;
            }
            
            //after simplifying the maximum we can solve parts of the equation that only have numbers between them
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

        public Node SolveWherePossible(Node n)
        {
            Node Solve(Node nn)
            {
                if (n.t == Types.Operator && (n.exp[0].t == Types.Double && n.exp[1].t == Types.Double))
                {
                    double value = s.Solve(n);
                    n = new Node() { t = Types.Double, value = value, priority_value = n.priority_value };
                    //it should inherit the priority value because of exponents, then it works always
                }
                return n;
            }
            if (n.exp[0] == null)
            {
                return n;
            }
            Node old_n = n;
            n = Solve(n);
            if (old_n == n)
            {
                n.exp[0] = SolveWherePossible(n.exp[0]);
                n.exp[1] = SolveWherePossible(n.exp[1]);
                //try to solve it again
                n = Solve(n);
            }
            return n;
        }

        private Node IncrementPriorityValue(Node n)
        {
            if (n.t == Types.Operator || (n.t == Types.Double && n.priority_value > 0))
            {
                n.priority_value++;
            }
            else if (!(n.exp[0] == null || n.exp[1] == null)) //check if there are children
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
            if (n.op != Operators.Subtraction)
            {
                return n;
            }
            n.op = Operators.Addition;
            Node old_r = n.exp[1]; //get the old right node
            n.exp[1] = new Node()
            {
                t = Types.Operator,
                op = Operators.Multiplication,
                priority_value = n.priority_value + 1
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
            if(applied_forth_rule == true)
            {
                return n;
            }
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
            }
            else
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
                        n.exp[0] = new Node { t = Types.Operator, op = Operators.Multiplication, priority_value = n.priority_value + 2 };
                        n.exp[0].exp[0] = coef1;
                        n.exp[0].exp[1] = coef2;
                        //we initialize the variable side
                        n.exp[1] = new Node { t = Types.Operator, op = Operators.Multiplication, priority_value = n.priority_value++ };
                        n.exp[1].exp[0] = exponent1;
                        n.exp[1].exp[1] = exponent2;
                    }
                }
                // now we go to the 2nd case 2* x^2 * 3 * x^3, where all have the same priority level
                if (n.exp[0].op == Operators.Multiplication && n.exp[1].op == Operators.Exponent)
                {
                    if (CheckForVariable(n.exp[0], 0, 3) == true && CheckForVariable(n.exp[1], 0, 2) == true)
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
            if (n.op == Operators.Division)
            {
                if (n.exp[0].op == Operators.Multiplication && n.exp[1].op == Operators.Multiplication)
                {
                    Node numerator = n.exp[0];
                    Node denominator = n.exp[1];
                    if (CheckForVariable(numerator, 0, 2) == true && CheckForVariable(denominator, 0, 2))
                    {
                        //as always we assume that ´the variable is on the right and the coeficient is on the left
                        Node coeficient_numerator = numerator.exp[0];
                        Node coeficient_denominator = denominator.exp[0];

                        Node variable_numerator = numerator.exp[1];
                        Node variable_denominator = denominator.exp[1];

                        n = new Node() { t = Types.Operator, op = Operators.Multiplication, priority_value = n.priority_value };
                        //create the coeficient division
                        n.exp[0] = new Node() { t = Types.Operator, op = Operators.Division, priority_value = n.priority_value + 1 };
                        n.exp[0].exp[0] = coeficient_numerator;
                        n.exp[0].exp[1] = coeficient_denominator;

                        //create the variable division
                        n.exp[1] = new Node() { t = Types.Operator, op = Operators.Division, priority_value = n.priority_value + 1 };
                        n.exp[1].exp[0] = variable_numerator;
                        n.exp[1].exp[1] = variable_denominator;
                    }
                }
            }

            return n;
        }

        private Node FifthRule(Node n, Node prev_n)
        {

            if (n.t == Types.Operator && n.op == Operators.Multiplication)//check if node is for multiplication
            {
                if ((n.exp[0].t == Types.Operator && n.exp[0].op == Operators.Exponent) && (n.exp[1].t == Types.Operator && n.exp[1].op == Operators.Exponent))
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
                        if (var_name1 == var_name2)
                        {
                            //We also assume that the coefient of this variable is 1, a new rule must be applied before
                            Node old_n = n;
                            n = new Node { t = Types.Operator, op = Operators.Exponent, priority_value = old_n.priority_value }; //setting up exponent

                            //Now we setup the exponent side
                            n.exp[0] = left_exponent_base.exp[1];
                            n.exp[1] = new Node { t = Types.Operator, op = Operators.Addition, priority_value = n.priority_value + 1 };
                            n.exp[1].exp[0] = left_exponent_exponent;
                            n.exp[1].exp[1] = right_exponent_exponent;
                        }
                    }
                }
            }
            //Now we will work on the division beetween two exponents of the same variable
            if (n.op == Operators.Division)
            {
                if (n.exp[0].op == Operators.Exponent && n.exp[1].op == Operators.Exponent)
                {
                    //check if the children are exponents 
                    //Now we assume the the 4th rule is apllied and the variable is always on the left side of the simplification (have to create a rule for that)
                    //also we need to check if node is a variable in them

                    Node numerator_exponent_base = n.exp[0].exp[0];
                    Node numerator_exponent_exponent = n.exp[0].exp[1];
                    Node denominator_exponent_base = n.exp[1].exp[0];
                    Node denominator_exponent_exponent = n.exp[1].exp[1];

                    //we check if the variable is there
                    if (CheckForVariable(numerator_exponent_base, 0, 2) == true && CheckForVariable(denominator_exponent_base, 0, 2) == true)
                    {
                        if (numerator_exponent_base.exp[1].var == denominator_exponent_base.exp[1].var)
                        {
                            n = new Node() { t = Types.Operator, op = Operators.Exponent, priority_value = n.priority_value };
                            n.exp[0] = numerator_exponent_base.exp[1];
                            n.exp[1] = new Node() { t = Types.Operator, op = Operators.Subtraction, priority_value = n.priority_value + 1 };
                            n.exp[1].exp[0] = numerator_exponent_exponent;
                            n.exp[1].exp[1] = denominator_exponent_exponent;
                        }
                    }

                }
            }
            return n;
        }

        private Node Despaghettifier(Node n, Node last_node)
        {
            if(n.op == Operators.Multiplication )
            {
                if((n.exp[0].t == Types.Double && n.exp[0].value == 1) && (n.exp[1].t == Types.Variable))
                {
                   
                    n = n.exp[1];
                }
            } 
            return n;
        }

        private List<(Node, double)> AddVariablesIfPossible(List<(Node,double)> NotOrganizedList)
        {
            string GetVariableName(Node n)
            {
                if(n.t == Types.Variable)
                {
                    return n.var;
                }
                if(n.op == Operators.Exponent)
                {
                    return n.exp[0].var;
                }
                if(n.op == Operators.Multiplication)
                {
                    if(n.exp[1].op == Operators.Exponent)
                    {
                        return n.exp[1].exp[0].var;
                    }
                    if(n.exp[1].t == Types.Variable){
                        return n.exp[1].var;
                    }
                }
                return "";
            }

            double GetCoeficientVariable(Node n)
            {
                if (n.t == Types.Variable)
                {
                    return 1;
                }
                if(n.op == Operators.Exponent)
                {
                    return 1;
                }
                if(n.op == Operators.Multiplication)
                {
                    if (n.exp[1].op == Operators.Exponent || n.exp[1].t == Types.Variable)
                    {
                        return n.exp[0].value;
                    }
                }
                return 0;
            }
            
            
            //this is preferable to call this function before it's organized makes my life a way lot easier and don't
            //have to deal with stuff that is already done


            double max_degree = 0;
            //Now we find the max degree we have to loop from
            foreach((Node n, double i) in NotOrganizedList)
            {
                if (max_degree < i)
                {
                    max_degree = i;
                }
            }
            //this part wont be very optimized but oh well
            for(int i = 0; i <= max_degree; i++)
            {
                //first of we get all members of the same degree in a list
                List<Node> same_degree_members = new List<Node>();
                foreach((Node n, double e) in NotOrganizedList)
                {
                    if (e == i)
                    {
                        same_degree_members.Add(n);
                    }
                }
                List<int> used_degree_members = new List<int>();
                //this will only do a addition at a time because implementation time and this can not be very efficient
                for (int e = 0; e<=(same_degree_members.Count - 1); e++)
                {
                    if (used_degree_members.Contains(e))
                    {
                        continue;
                    }
                    string var_name1 = GetVariableName(same_degree_members[e]);
                    for (int o = 0; o <= (same_degree_members.Count - 1); o++)
                    {
                        if(e == o)
                        {
                            continue;
                        }
                        if (used_degree_members.Contains(o) || used_degree_members.Contains(e))
                        {
                            continue;
                        }
                        string var_name2 = GetVariableName(same_degree_members[o]);
                        if(var_name1 != var_name2) //check if the variables names are the same
                        {
                            continue;
                        }
                        if(var_name1 == "" || var_name2 == ""){
                            continue;
                        }
                        double coef1 = GetCoeficientVariable(same_degree_members[e]);
                        double coef2 = GetCoeficientVariable(same_degree_members[o]);
                        //add to the used degree members
                        used_degree_members.Add(e);
                        used_degree_members.Add(o);

                        //remove from original list
                        NotOrganizedList.Remove((same_degree_members[e], i));
                        NotOrganizedList.Remove((same_degree_members[o], i));

                        //Finally we create a new node that does the addition between the two
                        Node n_a = new Node() { t = Types.Operator, op = Operators.Multiplication };

                        //create node for coeficient
                        n_a.exp[0] = new Node() { t = Types.Double, value = coef1 + coef2 };

                        if(i == 1)
                        {
                            n_a.exp[1] = new Node() { t = Types.Variable, var = var_name1 };
                        }
                        else
                        {
                            n_a.exp[1] = new Node() { t = Types.Operator, op = Operators.Exponent };
                            n_a.exp[1].exp[0] = new Node() { t = Types.Variable, var = var_name1 };
                            n_a.exp[1].exp[1] = new Node() { t = Types.Double, value = i };
                        }
                        //add n_a to the list
                        NotOrganizedList.Add((n_a, i));
                        


                    }

                }
            }
            return NotOrganizedList;
        }

        private double GetDegree(Node n_term)
        {
            double degree = 0;
            if (n_term.t == Types.Variable)
            {
                degree = 1;
            }
            else if (n_term.op == Operators.Multiplication)
            {
                if (n_term.exp[1].op == Operators.Exponent)
                {
                    degree = n_term.exp[1].exp[1].value;
                }
                if(n_term.exp[1].t == Types.Variable)
                {
                    degree = 1;
                }

            }
            else if (n_term.op == Operators.Exponent)
            {
                degree = n_term.exp[1].value;
            }
            return degree;
        }

        private Node CananonicalOrder(Node n, Node last_node)
        {
            bool CheckIfCanonicalOrderIsPossible(Node n_t)
            {
                bool n_testcondition = false;
                bool check_children = true;
                if(n_t is null)
                {
                    return true;
                }
                if(n_t.t == Types.Operator && n_t.op == Operators.Addition)
                {
                    n_testcondition = true;
                }
                if(n_t.op == Operators.Exponent)
                {
                    if(n_t.exp[0].t == Types.Variable && n_t.exp[1].t == Types.Double) //this means that the exponent must be solved first
                    {
                        n_testcondition = true;
                        check_children = false;
                    }
                }
                if(n_t.op == Operators.Multiplication)
                {
                    bool left_check = false;
                    bool right_check = false;
                    //the right side must be a coeficient and the left side could be a exponent or a variable
                    if(n_t.exp[0].t == Types.Double)
                    {
                        left_check = true;
                        
                    }
                    if(n_t.exp[1].op == Operators.Exponent)
                    {
                        if (n_t.exp[1].exp[0].t == Types.Variable && n_t.exp[1].exp[1].t == Types.Double) //this means that the exponent must be solved first
                        {
                            right_check = true;
                            check_children = false;
                        }
                    }
                    else if(n_t.exp[1].t == Types.Variable)
                    {
                        right_check = true;
                        check_children = false;
                    }
                    n_testcondition = left_check && right_check;
                }
                if(n_t.t == Types.Variable)
                {
                    n_testcondition = true;
                    check_children = false;
                }
                if(n_t.t == Types.Double)
                {
                    return true;
                }
                bool n1_testcondition = true;
                bool n2_testcondition = true;
                if(check_children == true)
                {
                    n1_testcondition = CheckIfCanonicalOrderIsPossible(n_t.exp[0]);
                    n2_testcondition = CheckIfCanonicalOrderIsPossible(n_t.exp[1]);
                }

                return n_testcondition && n1_testcondition && n2_testcondition;
            }

            List<Node> GetAllTerms(Node root)
            {
                List<Node> terms = new List<Node>();
                bool check_children = true;
                if(root is null)
                {
                    return terms;
                }
                if(root.op != Operators.Addition || root.t != Types.Operator)
                {
                    check_children = false;
                    terms.Add(root);
                }
                if(check_children == true)
                {
                    List<Node> terms1 = GetAllTerms(root.exp[0]);
                    List<Node> terms2 = GetAllTerms(root.exp[1]);
                    terms.AddRange(terms1);
                    terms.AddRange(terms2);
                }
                return terms;
            }



            Node WriteInCanonicalOrder(Node origin, int plus_used, int plus_max, List<(Node nn, double i)> node_list)
            {
                if(plus_used > plus_max)
                {
                    return origin;
                }
                origin = new Node() { t = Types.Operator, op = Operators.Addition };
                if(plus_used == plus_max)
                {
                    origin.exp[0] = node_list[0].nn;
                    origin.exp[1] = node_list[1].nn;
                    return origin;
                }
                origin.exp[1] = node_list[node_list.Count - 1].nn; //use the last one in the list
                node_list.Remove(node_list[node_list.Count - 1]); //remove the last item for the list because it's not needed anymore
                origin.exp[0] = WriteInCanonicalOrder(origin.exp[0], plus_used + 1,plus_max, node_list);
                return origin;
            }

            //to apply the canonical order all nodes must with + between variables and numbers 
            //this must be applied at the root of the function, where n == last_node
            if(n == last_node)
            {
                if(n.t==Types.Operator && n.op == Operators.Addition)
                {
                    if (CheckIfCanonicalOrderIsPossible(n)) {
                        List<Node> terms = GetAllTerms(n);
                        
                        List<(Node nn, double i)> organized_list = new List<(Node, double)>();
                        foreach(Node term in terms)
                        {
                            organized_list.Add((term ,GetDegree(term)));
                        }
                        List<(Node nn, double i)> old_organized_list = new List<(Node, double)>();
                        old_organized_list = organized_list;
                        while (true)
                        {
                            organized_list = AddVariablesIfPossible(organized_list);
                            if(organized_list == old_organized_list)
                            {
                                break;
                            }
                            old_organized_list = organized_list;
                        }
                        organized_list.Sort((x,y) => y.i.CompareTo(x.i));
                        //the addition needed is always one less of the total number of terms
                        if(organized_list.Count > 1)
                        {
                            n = WriteInCanonicalOrder(n, 1, organized_list.Count - 1, organized_list);
                        } else
                        {
                            n = organized_list[0].nn;
                        }
                        
                    }
                }
            }
            return n;
        }
    }
}
