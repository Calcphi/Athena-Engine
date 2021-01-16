using System;
using System.Collections.Generic;
using System.Globalization;


//TODO: Make Operators

namespace Athena_Engine {


	public class Tokenizer
	{

		private List<string> operators = new List<string>(){ "+", "-", "*", "/" };
		public Tokenizer()
		{
		}
		
		public List<Node> Tokenize(string s){

			int len_to_jump = 0;
			List<Node> list_of_nodes = new List<Node>();
			
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
					no.t = Types.Operator;
					switch (s[i].ToString()){ //Check what operator is it
						case "+":
							no.op = Operators.Addition;
							break;
						case "-":
							no.op = Operators.Subtraction;
							break;
						case "*":
							no.op = Operators.Multiplication;
							break;
						case "/":
							no.op = Operators.Division;
							break;
					}
					list_of_nodes.Add(no);
                }
				else if (Char.IsDigit(s[i])) { //NOTE: The first digit must start with a number not a . eg: 0.1 and not .1
					string number = s[i].ToString();
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
					nn.t = Types.Double;
					nn.value = double.Parse(number, CultureInfo.InvariantCulture);
					list_of_nodes.Add(nn);
					if (got_len == false)
                    {
						break;
                    }
				}
			}
			return list_of_nodes;
		}
	}
}

