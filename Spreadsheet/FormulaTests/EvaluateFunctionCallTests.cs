using CS3500.Formula;

namespace FormulaTests
{
    /// <summary>
    /// Tests for Formula.Evaluate() computing SUM/AVERAGE/MIN/MAX/COUNT over
    /// their range/cell arguments, and composing correctly with surrounding
    /// arithmetic.
    /// </summary>
    [TestClass]
    public class EvaluateFunctionCallTests
    {
        [TestMethod]
        public void Sum_OverRange_AddsAllCells()
        {
            var values = new Dictionary<string, double> { ["A1"] = 1, ["A2"] = 2, ["A3"] = 3 };
            var f = new Formula("SUM(A1:A3)");
            Assert.AreEqual(6.0, f.Evaluate(name => TestLookup.From(values, name)));
        }

        [TestMethod]
        public void Sum_OverMultipleCellArguments_AddsAllCells()
        {
            var values = new Dictionary<string, double> { ["A1"] = 1, ["A3"] = 3, ["A5"] = 5 };
            var f = new Formula("SUM(A1,A3,A5)");
            Assert.AreEqual(9.0, f.Evaluate(name => TestLookup.From(values, name)));
        }

        [TestMethod]
        public void Sum_OverMixedRangeAndCell_AddsAllCells()
        {
            var values = new Dictionary<string, double> { ["A1"] = 1, ["A2"] = 2, ["B5"] = 10 };
            var f = new Formula("SUM(A1:A2,B5)");
            Assert.AreEqual(13.0, f.Evaluate(name => TestLookup.From(values, name)));
        }

        [TestMethod]
        public void Average_OverRange_ComputesMean()
        {
            var values = new Dictionary<string, double> { ["A1"] = 2, ["A2"] = 4, ["A3"] = 6 };
            var f = new Formula("AVERAGE(A1:A3)");
            Assert.AreEqual(4.0, f.Evaluate(name => TestLookup.From(values, name)));
        }

        [TestMethod]
        public void Min_OverRange_ReturnsSmallestValue()
        {
            var values = new Dictionary<string, double> { ["A1"] = 5, ["A2"] = 1, ["A3"] = 9 };
            var f = new Formula("MIN(A1:A3)");
            Assert.AreEqual(1.0, f.Evaluate(name => TestLookup.From(values, name)));
        }

        [TestMethod]
        public void Max_OverRange_ReturnsLargestValue()
        {
            var values = new Dictionary<string, double> { ["A1"] = 5, ["A2"] = 1, ["A3"] = 9 };
            var f = new Formula("MAX(A1:A3)");
            Assert.AreEqual(9.0, f.Evaluate(name => TestLookup.From(values, name)));
        }

        [TestMethod]
        public void Count_OverRange_ReturnsNumberOfCells()
        {
            var values = new Dictionary<string, double> { ["A1"] = 5, ["A2"] = 1, ["A3"] = 9 };
            var f = new Formula("COUNT(A1:A3)");
            Assert.AreEqual(3.0, f.Evaluate(name => TestLookup.From(values, name)));
        }

        [TestMethod]
        public void FunctionCallResult_ComposesWithSurroundingArithmetic()
        {
            var values = new Dictionary<string, double> { ["A1"] = 1, ["A2"] = 2, ["A3"] = 3 };
            var f = new Formula("A1+3+AVERAGE(A2,A3)");
            // A1=1, +3 => 4, AVERAGE(2,3)=2.5, total 6.5
            Assert.AreEqual(6.5, f.Evaluate(name => TestLookup.From(values, name)));
        }

        [TestMethod]
        public void TwoFunctionCallsCombinedByOperator_BothEvaluate()
        {
            var values = new Dictionary<string, double> { ["A1"] = 1, ["A2"] = 2, ["B1"] = 10, ["B2"] = 20 };
            var f = new Formula("SUM(A1:A2)+MIN(B1,B2)");
            // SUM=3, MIN=10, total 13
            Assert.AreEqual(13.0, f.Evaluate(name => TestLookup.From(values, name)));
        }

        [TestMethod]
        public void FunctionCallInsideGroupingParens_ComposesCorrectly()
        {
            var values = new Dictionary<string, double> { ["A1"] = 2, ["A2"] = 3 };
            var f = new Formula("(SUM(A1:A2))*2");
            // SUM=5, *2 = 10
            Assert.AreEqual(10.0, f.Evaluate(name => TestLookup.From(values, name)));
        }

        [TestMethod]
        public void MissingCellInRange_ReturnsFormulaError()
        {
            // A2 intentionally absent from the lookup table.
            var values = new Dictionary<string, double> { ["A1"] = 1, ["A3"] = 3 };
            var f = new Formula("SUM(A1:A3)");
            object result = f.Evaluate(name => TestLookup.From(values, name));
            Assert.IsInstanceOfType(result, typeof(FormulaError));
        }

        [TestMethod]
        public void NonNumericCellInRange_ReturnsFormulaError()
        {
            // Simulates a text-cell lookup throwing, same contract Spreadsheet's
            // VariableLookup already uses for non-double cell values.
            double LookupWithTextCell(string name)
            {
                return name switch
                {
                    "A1" => 1,
                    "A2" => throw new ArgumentException("String cannot be evaluated"),
                    "A3" => 3,
                    _ => throw new ArgumentException("Unknown cell"),
                };
            }

            var f = new Formula("SUM(A1:A3)");
            object result = f.Evaluate(LookupWithTextCell);
            Assert.IsInstanceOfType(result, typeof(FormulaError));
        }
    }
}
