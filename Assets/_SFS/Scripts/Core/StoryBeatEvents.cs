using System;
using UnityEngine;

namespace SFS.Core
{
    /// <summary>
    /// Event bus specifically for story beat transitions.
    /// Keeps narrative flow decoupled from gameplay systems.
    /// </summary>
    public static class StoryBeatEvents
    {
        /// <summary>Fired when player enters a new story beat zone</summary>
        public static event Action<StoryBeat, StoryBeat> OnBeatChanged; // (previous, current)

        /// <summary>Fired when emotional tone shifts (for audio/camera)</summary>
        public static event Action<EmotionalTone> OnToneChanged;

        /// <summary>Fired when player enters a rest zone</summary>
        public static event Action OnRestZoneEntered;

        /// <summary>Fired when player leaves a rest zone</summary>
        public static event Action OnRestZoneExited;

        /// <summary>Fired when Society members become visible/present</summary>
        public static event Action OnSocietyRevealed;

        /// <summary>Fired when companion joins player</summary>
        public static event Action<Transform> OnCompanionJoined;

        /// <summary>Fired when companion leaves</summary>
        public static event Action OnCompanionLeft;

        public static void BeatChanged(StoryBeat previous, StoryBeat current)
        {
            OnBeatChanged?.Invoke(previous, current);
            OnToneChanged?.Invoke(current.GetTone());
        }

        public static void RestZoneEntered() => OnRestZoneEntered?.Invoke();
        public static void RestZoneExited() => OnRestZoneExited?.Invoke();
        public static void SocietyRevealed() => OnSocietyRevealed?.Invoke();
        public static void CompanionJoined(Transform companion) => OnCompanionJoined?.Invoke(companion);
        public static void CompanionLeft() => OnCompanionLeft?.Invoke();
    }
}
