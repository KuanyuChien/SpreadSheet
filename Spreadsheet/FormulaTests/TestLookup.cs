namespace FormulaTests
{
    /// <summary>
    /// Shared lookup helper for tests that need a Formula.Lookup delegate backed by
    /// a simple in-memory cell-name-to-value table.
    /// </summary>
    internal static class TestLookup
    {
        public static double From(Dictionary<string, double> values, string name)
        {
            if (values.TryGetValue(name, out double v)) { return v; }
            throw new ArgumentException($"Unknown cell {name}");
        }
    }
}
