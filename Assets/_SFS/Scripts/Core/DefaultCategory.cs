namespace SFS.Core
{
    /// <summary>
    /// Which aspect of the Society a default governs.
    /// Maps directly from the Python prototype's DefaultCategory enum.
    /// </summary>
    public enum DefaultCategory
    {
        Timing,      // Speed expectations, window widths
        Sensory,     // Visual density, audio load, clutter
        Routing,     // Path strictness, alternatives
        Social,      // Communication norms, expression modes
        Failure,     // Penalty severity, retry cost
        Consent      // Gate presence, opt-in/opt-out
    }
}
