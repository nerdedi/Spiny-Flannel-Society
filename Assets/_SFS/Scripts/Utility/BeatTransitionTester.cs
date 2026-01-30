using UnityEngine;
using SFS.Core;

namespace SFS.Utility
{
    /// <summary>
    /// Runtime utility to manually trigger beat transitions with number keys.
    /// Useful for testing without walking through the level.
    /// </summary>
    public class BeatTransitionTester : MonoBehaviour
    {
        [Header("Settings")]
        public bool enableNumberKeys = true;

        [Tooltip("Require holding this key + number")]
        public bool requireModifier = true;
        public KeyCode modifierKey = KeyCode.LeftShift;

        void Update()
        {
            if (!enableNumberKeys) return;
            if (!StoryBeatManager.Instance) return;

            bool modifierHeld = !requireModifier || Input.GetKey(modifierKey);
            if (!modifierHeld) return;

            // 1-7 for beats
            if (Input.GetKeyDown(KeyCode.Alpha1))
                TransitionTo(StoryBeat.Arrival);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                TransitionTo(StoryBeat.FirstContact);
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                TransitionTo(StoryBeat.Compression);
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                TransitionTo(StoryBeat.ChoosingRest);
            else if (Input.GetKeyDown(KeyCode.Alpha5))
                TransitionTo(StoryBeat.TheSociety);
            else if (Input.GetKeyDown(KeyCode.Alpha6))
                TransitionTo(StoryBeat.SharedDifficulty);
            else if (Input.GetKeyDown(KeyCode.Alpha7))
                TransitionTo(StoryBeat.QuietBelonging);
        }

        void TransitionTo(StoryBeat beat)
        {
            Debug.Log($"[BeatTester] Forcing transition to: {beat}");
            StoryBeatManager.Instance.TransitionTo(beat);
        }
    }
}
