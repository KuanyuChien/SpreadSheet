using CS3500.Formula;

namespace FormulaTests
{
    /// <summary>
    /// Tests for Formula.ExpandRange, the helper that turns a range's two corner
    /// cell names (e.g. "A1", "C3") into the list of individual cell names the
    /// range covers.
    /// </summary>
    [TestClass]
    public class RangeExpansionTests
    {
        private static void AssertSameCells(IEnumerable<string> expected, IEnumerable<string> actual)
        {
            CollectionAssert.AreEquivalent(expected.ToList(), actual.ToList());
        }

        [TestMethod]
        public void ExpandRange_SingleColumn_ReturnsAllRowsInColumn()
        {
            AssertSameCells(new[] { "A1", "A2", "A3" }, Formula.ExpandRange("A1", "A3"));
        }

        [TestMethod]
        public void ExpandRange_SingleRow_ReturnsAllColumnsInRow()
        {
            AssertSameCells(new[] { "A1", "B1", "C1" }, Formula.ExpandRange("A1", "C1"));
        }

        [TestMethod]
        public void ExpandRange_Rectangle_ReturnsAllCellsInRectangle()
        {
            AssertSameCells(new[] { "A1", "B1", "A2", "B2" }, Formula.ExpandRange("A1", "B2"));
        }

        [TestMethod]
        public void ExpandRange_ReversedCorners_SameResultAsForwardCorners()
        {
            IEnumerable<string> forward = Formula.ExpandRange("A1", "C3");
            IEnumerable<string> reversed = Formula.ExpandRange("C3", "A1");
            AssertSameCells(forward, reversed);
            AssertSameCells(
                new[] { "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3" },
                reversed);
        }

        [TestMethod]
        public void ExpandRange_MixedCorners_TopRightAndBottomLeft_StillFormsRectangle()
        {
            // "C1" and "A3" are the top-right/bottom-left corners of the same
            // rectangle as A1:C3.
            AssertSameCells(
                new[] { "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3" },
                Formula.ExpandRange("C1", "A3"));
        }

        [TestMethod]
        public void ExpandRange_DegenerateSingleCell_ReturnsThatCellOnly()
        {
            AssertSameCells(new[] { "A1" }, Formula.ExpandRange("A1", "A1"));
        }

        [TestMethod]
        public void ExpandRange_MultiLetterColumns_ConvertsColumnsCorrectly()
        {
            AssertSameCells(
                new[] { "AA1", "AB1", "AA2", "AB2" },
                Formula.ExpandRange("AA1", "AB2"));
        }

        [TestMethod]
        public void ExpandRange_ColumnBoundaryAtZ_HandlesZToAaTransition()
        {
            // Z is column 26, AA is column 27 -- make sure the range spanning
            // that boundary includes both and nothing extra.
            AssertSameCells(new[] { "Z1", "AA1" }, Formula.ExpandRange("Z1", "AA1"));
        }

        [TestMethod]
        public void ExpandRange_MultiDigitRows_ComparesRowsNumericallyNotLexicographically()
        {
            // If rows were compared as strings instead of ints, "9" > "30"
            // lexicographically and this range would come out wrong/truncated.
            AssertSameCells(new[] { "A9", "A10", "A11" }, Formula.ExpandRange("A9", "A11"));
        }

        [TestMethod]
        public void ExpandRange_ReversedCornersWithMultiDigitRowBoundary_FormsFullRectangle()
        {
            // Combines a reversed-corner range with a multi-digit row boundary:
            // C3:A30 should cover columns A-C and rows 3-30.
            List<string> expected = new();
            string[] cols = { "A", "B", "C" };
            for (int row = 3; row <= 30; row++)
            {
                foreach (string col in cols)
                {
                    expected.Add(col + row);
                }
            }

            AssertSameCells(expected, Formula.ExpandRange("C3", "A30"));
        }

        [TestMethod]
        public void ExpandRange_ColumnCarryBeyondFirstLetter_HandlesAzToBaTransition()
        {
            // AZ is column 52, BA is column 53 -- unlike Z->AA, this carry
            // happens in the *second* letter position, exercising the general
            // base-26 conversion rather than a Z/AA special case.
            AssertSameCells(new[] { "AZ1", "BA1" }, Formula.ExpandRange("AZ1", "BA1"));
        }

        [TestMethod]
        public void ExpandRange_RowDigitCountBoundary_HandlesTwoToThreeDigitTransition()
        {
            AssertSameCells(new[] { "A99", "A100" }, Formula.ExpandRange("A99", "A100"));
        }

        [TestMethod]
        public void ExpandRange_MediumRectangleAtColumn50Boundary_Returns15Cells()
        {
            // AV=48, AW=49, AX=50 -- AX is the 50th column, i.e. the last
            // column of the upcoming 50x50 grid. 3 columns x 5 rows = 15 cells.
            AssertSameCells(
                new[]
                {
                    "AV1", "AW1", "AX1",
                    "AV2", "AW2", "AX2",
                    "AV3", "AW3", "AX3",
                    "AV4", "AW4", "AX4",
                    "AV5", "AW5", "AX5",
                },
                Formula.ExpandRange("AV1", "AX5"));
        }
    }
}
