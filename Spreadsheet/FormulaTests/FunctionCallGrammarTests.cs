using CS3500.Formula;

namespace FormulaTests
{
    /// <summary>
    /// Tests for the Formula constructor accepting/rejecting function-call
    /// syntax: FUNCNAME "(" arg ("," arg)* ")" where arg := range | cell.
    /// No bare numbers as arguments, no nested function calls.
    /// </summary>
    [TestClass]
    public class FunctionCallGrammarTests
    {
        // ---- Valid formulas: constructing must NOT throw ----

        [TestMethod]
        public void SingleCellArgument_DoesNotThrow()
        {
            _ = new Formula("SUM(A1)");
        }

        [TestMethod]
        public void SingleRangeArgument_DoesNotThrow()
        {
            _ = new Formula("SUM(A1:A3)");
        }

        [TestMethod]
        public void MultipleCellArguments_DoesNotThrow()
        {
            _ = new Formula("SUM(A1,A3,A5)");
        }

        [TestMethod]
        public void MultipleRangeArguments_DoesNotThrow()
        {
            _ = new Formula("AVERAGE(A1:A3,B1:B3)");
        }

        [TestMethod]
        public void MixedRangeAndCellArguments_DoesNotThrow()
        {
            _ = new Formula("SUM(A1:A3,B5)");
        }

        [TestMethod]
        public void FunctionCallFollowedByArithmetic_DoesNotThrow()
        {
            _ = new Formula("SUM(A1:A3)+5");
        }

        [TestMethod]
        public void ArithmeticFollowedByFunctionCall_DoesNotThrow()
        {
            _ = new Formula("5+SUM(A1:A3)");
        }

        [TestMethod]
        public void FunctionCallWrappedInGroupingParens_DoesNotThrow()
        {
            _ = new Formula("(SUM(A1:A3))");
        }

        [TestMethod]
        public void TwoSeparateFunctionCallsCombinedByOperator_DoesNotThrow()
        {
            _ = new Formula("MIN(A1:A3)+MAX(B1:B3)");
        }

        [TestMethod]
        public void LowercaseFunctionNameAndCells_DoesNotThrow()
        {
            _ = new Formula("sum(a1:a3)");
        }

        [TestMethod]
        public void Average_DoesNotThrow()
        {
            _ = new Formula("AVERAGE(A1)");
        }

        [TestMethod]
        public void Min_DoesNotThrow()
        {
            _ = new Formula("MIN(A1)");
        }

        [TestMethod]
        public void Max_DoesNotThrow()
        {
            _ = new Formula("MAX(A1)");
        }

        [TestMethod]
        public void Count_DoesNotThrow()
        {
            _ = new Formula("COUNT(A1)");
        }

        [TestMethod]
        public void ToString_NormalizesFunctionCallCaseAndRemovesNoExtraCharacters()
        {
            Assert.AreEqual("SUM(A1:A3)", new Formula("sum(a1:a3)").ToString());
        }

        // ---- Invalid formulas: constructing must throw FormulaFormatException ----

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BareFunctionNameWithNoParens_Throws()
        {
            _ = new Formula("SUM");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FunctionNameFollowedByOperatorInsteadOfParen_Throws()
        {
            _ = new Formula("SUM+5");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void EmptyArgumentList_Throws()
        {
            _ = new Formula("SUM()");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TrailingComma_Throws()
        {
            _ = new Formula("SUM(A1:A3,)");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void LeadingComma_Throws()
        {
            _ = new Formula("SUM(,A1:A3)");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void DoubleCommaEmptyMiddleArgument_Throws()
        {
            _ = new Formula("SUM(A1:A3,,B1:B3)");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TwoArgumentsWithNoCommaBetweenThem_Throws()
        {
            _ = new Formula("SUM(A1 A3)");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BareNumberAsArgument_Throws()
        {
            _ = new Formula("SUM(A1:A3,5)");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void NestedFunctionCallAsArgument_Throws()
        {
            _ = new Formula("SUM(AVERAGE(A1:A3))");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BareRangeOutsideFunctionCall_Throws()
        {
            _ = new Formula("A1:A3");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BareCommaOutsideFunctionCall_Throws()
        {
            _ = new Formula("A1,A3");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void MissingClosingParen_Throws()
        {
            _ = new Formula("SUM(A1:A3");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ExtraUnmatchedClosingParen_Throws()
        {
            _ = new Formula("SUM(A1:A3))");
        }
    }
}
