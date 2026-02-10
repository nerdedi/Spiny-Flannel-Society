using System;

namespace SFS.Core
{
    /// <summary>
    /// A single rewritable default in the Society.
    ///
    /// The Society imposed rigid defaults on everyone.
    /// Each default encodes an assumption about how people should function.
    /// The player discovers these (Read) and changes them (Rewrite).
    ///
    /// Values are floats: rigid_value is the punishing original,
    /// rewritten_value is the generous replacement.
    /// current_value starts at rigid and becomes rewritten after Rewrite.
    /// </summary>
    [Serializable]
    public class Default
    {
        public string Key;
        public string Label;
        public string Description;
        public DefaultCategory Category;

        public float RigidValue;
        public float RewrittenValue;
        public float CurrentValue;

        public bool IsRead;
        public bool IsRewritten;

        public Default(string key, string label, string description,
                       DefaultCategory category, float rigidValue, float rewrittenValue)
        {
            Key = key;
            Label = label;
            Description = description;
            Category = category;
            RigidValue = rigidValue;
            RewrittenValue = rewrittenValue;
            CurrentValue = rigidValue;
            IsRead = false;
            IsRewritten = false;
        }

        /// <summary>
        /// Player uses Read Default. Reveals the assumption.
        /// Returns a formatted description string.
        /// </summary>
        public string Read()
        {
            IsRead = true;
            return $"[{Label}]\n  Assumption: {Description}\n  Current value: {CurrentValue}\n  Rigid default: {RigidValue}";
        }

        /// <summary>
        /// Player uses Rewrite Default.
        /// Can only rewrite after reading. Returns true on success.
        /// </summary>
        public bool Rewrite()
        {
            if (!IsRead) return false;
            IsRewritten = true;
            CurrentValue = RewrittenValue;
            return true;
        }

        /// <summary>Reset to rigid state (for new game).</summary>
        public void Reset()
        {
            CurrentValue = RigidValue;
            IsRead = false;
            IsRewritten = false;
        }
    }
}
