using System;
using System.Collections.Generic;
using System.Text;

namespace Athena_Engine
{
    public enum Types {

        Variable,
        Operator,
        Double
    }

    public enum Flags
    {
        None,
        Priority
    }

    public enum Operators
    {
        Addition,
        Subtraction,
        Division,
        Multiplication,
        Exponent
    }
}
