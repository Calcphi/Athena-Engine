using System;
using System.Collections.Generic;


//TODO: Make Operators

namespace Athena_Engine {

	
	public class Tokenizer
	{
		public Tokenizer()
		{
		}
		
		public void Tokenize(string s){

			int len_to_jump = 0;
			List<Node> list_of_nodes = new List<Node>();
			
			for(int i = 0; i<=(s.Length - 1); i++) {
				if (len_to_jump != 0)
                {
					len_to_jump -= 1;
					continue;
                }
				if (Char.IsLetter(s[i])) //Check if current letter is a string and if it is creates a Node with it
                {
					string variable = s[i].ToString();
					for (int e = i + 1; e<= (s.Length - 1); e++)
                    {
						if (Char.IsLetter(s[e]))
                        {
							variable += s[e];
                        } else
                        {
							len_to_jump = e + 1;
                        }
                    }
					Console.WriteLine(variable);
				}
			}
		}
	}
}

