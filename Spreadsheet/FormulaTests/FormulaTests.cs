using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;

namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {
        [TestMethod]
        public void TestEvaluate_simpleArithmetic()
        {
            Formula f = new Formula("5.6 - 3.6");

            double val = (double)f.Evaluate(x => 0);

            Assert.AreEqual(2.0, val, 1e-9);

        }

        [TestMethod]
        public void TestEvaluate_simpleArithmetic2()
        {
            Formula f = new Formula("5 / 0");

            Assert.IsInstanceOfType(f.Evaluate(x => 0), typeof(FormulaError));

        }
        [TestMethod]
        public void testWithParanthesis1()
        {
            Formula f1 = new Formula("(6+6)/5-4*(1+1)");
            Assert.AreEqual(-5.6, (double)f1.Evaluate(SimpleLookUp), 1e-9);
            Assert.AreEqual("(6+6)/5-4*(1+1)", f1.ToString());
        }

        [TestMethod]
        public void testWithThreeVars()
        {
            Formula f1 = new Formula("A1 + AB23 + A3 * 5");
            Assert.AreEqual(40, (double)f1.Evaluate(SimpleLookUp2), 1e-9);
            Assert.AreEqual("A1+AB23+A3*5",f1.ToString());
        }

        [TestMethod]
        public void testlegalityOfParanthesis()
        {
            try
            {
                Formula f1 = new Formula("2(+2)");
                Assert.IsTrue(false);
            }
            catch (FormulaFormatException)
            {
                Assert.IsTrue(true);
            }
            
        }

        [TestMethod]
        public void testInvalidExpression()
        {
            try
            {
                Formula f1 = new Formula("2//2");
                Assert.IsTrue(false);
            }
            catch (FormulaFormatException)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void testInvalidExpression2()
        {
            try
            {
                Formula f1 = new Formula("-2+5");
                Assert.IsTrue(false);
            }
            catch (FormulaFormatException)
            {
                Assert.IsTrue(true);
            }
        }


        [TestMethod]
        public void testInvalidVar()
        {
            try
            {
                Formula f1 = new Formula("A3B5 + 5");
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void testDivideByZero()
        {
            Formula f1 = new Formula("5+6+7+9+7/(6-6)*8");
            Assert.IsInstanceOfType(f1.Evaluate(SimpleLookUp), typeof(FormulaError));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("2")]
        public void TestSingleVariable()
        {
            Formula f1 = new Formula("X5");
            Assert.AreEqual(13, (double)f1.Evaluate( s => 13), 1e-9);
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("7")]
        public void TestArithmeticWithVariable()
        {
            Formula f1 = new Formula("2+X1");
            Assert.AreEqual(6, (double)f1.Evaluate( s => 4), 1e-9);
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("27")]
        public void TestComplexNestedParensRight()
        {
            Formula f1 = new Formula("x1+(x2+(x3+(x4+(x5+x6))))");
            Assert.AreEqual(6, (double)f1.Evaluate( s => 1));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("28")]
        public void TestComplexNestedParensLeft()
        {
            Formula f1 = new Formula("((((x1+x2)+x3)+x4)+x5)+x6");
            Assert.AreEqual(12, (double)f1.Evaluate(s => 2), 1e-9);
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("29")]
        public void TestRepeatedVar()
        {
            Formula f1 = new Formula("a4-a4*a4/a4");
            Assert.AreEqual(0, (double)f1.Evaluate(s => 3), 1e-9);
        }

        [TestMethod()]
        public void testOperationWithDoubles1()
        {
            Formula f1 = new Formula("a1 +  c23   -d3/c23 +(A1 - 3)", s => s.ToUpper(), s => true);
            Assert.AreEqual(4.06111, (double)f1.Evaluate(SimpleLookUp3), 2e-5);
            Assert.AreEqual("A1+C23-D3/C23+(A1-3)", f1.ToString());
            HashSet<String> expected = new HashSet<String>();
            expected.Add( "A1"); expected.Add("C23"); expected.Add("D3");
            HashSet<String> actual = (HashSet<String>)f1.GetVariables();
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod()]
        public void testEqualsMethodTwoSimpleEquivalentFormulas()
        {
            Formula f1 = new Formula("a1 +  c23   -d3/c23 +(A1 - 3.009)", s => s.ToUpper(), s => true);
            Formula f2 = new Formula("A1 +  c23   -D3/c23 +(A1 - 3.00900000)", s => s.ToUpper(), s => true);
            Assert.IsTrue(f1.Equals(f2));
        }
        [TestMethod()]
        public void testEqualsMethodTwoSimpleUnEquivalentFormulas()
        {
            Formula f1 = new Formula("a1 +  c23   -d3/c23 +(A1 - 3.009)", s => s.ToUpper(), s => true);
            Formula f2 = new Formula("A1 +  c23   -D3/c23 +(A1 - 3.0090001)", s => s.ToUpper(), s => true);
            Assert.IsFalse(f1.Equals(f2));
        }
        [TestMethod()]
        public void testEqualsOperatorWithSimpleFormulas()
        {
            Formula f1 = new Formula("a1 +  c23   -d3/c23 +(A1 - 3.009)", s => s.ToUpper(), s => true);
            Formula f2 = new Formula("A1 +  c23   -D3/c23 +(A1 - 3.00900000)", s => s.ToUpper(), s => true);
            var areEqual = (f1 == f2);
            Assert.IsTrue(areEqual);
        }
        [TestMethod()]
        public void testNotEqualsOperatorWithSimpleFormulas()
        {
            Formula f1 = new Formula("a1 +  c23   -d3/c23 +(A1 - 3.009)", s => s.ToUpper(), s => true);
            Formula f2 = new Formula("A1 +  c23   -D3/c23 +(A1 - 3.0090001)", s => s.ToUpper(), s => true);
            var notEqual = (f1 != f2);
            Assert.IsTrue(notEqual);
        }

        [TestMethod()]
        public void testEqualsWithNull()
        {
            Formula f1 = null;
            Formula f2 = null;
            Assert.IsTrue(f1 == f2);
            Assert.IsFalse(f1 != f2);
        }
        [TestMethod()]
        public void testEqualsWithOneNullOnLeft()
        {
            Formula f1 = null;
            Formula f2 = new Formula("((((x1+x2)+x3)+x4)+x5)+x6");
            Assert.IsFalse(f1 == f2);
            Assert.IsFalse(f2.Equals(f1));
            Assert.IsTrue(f1 != f2);

        }
        [TestMethod()]
        public void testEqualsWithOneNullOnRight()
        {
            Formula f1 = new Formula("((((x1+x2)+x3)+x4)+x5)+x6");
            Formula f2 = null;
            Assert.IsFalse(f1 == f2);
            Assert.IsFalse(f1.Equals(f2));
            Assert.IsTrue(f1 != f2);
        }

        [TestMethod()]
        public void TestExpressionWithUnderscoreVars()
        {
            Formula f1 = new Formula("((((_1+__2)+___3)+_4)+_5)+_____6");
            Assert.AreEqual(12, (double)f1.Evaluate(s => 2), 1e-9);
        }

        [TestMethod]
        public void TestWithBadParanthesisSyntax()
        {
            try
            {
                Formula f1 = new Formula("6.7645- T5((x-5) - 8.34))");
                Assert.IsTrue(false);
            }
            catch (FormulaFormatException)
            {
                Assert.IsTrue(true);
            }
        }
        [TestMethod]
        public void TestWithBadEndSyntax()
        {
            try
            {
                Formula f1 = new Formula("6.7645- T5((x-5) - 8.34) + ");
                Assert.IsTrue(false);
            }
            catch (FormulaFormatException)
            {
                Assert.IsTrue(true);
            }
        }
        [TestMethod]
        public void TsetWithEmptyFormula()
        {
            try
            {
                Formula f1 = new Formula(" ");
                Assert.IsTrue(false);
            }
            catch (FormulaFormatException)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void TestWithConsecutiveOperators()
        {
            try
            {
                Formula f1 = new Formula("6.7645-/ T5((x-5) - 8.34)");
                Assert.IsTrue(false);
            }
            catch (FormulaFormatException)
            {
                Assert.IsTrue(true);
            }
        }
        [TestMethod(), Timeout(5000)]
        [TestCategory("26")]
        public void TestComplexMultiVar()
        {
            Formula f1 = new Formula("y1*3-8/2+4*(8-9*2)/14*x7");
            Assert.AreEqual(5.8, (double)f1.Evaluate(s => (s == "x7") ? 1.4 : 4.6), 1e-2); //5.8 ~ 5.79999999
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("26")]
        public void TestParanthesisWithoutOperation()
        {
            Formula f1 = new Formula("y1+(5)-6+(6-7)-(5.756)*x7");
            Assert.AreEqual(-5.4584, (double)f1.Evaluate(s => (s == "x7") ? 1.4 : 4.6), 1e-9); //5.8 ~ 5.79999999
        }
        [TestMethod(), Timeout(2000)]
        [TestCategory("1")]
        public void TestNormalizerGetVars()
        {
            Formula f = new Formula("2+x1", s => s.ToUpper(), s => true);
            HashSet<string> vars = new HashSet<string>(f.GetVariables());

            Assert.IsTrue(vars.SetEquals(new HashSet<string> { "X1" }));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("2")]
        public void TestNormalizerEquals()
        {
            Formula f = new Formula("2+x1", s => s.ToUpper(), s => true);
            Formula f2 = new Formula("2+X1", s => s.ToUpper(), s => true);

            Assert.IsTrue(f.Equals(f2));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("3")]
        public void TestNormalizerToString()
        {
            Formula f = new Formula("2+x1", s => s.ToUpper(), s => true);
            Formula f2 = new Formula(f.ToString());

            Assert.IsTrue(f.Equals(f2));
        }

        // Validator tests
        [TestMethod(), Timeout(2000)]
        [TestCategory("4")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestValidatorFalse()
        {
            Formula f = new Formula("2+x1", s => s, s => false);
        }

        [TestMethod()]
        [TestCategory("5")]
        public void TestValidatorX1()
        {
            Formula f = new Formula("2+x", s => s, s => (s == "x"));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("6")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestValidatorX2()
        {
            Formula f = new Formula("2+y1", s => s, s => (s == "x"));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("7")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestValidatorX3()
        {
            Formula f = new Formula("2+x1", s => s, s => (s == "x"));
        }


        // Simple tests that return FormulaErrors
        [TestMethod(), Timeout(2000)]
        [TestCategory("8")]
        public void TestUnknownVariable()
        {
            Formula f = new Formula("2+X1");
            Assert.IsInstanceOfType(f.Evaluate(s => { throw new ArgumentException("Unknown variable"); }), typeof(FormulaError));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("9")]
        public void TestDivideByZero()
        {
            Formula f = new Formula("5/0");
            Assert.IsInstanceOfType(f.Evaluate(s => 0), typeof(FormulaError));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("10")]
        public void TestDivideByZeroVars()
        {
            Formula f = new Formula("(5 + X1) / (X1 - 3)");
            Assert.IsInstanceOfType(f.Evaluate(s => 3), typeof(FormulaError));
        }


        // Tests of syntax errors detected by the constructor
        [TestMethod(), Timeout(2000)]
        [TestCategory("11")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestSingleOperator()
        {
            Formula f = new Formula("+");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("12")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraOperator()
        {
            Formula f = new Formula("2+5+");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("13")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraCloseParen()
        {
            Formula f = new Formula("2+5*7)");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("14")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraOpenParen()
        {
            Formula f = new Formula("((3+5*7)");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("15")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNoOperator()
        {
            Formula f = new Formula("5x");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("16")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNoOperator2()
        {
            Formula f = new Formula("5+5x");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("17")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNoOperator3()
        {
            Formula f = new Formula("5+7+(5)8");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("18")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNoOperator4()
        {
            Formula f = new Formula("5 5");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("19")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestDoubleOperator()
        {
            Formula f = new Formula("5 + + 3");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("20")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestEmpty()
        {
            Formula f = new Formula("");
        }

        // Some more complicated formula evaluations
        [TestMethod(), Timeout(2000)]
        [TestCategory("21")]
        public void TestComplex1()
        {
            Formula f = new Formula("y1*3-8/2+4*(8-9*2)/14*x7");
            Assert.AreEqual(5.14285714285714, (double)f.Evaluate(s => (s == "x7") ? 1 : 4), 1e-9);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("22")]
        public void TestRightParens()
        {
            Formula f = new Formula("x1+(x2+(x3+(x4+(x5+x6))))");
            Assert.AreEqual(6, (double)f.Evaluate(s => 1), 1e-9);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("23")]
        public void TestLeftParens()
        {
            Formula f = new Formula("((((x1+x2)+x3)+x4)+x5)+x6");
            Assert.AreEqual(12, (double)f.Evaluate(s => 2), 1e-9);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("53")]
        public void TestRepeatedVar2()
        {
            Formula f = new Formula("a4-a4*a4/a4");
            Assert.AreEqual(0, (double)f.Evaluate(s => 3), 1e-9);
        }

        // Test of the Equals method
        [TestMethod(), Timeout(2000)]
        [TestCategory("24")]
        public void TestEqualsBasic()
        {
            Formula f1 = new Formula("X1+X2");
            Formula f2 = new Formula("X1+X2");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("25")]
        public void TestEqualsWhitespace()
        {
            Formula f1 = new Formula("X1+X2");
            Formula f2 = new Formula(" X1  +  X2   ");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("26")]
        public void TestEqualsDouble()
        {
            Formula f1 = new Formula("2+X1*3.00");
            Formula f2 = new Formula("2.00+X1*3.0");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("27")]
        public void TestEqualsComplex()
        {
            Formula f1 = new Formula("1e-2 + X5 + 17.00 * 19 ");
            Formula f2 = new Formula("   0.0100  +     X5+ 17 * 19.00000 ");
            Assert.IsTrue(f1.Equals(f2));
        }


        [TestMethod(), Timeout(2000)]
        [TestCategory("28")]
        public void TestEqualsNullAndString()
        {
            Formula f = new Formula("2");
            Assert.IsFalse(f.Equals(null));
            Assert.IsFalse(f.Equals(""));
        }


        // Tests of == operator
        [TestMethod(), Timeout(2000)]
        [TestCategory("29")]
        public void TestEq()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("2");
            Assert.IsTrue(f1 == f2);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("30")]
        public void TestEqFalse()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("5");
            Assert.IsFalse(f1 == f2);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("31")]
        public void TestEqNull()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("2");
            Assert.IsFalse(null == f1);
            Assert.IsFalse(f1 == null);
            Assert.IsTrue(f1 == f2);
        }


        // Tests of != operator
        [TestMethod(), Timeout(2000)]
        [TestCategory("32")]
        public void TestNotEq()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("2");
            Assert.IsFalse(f1 != f2);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("33")]
        public void TestNotEqTrue()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("5");
            Assert.IsTrue(f1 != f2);
        }


        // Test of ToString method
        [TestMethod(), Timeout(2000)]
        [TestCategory("34")]
        public void TestString()
        {
            Formula f = new Formula("2*5");
            Assert.IsTrue(f.Equals(new Formula(f.ToString())));
        }


        // Tests of GetHashCode method
        [TestMethod(), Timeout(2000)]
        [TestCategory("35")]
        public void TestHashCode()
        {
            Formula f1 = new Formula("2*5");
            Formula f2 = new Formula("2*5");
            Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());
        }

        // Technically the hashcodes could not be equal and still be valid,
        // extremely unlikely though. Check their implementation if this fails.
        [TestMethod(), Timeout(2000)]
        [TestCategory("36")]
        public void TestHashCodeFalse()
        {
            Formula f1 = new Formula("2*5");
            Formula f2 = new Formula("3/8*2+(7)");
            Assert.IsTrue(f1.GetHashCode() != f2.GetHashCode());
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("37")]
        public void TestHashCodeComplex()
        {
            Formula f1 = new Formula("2 * 5 + 4.00 - _x");
            Formula f2 = new Formula("2*5+4-_x");
            Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());
        }


        // Tests of GetVariables method
        [TestMethod(), Timeout(2000)]
        [TestCategory("38")]
        public void TestVarsNone()
        {
            Formula f = new Formula("2*5");
            Assert.IsFalse(f.GetVariables().GetEnumerator().MoveNext());
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("39")]
        public void TestVarsSimple()
        {
            Formula f = new Formula("2*X2");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "X2" };
            Assert.AreEqual(actual.Count, 1);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("40")]
        public void TestVarsTwo()
        {
            Formula f = new Formula("2*X2+Y3");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "Y3", "X2" };
            Assert.AreEqual(actual.Count, 2);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("41")]
        public void TestVarsDuplicate()
        {
            Formula f = new Formula("2*X2+X2");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "X2" };
            Assert.AreEqual(actual.Count, 1);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("42")]
        public void TestVarsComplex()
        {
            Formula f = new Formula("X1+Y2*X3*Y2+Z7+X1/Z8");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "X1", "Y2", "X3", "Z7", "Z8" };
            Assert.AreEqual(actual.Count, 5);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        // Tests to make sure there can be more than one formula at a time
        [TestMethod(), Timeout(2000)]
        [TestCategory("43")]
        public void TestMultipleFormulae()
        {
            Formula f1 = new Formula("2 + a1");
            Formula f2 = new Formula("3");
            Assert.AreEqual(2.0, f1.Evaluate(x => 0));
            Assert.AreEqual(3.0, f2.Evaluate(x => 0));
            Assert.IsFalse(new Formula(f1.ToString()) == new Formula(f2.ToString()));
            IEnumerator<string> f1Vars = f1.GetVariables().GetEnumerator();
            IEnumerator<string> f2Vars = f2.GetVariables().GetEnumerator();
            Assert.IsFalse(f2Vars.MoveNext());
            Assert.IsTrue(f1Vars.MoveNext());
        }

        // Repeat this test to increase its weight
        [TestMethod(), Timeout(2000)]
        [TestCategory("44")]
        public void TestMultipleFormulaeB()
        {
            TestMultipleFormulae();
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("45")]
        public void TestMultipleFormulaeC()
        {
            TestMultipleFormulae();
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("46")]
        public void TestMultipleFormulaeD()
        {
            TestMultipleFormulae();
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("47")]
        public void TestMultipleFormulaeE()
        {
            TestMultipleFormulae();
        }

        // Stress test for constructor
        [TestMethod(), Timeout(2000)]
        [TestCategory("48")]
        public void TestConstructor()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        // This test is repeated to increase its weight
        [TestMethod(), Timeout(2000)]
        [TestCategory("49")]
        public void TestConstructorB()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("50")]
        public void TestConstructorC()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("51")]
        public void TestConstructorD()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        // Stress test for constructor
        [TestMethod(), Timeout(2000)]
        [TestCategory("52")]
        public void TestConstructorE()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }
        public static double SimpleLookUp(string var)
        {
            if (var.Equals("A2"))
                return 4;
            throw new ArgumentException();
        }
        public static double SimpleLookUp2(string var)
        {
            if (var.Equals("A1"))
                return 2;

            if (var.Equals("AB23"))
                return 3;

            if (var.Equals("A3"))
                return 7;
            throw new ArgumentException();
        }
        public static double SimpleLookUp3(string var)
        {
            if (var.Equals("A1"))
                return 2.8;

            if (var.Equals("C23"))
                return 3.6;

            if (var.Equals("D3"))
                return 7.7;
            throw new ArgumentException();
        }
    }
}