using CS3500.Formula;
using CS3500.Spreadsheet;

namespace SpreadsheetTests
{
    /// <summary>
    /// Spreadsheet-level integration tests for range references and
    /// aggregate functions: recalculation when a cell inside a range
    /// changes, and circular-dependency detection through a range.
    /// </summary>
    [TestClass]
    public class RangeFunctionTests
    {
        [TestMethod]
        public void Sum_OverRange_ComputesCorrectValue()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "1");
            s.SetContentsOfCell("A2", "2");
            s.SetContentsOfCell("A3", "3");
            s.SetContentsOfCell("B1", "=SUM(A1:A3)");
            Assert.AreEqual(6.0, s.GetCellValue("B1"));
        }

        [TestMethod]
        public void Sum_RecalculatesWhenACellInsideTheRangeChanges()
        {
            // This only works if GetVariables() actually expands the range
            // into individual cells for the dependency graph -- otherwise
            // B1 would never be told it depends on A1/A2/A3 at all.
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "1");
            s.SetContentsOfCell("A2", "2");
            s.SetContentsOfCell("A3", "3");
            s.SetContentsOfCell("B1", "=SUM(A1:A3)");
            Assert.AreEqual(6.0, s.GetCellValue("B1"));

            s.SetContentsOfCell("A2", "20");
            Assert.AreEqual(24.0, s.GetCellValue("B1")); // 1+20+3
        }

        [TestMethod]
        public void SetContentsOfCell_RecalculationListIncludesRangeDependentCell()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "1");
            s.SetContentsOfCell("A2", "2");
            s.SetContentsOfCell("A3", "3");
            s.SetContentsOfCell("B1", "=SUM(A1:A3)");

            IList<string> changed = s.SetContentsOfCell("A2", "5");
            CollectionAssert.Contains(changed.ToList(), "A2");
            CollectionAssert.Contains(changed.ToList(), "B1");
        }

        [TestMethod]
        public void CircularDependencyThroughRange_IsDetected()
        {
            // B1 depends on A1:A3 (via the range). If A3 is then made to
            // depend on B1, that's circular -- but only catchable if the
            // dependency graph actually knows B1 depends on A3.
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "1");
            s.SetContentsOfCell("A2", "2");
            s.SetContentsOfCell("A3", "3");
            s.SetContentsOfCell("B1", "=SUM(A1:A3)");

            Assert.ThrowsException<CircularException>(() => s.SetContentsOfCell("A3", "=B1"));
        }

        [TestMethod]
        public void MissingCellInRange_ProducesFormulaError()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "1");
            // A2 intentionally left unset.
            s.SetContentsOfCell("A3", "3");
            s.SetContentsOfCell("B1", "=SUM(A1:A3)");
            Assert.IsInstanceOfType(s.GetCellValue("B1"), typeof(FormulaError));
        }

        [TestMethod]
        public void TextCellInRange_ProducesFormulaError()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "1");
            s.SetContentsOfCell("A2", "hello");
            s.SetContentsOfCell("A3", "3");
            s.SetContentsOfCell("B1", "=SUM(A1:A3)");
            Assert.IsInstanceOfType(s.GetCellValue("B1"), typeof(FormulaError));
        }

        [TestMethod]
        public void PlainVariablePlusNumberPlusAverageOfRange_ComposesCorrectly()
        {
            // The end-to-end scenario: A1 + 3 + AVERAGE(...) all in one formula.
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "1");
            s.SetContentsOfCell("A2", "2");
            s.SetContentsOfCell("A3", "9");
            s.SetContentsOfCell("B1", "=A1+3+AVERAGE(A2,A3)");
            // A1=1, +3=4, AVERAGE(2,9)=5.5, total = 9.5
            Assert.AreEqual(9.5, s.GetCellValue("B1"));
        }
    }
}
