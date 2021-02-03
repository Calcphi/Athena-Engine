using System;
using System.Collections.Generic;
using System.Text;

namespace Athena_Engine
{
    class Functions
    {
        public List<(string, Func<string, int, string>)> functions = new List<(string, Func<string, int, string>)>();

        public Functions()
        {
            Func<string, int, string> r = RootFunction;
            functions.Add(("root", r));
        }

       private int FindCloseParenthesis(string input, int first_parenthesis)
        {
            List<int> excluded_close_parenthesis = new List<int>();
            int closed_parenthesis_index = 0;
            for(int i = first_parenthesis + 1; i <= (input.Length - 1); i++)
            { 
                if(input[i]  == '(')
                {
                    excluded_close_parenthesis.Add(FindCloseParenthesis(input, i));
                    continue;
                } else if(input[i] == ')')
                {
                    if (excluded_close_parenthesis.Contains(i) == false)
                    {
                        closed_parenthesis_index = i;
                    }
                }
            }
            if (closed_parenthesis_index == 0)
            {
                throw new ArgumentException("There is a Syntax error in the expression, probably there is too many closing parenthesis...");
            }
            return closed_parenthesis_index;
        }
        
        public string RootFunction(string input, int index)
        {
            //Now let's find the root_index
            string root_index = "";
            if (Char.IsDigit(input[index + 4]) || input[index + 4] == '-'){
                root_index += input[index + 4];
            } else
            {
                throw new ArgumentException("There's no index at the the 5th letter");
            }
            int first_parenthesis = 0;
            for (int i = index + 5; i <= (input.Length - 1); i++)
            {
                if (Char.IsDigit(input[i]))
                {
                    root_index += input[i];
                    continue;
                }
                else
                {
                    if (input[i] == '(')
                    {
                        first_parenthesis = i;
                    }
                    break;
                }
            }
            if(root_index == "-")
            {
                throw new ArgumentException("There is no number after the minus signal.");
            }
            if (first_parenthesis == 0)
            {
                throw new ArgumentException("Didn't find the first REQUIRED parenthesis");
            }
            int closing_parenthesis = FindCloseParenthesis(input, first_parenthesis);
            string expression = "";
            for (int i = first_parenthesis + 1; i < closing_parenthesis; i++)
            {
                expression += input[i];
            }
            
            return input.Replace(input.Substring(index, closing_parenthesis - index), "("+expression+")"+"^"+"(1/"+root_index+")");
        }

    }
}
