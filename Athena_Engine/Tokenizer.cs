using System;
using System.Collections.Generic;
using System.Globalization;


//TODO: Make Operators

namespace Athena_Engine {


	public class Tokenizer
	{
		private Functions funct = new Functions();

		

		private List<string> operators = new List<string>(){ "+", "-", "*", "/", "^"};

		public Tokenizer()
		{
			

		}
		
		private string SearchForFunctions(string input)
        {
			foreach((string func, Func<string, int, string> thing) in funct.functions)
            {
				//All functions should ONLY modify the the input string
				int index = 0;
				while(input.IndexOf(func, index) != -1){

					index = input.IndexOf(func, index);
					input = thing(input, index);
					if (index == input.IndexOf(func, index))
                    {
						index++;
                    }
					
					

				}
            }
			return input;
        }

		public List<Node> Tokenize(string s){
			s = SearchForFunctions(s);
			int len_to_jump = 0;
			List<int> priority_operator = new List<int>();
			List<Node> list_of_nodes = new List<Node>();
			List<int> closed_parenthesis_used = new List<int>();
			
			for(int i = 0; i<=(s.Length - 1); i++) {
				if (len_to_jump > 0)
                {
					len_to_jump -= 1;
					continue;
                }
				else if (Char.IsLetter(s[i])) //Check if current letter is a string and if it is creates a Node with it
                {
					string variable = s[i].ToString();
					bool got_len = false;
					for (int e = i + 1; e<= (s.Length - 1); e++) //Now get the rest of chars of the variable and get the next element to tokenize
                    {
						if (Char.IsLetter(s[e]))
                        {
							variable += s[e];
                        } else
                        {
							got_len = true;
							len_to_jump = e - i - 1;
							break;
                        }
                    }
					//TODO:Check if variable is different than exclusive ones if not, create an exception
					Node nv = new Node(); //Finally create a node for this variable and add it to the list
					nv.t = Types.Variable;
					nv.var = variable;
					list_of_nodes.Add(nv);
					if (got_len == false)
					{
						break;
					}
				} 
				else if (operators.Contains(s[i].ToString())){ //Check if current char is a valid operator
					Node no = new Node();
                    //Now we are going to check if the operator is given the priority flag
                    if (priority_operator.Contains(i))
                    {
						no.f = Flags.Priority;
						int p_level = 0;
						foreach(int e in priority_operator)
                        {
                            if (i == e)
                            {
								p_level++;
                            }
                        }
						no.priority_value = p_level;

                    }
					no.t = Types.Operator;
					bool negative_number = false;
					switch (s[i].ToString()){ //Check what operator is it
						case "+":
							no.op = Operators.Addition;
							break;
						case "-":
							no.op = Operators.Subtraction;
                            try
                            {
								if (operators.Contains(s[i - 1].ToString()) || s[i-1] == '(')
								{
									negative_number = true;
								}
							} catch (System.IndexOutOfRangeException)
                            {
								negative_number = true;
                            }
							break;
						case "*":
							no.op = Operators.Multiplication;
							break;
						case "/":
							no.op = Operators.Division;
							break;
						case "^":
							no.op = Operators.Exponent;
							break;
					}
					if(negative_number == false)
                    {
						list_of_nodes.Add(no);
					}
					
                }
				if (Char.IsDigit(s[i]) || s[i] == '-') { //NOTE: The first digit must start with a number not a . eg: 0.1 and not .1
					string number = "";
					if (s[i] == '-')
                    {
						try
                        {
							if (operators.Contains(s[i - 1].ToString()) || s[i - 1] == '(')
							{
								number = s[i].ToString();
							} else {
								continue; //continue because this - is an operator
                            }
						} catch(System.IndexOutOfRangeException)
                        {
							number = s[i].ToString();
						}
					}
                    else
                    {
						number = s[i].ToString();
					}

					
					bool got_len = false;
					for (int e = i + 1; e <= (s.Length - 1); e++) //Now get the rest of chars of the variable and get the next element to tokenize
					{
						if (Char.IsDigit(s[e]) || s[e] == '.' )
						{
							number += s[e];
						}
						else
						{
							got_len = true;
							len_to_jump = e - i - 1;
							break;
						}
					}
					
					Node nn = new Node();
					if (s[i - 1] == '(' && s[i + len_to_jump + 1] == ')')
                    {
						nn.f = Flags.Priority;
						nn.priority_value = 1;
                    }
					nn.t = Types.Double;
					nn.value = double.Parse(number, CultureInfo.InvariantCulture);
					list_of_nodes.Add(nn);
					if (got_len == false)
                    {
						break;
                    }
				}
				else if (s[i] == '(') //Check for operators to give the priority flag
                {
					bool found_parentheses = false;
					for (int e = i + 1; e <= (s.Length - 1); e++)
                    {
						List<char> operators = new List<char>() { '+', '-', '*', '/', '^' }; //Add the length of the operators that need the priority flag
						if (operators.Contains(s[e]))
                        {
							priority_operator.Add(e);
                        } else if (s[e] == ')')
                        {
							if (closed_parenthesis_used.Contains(e))
                            {
								continue;
                            } else
                            {
								found_parentheses = true;
								closed_parenthesis_used.Add(e);
								break;
							}

                        }
                    }
					if (found_parentheses == false)
                    {
						throw new ArgumentException("There is one parentheses missing in this expression");
                    }
                }
			}
			return list_of_nodes;
		}
	}
}

