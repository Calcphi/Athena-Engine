using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Athena_Engine;

namespace Tests
{
    [TestClass]
    public class SimplifierTest1
    {
        Tokenizer t = new Tokenizer();
        Parser p = new Parser();
        Simplifier simp = new Simplifier();
        Random r = new Random();

        [TestMethod]
        public void FifthRuleTestingMultiplication1()
        {
            for(int i = 0; i <= 1000; i++)
            {
                int exponent1 = r.Next(1, 10000000);
                int exponent2 = r.Next(1, 10000000);

                Node actual = simp.Simplify(p.Parse(t.Tokenize("x^" + exponent1.ToString() + "*x^" + exponent2.ToString())));
                Node expected = p.Parse(t.Tokenize("x^" + (exponent1 + exponent2).ToString()));
                Assert.IsTrue(expected == actual);
            }
        }

        [TestMethod]
        public void FifthRuleTestingDivision1()
        {
            for (int i = 0; i <= 1000; i++)
            {
                int exponent1 = r.Next(1, 10000000);
                int exponent2 = r.Next(1, 10000000);

                Node actual = simp.Simplify(p.Parse(t.Tokenize("x^" + exponent1.ToString() + "/x^" + exponent2.ToString())));
                Node expected = p.Parse(t.Tokenize("x^" + (exponent1 - exponent2).ToString()));
                Assert.IsTrue(expected == actual);
            }
        }
    }
}
