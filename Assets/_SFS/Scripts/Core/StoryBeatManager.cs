using UnityEngine;

namespace SFS.Core
{
    /// <summary>
    /// Singleton that tracks current story beat and manages transitions.
    /// Systems query this to know current emotional context.
    /// </summary>
    public class StoryBeatManager : MonoBehaviour
    {
        public static StoryBeatManager Instance { get; private set; }

        [Header("Current State")]
        [SerializeField] private StoryBeat currentBeat = StoryBeat.Arrival;

        [Header("Debug")]
        [SerializeField] private bool logTransitions = true;

        public StoryBeat CurrentBeat => currentBeat;
        public EmotionalTone CurrentTone => currentBeat.GetTone();
        public float CurrentIdleIntensity => currentBeat.GetIdleIntensity();

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        /// <summary>
        /// Called by zone triggers when player enters a new beat area.
        /// </summary>
        public void TransitionTo(StoryBeat newBeat)
        {
            if (newBeat == currentBeat) return;

            var previous = currentBeat;
            currentBeat = newBeat;

            if (logTransitions)
            {
                Debug.Log($"[Story] Beat transition: {previous} → {newBeat} (Tone: {newBeat.GetTone()})");
            }

            StoryBeatEvents.BeatChanged(previous, newBeat);
        }

        /// <summary>
        /// Force set beat without transition logic (for scene load).
        /// </summary>
        public void SetBeatImmediate(StoryBeat beat)
        {
            currentBeat = beat;
        }
    }
}
