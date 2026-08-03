using CS3500.Formula;

namespace FormulaTests
{
    /// <summary>
    /// Tests for Formula.GetTokens recognizing "A1:B3"-style range syntax as a
    /// single token, rather than splitting on the colon.
    /// </summary>
    [TestClass]
    public class RangeTokenizingTests
    {
        [TestMethod]
        public void GetTokens_SimpleRange_ReturnsRangeAsSingleToken()
        {
            CollectionAssert.AreEqual(new[] { "A1:B3" }, Formula.GetTokens("A1:B3"));
        }

        [TestMethod]
        public void GetTokens_RangeFollowedByOperatorAndVariable_TokenizesRangeSeparately()
        {
            CollectionAssert.AreEqual(new[] { "A1:B3", "+", "C1" }, Formula.GetTokens("A1:B3+C1"));
        }

        [TestMethod]
        public void GetTokens_RangePrecededByNumberAndOperator_TokenizesRangeSeparately()
        {
            CollectionAssert.AreEqual(new[] { "2", "+", "A1:B3" }, Formula.GetTokens("2+A1:B3"));
        }

        [TestMethod]
        public void GetTokens_RangeInParentheses_KeepsParensAndRangeDistinct()
        {
            CollectionAssert.AreEqual(new[] { "(", "A1:B3", ")" }, Formula.GetTokens("(A1:B3)"));
        }

        [TestMethod]
        public void GetTokens_RangeWithMultiLetterColumns_ReturnsWholeRangeAsOneToken()
        {
            CollectionAssert.AreEqual(new[] { "AA1:AB2" }, Formula.GetTokens("AA1:AB2"));
        }

        [TestMethod]
        public void GetTokens_MalformedDoubleColonRange_OnlyFirstSegmentBecomesRangeToken()
        {
            // A1:B3:C1 has two colons -- only the first well-formed
            // "cell:cell" segment becomes a range token; the leftover ":" is
            // a stray token, left for the constructor's existing invalid-token
            // rule to reject later (not this function's job to validate).
            CollectionAssert.AreEqual(new[] { "A1:B3", ":", "C1" }, Formula.GetTokens("A1:B3:C1"));
        }

        [TestMethod]
        public void GetTokens_PlainVariable_StillTokenizesAsBefore()
        {
            // Regression: adding the range pattern must not change how a
            // plain single-cell variable tokenizes.
            CollectionAssert.AreEqual(new[] { "A1" }, Formula.GetTokens("A1"));
        }

        [TestMethod]
        public void GetTokens_TwoPlainVariablesWithOperator_DoesNotAccidentallyMergeIntoRange()
        {
            // Regression: two ordinary variables separated by an operator
            // (no colon) must stay as separate tokens, not get merged.
            CollectionAssert.AreEqual(new[] { "A1", "+", "B3" }, Formula.GetTokens("A1+B3"));
        }

        [TestMethod]
        public void GetTokens_ColonWithNoLeftOperand_DoesNotProduceRangeToken()
        {
            CollectionAssert.AreEqual(new[] { ":", "B3" }, Formula.GetTokens(":B3"));
        }

        [TestMethod]
        public void GetTokens_ColonWithNoRightOperand_DoesNotProduceRangeToken()
        {
            CollectionAssert.AreEqual(new[] { "A1", ":" }, Formula.GetTokens("A1:"));
        }

        [TestMethod]
        public void GetTokens_NumberColonVariable_DoesNotProduceRangeToken()
        {
            // A range must be cell:cell, not number:cell.
            CollectionAssert.AreEqual(new[] { "5", ":", "A1" }, Formula.GetTokens("5:A1"));
        }

        [TestMethod]
        public void GetTokens_VariableColonNumber_DoesNotProduceRangeToken()
        {
            CollectionAssert.AreEqual(new[] { "A1", ":", "5" }, Formula.GetTokens("A1:5"));
        }

        [TestMethod]
        public void GetTokens_DoubleColon_DoesNotProduceRangeToken()
        {
            CollectionAssert.AreEqual(new[] { "A1", "::", "B3" }, Formula.GetTokens("A1::B3"));
        }

        [TestMethod]
        public void GetTokens_WhitespaceAroundColon_BreaksRangeRecognition()
        {
            // Spaces delimit tokens, same as they would for a plain variable
            // ("x 1" is never a single variable "x1"). "A1 : B3" must not
            // become one range token.
            CollectionAssert.AreEqual(new[] { "A1", ":", "B3" }, Formula.GetTokens("A1 : B3"));
        }

        [TestMethod]
        public void GetTokens_TwoSeparateRangesInOneFormula_BothTokenizeCorrectly()
        {
            CollectionAssert.AreEqual(
                new[] { "A1:B3", "+", "C1:D3" },
                Formula.GetTokens("A1:B3+C1:D3"));
        }

        [TestMethod]
        public void GetTokens_LowercaseRange_CasePreservedAtThisLayer()
        {
            // Case normalization happens later in the constructor's main
            // token loop, not in GetTokens.
            CollectionAssert.AreEqual(new[] { "a1:b3" }, Formula.GetTokens("a1:b3"));
        }
    }
}
