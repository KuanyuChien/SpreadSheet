using CS3500.Formula;

namespace FormulaTests
{
    /// <summary>
    /// Tests for Formula.GetVariables() expanding ranges into their
    /// individual cells, so the dependency graph sees every cell a
    /// range/function-call formula actually depends on.
    /// </summary>
    [TestClass]
    public class GetVariablesRangeTests
    {
        [TestMethod]
        public void RangeInsideFunctionCall_ExpandsToAllCellsInRange()
        {
            ISet<string> variables = new Formula("SUM(A1:A3)").GetVariables();
            CollectionAssert.AreEquivalent(new[] { "A1", "A2", "A3" }, variables.ToList());
        }

        [TestMethod]
        public void MultipleCellArguments_IncludesEachCell()
        {
            ISet<string> variables = new Formula("SUM(A1,A3,A5)").GetVariables();
            CollectionAssert.AreEquivalent(new[] { "A1", "A3", "A5" }, variables.ToList());
        }

        [TestMethod]
        public void MixedRangeAndCellArguments_IncludesAllCells()
        {
            ISet<string> variables = new Formula("SUM(A1:A2,B5)").GetVariables();
            CollectionAssert.AreEquivalent(new[] { "A1", "A2", "B5" }, variables.ToList());
        }

        [TestMethod]
        public void PlainVariableOutsideFunctionCall_StillWorks()
        {
            // Regression: adding range expansion must not change plain-variable behavior.
            ISet<string> variables = new Formula("A1+B1").GetVariables();
            CollectionAssert.AreEquivalent(new[] { "A1", "B1" }, variables.ToList());
        }

        [TestMethod]
        public void PlainVariableCombinedWithFunctionCall_IncludesBoth()
        {
            ISet<string> variables = new Formula("A1+SUM(B1:B3)").GetVariables();
            CollectionAssert.AreEquivalent(new[] { "A1", "B1", "B2", "B3" }, variables.ToList());
        }

        [TestMethod]
        public void CellReferencedBothDirectlyAndInsideRange_NotDuplicated()
        {
            ISet<string> variables = new Formula("A1+SUM(A1:A3)").GetVariables();
            Assert.AreEqual(3, variables.Count);
            CollectionAssert.AreEquivalent(new[] { "A1", "A2", "A3" }, variables.ToList());
        }

        [TestMethod]
        public void TwoOverlappingRanges_NoDuplicateCells()
        {
            ISet<string> variables = new Formula("SUM(A1:A3,A2:A4)").GetVariables();
            CollectionAssert.AreEquivalent(new[] { "A1", "A2", "A3", "A4" }, variables.ToList());
        }

        [TestMethod]
        public void FunctionNameToken_NeverIncludedAsAVariable()
        {
            ISet<string> variables = new Formula("SUM(A1:A3)").GetVariables();
            Assert.IsFalse(variables.Contains("SUM"));
        }
    }
}
