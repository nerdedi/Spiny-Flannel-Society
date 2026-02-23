using UnityEngine;

namespace SFS.UI
{
    /// <summary>
    /// Lightweight placeholder for user interface helpers used by animation
    /// scripts.  Eventually this will drive the in-game overlay that explains
    /// what default assumptions have been read or rewritten.  For now it merely
    /// logs to the console so the feature is wired and ready for extension.
    /// </summary>
    public static class UIManager
    {
        /// <summary>
        /// Display an overlay describing a default that has just been read.
        /// </summary>
        /// <param name="description">Text to show in the overlay.</param>
        public static void ShowDefaultOverlay(string description)
        {
            // TODO: replace this debug log with actual UI presentation logic.
            Debug.Log($"[UI] Default overlay: {description}");
        }
    }
}
