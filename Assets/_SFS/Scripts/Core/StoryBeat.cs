using System;

namespace SFS.Core
{
    /// <summary>
    /// Defines the 7 story beats of the Spiny Flannel Society experience.
    /// Each beat has distinct emotional, visual, and mechanical qualities.
    /// </summary>
    public enum StoryBeat
    {
        /// <summary>Soft Arrival (Gentle) - Welcome without pressure</summary>
        Arrival = 0,

        /// <summary>First Contact (Hopeful) - Interaction is safe</summary>
        FirstContact = 1,

        /// <summary>Compression (Melancholic) - Friction without punishment</summary>
        Compression = 2,

        /// <summary>Choosing Rest (Gentle) - Pause as agency</summary>
        ChoosingRest = 3,

        /// <summary>The Society (Hopeful + Tender) - Community without demand</summary>
        TheSociety = 4,

        /// <summary>Shared Difficulty (Melancholic → Hopeful) - Support changes experience</summary>
        SharedDifficulty = 5,

        /// <summary>Quiet Belonging (Grounded Hope) - Stability, not spectacle</summary>
        QuietBelonging = 6
    }

    /// <summary>
    /// Emotional tone for camera, audio, and animation systems to respond to.
    /// </summary>
    public enum EmotionalTone
    {
        Gentle,
        Hopeful,
        Melancholic,
        Grounded
    }

    public static class StoryBeatExtensions
    {
        public static EmotionalTone GetTone(this StoryBeat beat) => beat switch
        {
            StoryBeat.Arrival => EmotionalTone.Gentle,
            StoryBeat.FirstContact => EmotionalTone.Hopeful,
            StoryBeat.Compression => EmotionalTone.Melancholic,
            StoryBeat.ChoosingRest => EmotionalTone.Gentle,
            StoryBeat.TheSociety => EmotionalTone.Hopeful,
            StoryBeat.SharedDifficulty => EmotionalTone.Hopeful,
            StoryBeat.QuietBelonging => EmotionalTone.Grounded,
            _ => EmotionalTone.Gentle
        };

        public static float GetIdleIntensity(this StoryBeat beat) => beat switch
        {
            StoryBeat.Arrival => 0.3f,        // Subtle, uncertain
            StoryBeat.FirstContact => 0.4f,   // Slightly more present
            StoryBeat.Compression => 0.7f,    // Heightened, faster
            StoryBeat.ChoosingRest => 0.15f,  // Slowed, restful
            StoryBeat.TheSociety => 0.35f,    // Calm presence
            StoryBeat.SharedDifficulty => 0.5f, // Active but supported
            StoryBeat.QuietBelonging => 0.2f, // Settled, grounded
            _ => 0.3f
        };
    }
}
