using CS3500.Formula;

namespace FormulaTests
{
    /// <summary>
    /// Tests for Formula.GetTokens recognizing function names (SUM, AVERAGE,
    /// MIN, MAX, COUNT) and commas as their own tokens.
    /// </summary>
    [TestClass]
    public class FunctionNameTokenizingTests
    {
        [TestMethod]
        public void GetTokens_Sum_RecognizedAsFunctionNameToken()
        {
            CollectionAssert.AreEqual(new[] { "SUM", "(", "A1", ")" }, Formula.GetTokens("SUM(A1)"));
        }

        [TestMethod]
        public void GetTokens_Average_RecognizedAsFunctionNameToken()
        {
            CollectionAssert.AreEqual(new[] { "AVERAGE", "(", "A1", ")" }, Formula.GetTokens("AVERAGE(A1)"));
        }

        [TestMethod]
        public void GetTokens_Min_RecognizedAsFunctionNameToken()
        {
            CollectionAssert.AreEqual(new[] { "MIN", "(", "A1", ")" }, Formula.GetTokens("MIN(A1)"));
        }

        [TestMethod]
        public void GetTokens_Max_RecognizedAsFunctionNameToken()
        {
            CollectionAssert.AreEqual(new[] { "MAX", "(", "A1", ")" }, Formula.GetTokens("MAX(A1)"));
        }

        [TestMethod]
        public void GetTokens_Count_RecognizedAsFunctionNameToken()
        {
            CollectionAssert.AreEqual(new[] { "COUNT", "(", "A1", ")" }, Formula.GetTokens("COUNT(A1)"));
        }

        [TestMethod]
        public void GetTokens_FunctionCallWithRangeArgument_TokenizesNameParensAndRangeSeparately()
        {
            CollectionAssert.AreEqual(
                new[] { "SUM", "(", "A1:A3", ")" },
                Formula.GetTokens("SUM(A1:A3)"));
        }

        [TestMethod]
        public void GetTokens_FunctionCallWithMultipleCellArguments_CommasAreSeparateTokens()
        {
            CollectionAssert.AreEqual(
                new[] { "SUM", "(", "A1", ",", "A3", ",", "A5", ")" },
                Formula.GetTokens("SUM(A1,A3,A5)"));
        }

        [TestMethod]
        public void GetTokens_FunctionCallMixingRangeAndCellArguments_TokenizesEachArgumentSeparately()
        {
            CollectionAssert.AreEqual(
                new[] { "AVERAGE", "(", "A1:A3", ",", "B1:B3", ")" },
                Formula.GetTokens("AVERAGE(A1:A3,B1:B3)"));
        }

        [TestMethod]
        public void GetTokens_FunctionNameLowercase_CasePreservedAtThisLayer()
        {
            // Case normalization happens later in the constructor's main
            // token loop, not in GetTokens (same principle as ranges).
            CollectionAssert.AreEqual(new[] { "sum", "(", "A1", ")" }, Formula.GetTokens("sum(A1)"));
        }

        [TestMethod]
        public void GetTokens_FunctionNameWithSpaceBeforeParen_StillTokenizesCorrectly()
        {
            CollectionAssert.AreEqual(new[] { "SUM", "(", "A1", ")" }, Formula.GetTokens("SUM (A1)"));
        }

        [TestMethod]
        public void GetTokens_CellNameStartingWithFunctionKeywordFollowedByDigit_TokenizesAsOneVariable()
        {
            // Critical: "SUM1" must be one variable token, not "SUM" + "1".
            // The lookahead must block a trailing digit, not just a letter.
            CollectionAssert.AreEqual(new[] { "SUM1" }, Formula.GetTokens("SUM1"));
        }

        [TestMethod]
        public void GetTokens_KeywordFollowedByMoreLetters_NotTreatedAsFunctionName()
        {
            // "SUMX" is all letters, no digits: not a valid variable, and not
            // a function name either (lookahead blocks a trailing letter).
            // It should fall through as one unrecognized stray token, same as
            // any other unrecognized word (e.g. "hello") does today.
            CollectionAssert.AreEqual(new[] { "SUMX" }, Formula.GetTokens("SUMX"));
        }

        [TestMethod]
        public void GetTokens_AverageFollowedByDigit_TokenizesAsOneVariable()
        {
            // Same guard, exercised on the longest keyword.
            CollectionAssert.AreEqual(new[] { "AVERAGE1" }, Formula.GetTokens("AVERAGE1"));
        }

        [TestMethod]
        public void GetTokens_AverageFollowedByMoreLetters_NotTreatedAsFunctionName()
        {
            CollectionAssert.AreEqual(new[] { "AVERAGEX" }, Formula.GetTokens("AVERAGEX"));
        }

        [TestMethod]
        public void GetTokens_CommaOutsideAnyFunctionCall_StillTokenizesAsCommaToken()
        {
            // GetTokens only recognizes shape, not grammar/context -- whether
            // a bare comma like this is actually valid is a later concern.
            CollectionAssert.AreEqual(new[] { "A1", ",", "B1" }, Formula.GetTokens("A1,B1"));
        }

        [TestMethod]
        public void GetTokens_ConsecutiveCommas_TokenizeAsTwoSeparateCommaTokens()
        {
            // The one case that actually distinguishes "comma has its own
            // dedicated pattern" from "comma just happens to fall through as
            // leftover text": two unrecognized characters directly adjacent
            // to each other merge into one leftover chunk unless each has
            // its own pattern. Without a dedicated comma pattern, ",," would
            // wrongly become a single two-character token.
            CollectionAssert.AreEqual(
                new[] { "SUM", "(", "A1", ",", ",", "A3", ")" },
                Formula.GetTokens("SUM(A1,,A3)"));
        }

        [TestMethod]
        public void GetTokens_PlainVariable_StillTokenizesAsBefore()
        {
            // Regression: adding function-name/comma patterns must not
            // change how a plain single-cell variable tokenizes.
            CollectionAssert.AreEqual(new[] { "A1" }, Formula.GetTokens("A1"));
        }
    }
}
