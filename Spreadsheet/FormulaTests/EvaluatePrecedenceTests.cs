using CS3500.Formula;

namespace FormulaTests
{
    /// <summary>
    /// Tests for the operator-precedence bug fix: Evaluate() previously only
    /// gave numeric literals the "immediately resolve * or / " treatment, not
    /// variables or (once added) function-call results, so e.g. A1*A2+A3
    /// computed the wrong answer while the equivalent 2*3+4 was correct.
    /// </summary>
    [TestClass]
    public class EvaluatePrecedenceTests
    {
        [TestMethod]
        public void MultiplyThenAdd_WithVariables_RespectsPrecedence()
        {
            // Confirmed bug: previously evaluated to 14 instead of the
            // correct (2*3)+4=10.
            var values = new Dictionary<string, double> { ["A1"] = 2, ["A2"] = 3, ["A3"] = 4 };
            var f = new Formula("A1*A2+A3");
            Assert.AreEqual(10.0, f.Evaluate(name => TestLookup.From(values, name)));
        }

        [TestMethod]
        public void DivideThenSubtract_WithVariables_RespectsPrecedence()
        {
            var values = new Dictionary<string, double> { ["A1"] = 10, ["A2"] = 2, ["A3"] = 1 };
            var f = new Formula("A1/A2-A3");
            // (10/2)-1 = 4
            Assert.AreEqual(4.0, f.Evaluate(name => TestLookup.From(values, name)));
        }

        [TestMethod]
        public void AddThenMultiply_WithVariables_RespectsPrecedence()
        {
            // Regression: the opposite order (low-then-high precedence)
            // already worked correctly before the fix; must keep working.
            var values = new Dictionary<string, double> { ["A1"] = 2, ["A2"] = 3, ["A3"] = 4 };
            var f = new Formula("A1+A2*A3");
            // 2+(3*4) = 14
            Assert.AreEqual(14.0, f.Evaluate(name => TestLookup.From(values, name)));
        }

        [TestMethod]
        public void MultiplyThenAdd_WithFunctionCallResult_RespectsPrecedence()
        {
            // Same bug class, now with a function call's pushed value.
            var values = new Dictionary<string, double> { ["A1"] = 2, ["A2"] = 3, ["A3"] = 1, ["A4"] = 5 };
            var f = new Formula("A1*SUM(A2,A3)+A4");
            // A1*(A2+A3) = 2*4=8, +A4(5) = 13
            Assert.AreEqual(13.0, f.Evaluate(name => TestLookup.From(values, name)));
        }

        [TestMethod]
        public void MultiplyThenAdd_WithNumericLiterals_StillCorrect()
        {
            // Regression: the already-correct numeric-literal case must be unaffected.
            var f = new Formula("2*3+4");
            Assert.AreEqual(10.0, f.Evaluate(_ => throw new ArgumentException()));
        }
    }
}
