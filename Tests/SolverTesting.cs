using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Athena_Engine;

namespace Tests
{
    [TestClass]
    public class SolverTesting
    {
        Tokenizer t = new Tokenizer();
        Parser p = new Parser();
        Solver s = new Solver();

        [TestMethod]
        
        public void SolverAdditionTest()
        {
            for (int i = 0; i <= 100; i++)
            {
                Random r = new Random();
                int i1 = r.Next(1, 1000000);
                int i2 = r.Next(1, 1000000);

                Assert.AreEqual((double)(i1 + i2), s.Solve(p.Parse(t.Tokenize(i1.ToString() + "+" + i2.ToString()))));
            }
        }

        [TestMethod]
        public void SolverSubtractionTest()
        {
            for (int i = 0; i <= 100; i++)
            {
                Random r = new Random();
                int i1 = r.Next(1, 1000000);
                int i2 = r.Next(1, 1000000);

                Assert.AreEqual((double)(i1 - i2), s.Solve(p.Parse(t.Tokenize(i1.ToString() + "-" + i2.ToString()))));
            }
        }

        [TestMethod]
        public void SolverMultiplicationTest()
        {
            for (int i = 0; i <= 100; i++)
            {
                Random r = new Random();
                int i1 = r.Next(1, 1000000);
                int i2 = r.Next(1, 1000000);

                Assert.AreEqual((double)i1 * i2, s.Solve(p.Parse(t.Tokenize(i1.ToString() + "*" + i2.ToString()))));
            }
        }

        [TestMethod]
        public void SolverDivisionTest()
        {
            for (int i = 0; i <= 100; i++)
            {
                Random r = new Random();
                int i1 = r.Next(1, 1000000);
                int i2 = r.Next(1, 1000000);

                Assert.AreEqual((double)i1 / i2, s.Solve(p.Parse(t.Tokenize(i1.ToString() + "/" + i2.ToString()))));
            }
        }

        [TestMethod]
        public void SolverExponentTest()
        {
            for (int i = 0; i <= 100; i++)
            {
                Random r = new Random();
                int i1 = r.Next(1, 1000000);
                int i2 = r.Next(1, 1000000);

                Assert.AreEqual((double)Math.Pow(i1, i2), s.Solve(p.Parse(t.Tokenize(i1.ToString() + "^" + i2.ToString()))));
            }
        }
    }
}
