using System;
using System.Collections.Generic;

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
					for (int e = i + 1; e<= (s.Length - 1); e++) //Now get the rest of chars of the variable and get the next element to tokenize
                    {
						if (Char.IsLetter(s[e]))
                        {
							variable += s[e];
                        } else
                        {
							len_to_jump = e - 1;
							break;
                        }
                    }
					//TODO:Check if variable is different than exclusive ones if not, create an exception
					Node nv = new Node(); //Finally create a node for this variable and add it to the list
					nv.t = Types.Variable;
					nv.var = variable;
					list_of_nodes.Add(nv);
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
				
			}
			return list_of_nodes;
		}
	}
}

