using UnityEngine;
using UnityEngine.Playables;
using SFS.Core;

namespace SFS.World
{
    /// <summary>
    /// Trigger volume that transitions the player into a specific story beat.
    /// Can optionally trigger a Timeline cutscene on entry.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class StoryBeatZone : MonoBehaviour
    {
        [Header("Beat Configuration")]
        public StoryBeat beat;

        [Header("Optional Cutscene")]
        [Tooltip("If assigned, plays when entering this zone")]
        public PlayableDirector entryCutscene;
        public bool playCutsceneOnce = true;

        [Header("Visual Feedback")]
        [Tooltip("Objects to enable when this beat is active")]
        public GameObject[] activateOnEnter;
        [Tooltip("Objects to disable when this beat is active")]
        public GameObject[] deactivateOnEnter;

        bool cutscenePlayed;

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            // Transition story beat
            if (StoryBeatManager.Instance)
            {
                StoryBeatManager.Instance.TransitionTo(beat);
            }

            // Toggle objects
            foreach (var obj in activateOnEnter)
                if (obj) obj.SetActive(true);
            foreach (var obj in deactivateOnEnter)
                if (obj) obj.SetActive(false);

            // Play cutscene if configured
            if (entryCutscene && (!cutscenePlayed || !playCutsceneOnce))
            {
                cutscenePlayed = true;

                var player = other.GetComponentInParent<SFS.Player.PlayerController>();
                if (player) player.LockControls(true);

                entryCutscene.stopped += (_) =>
                {
                    if (player) player.LockControls(false);
                };

                entryCutscene.Play();
            }
        }

        void OnDrawGizmos()
        {
            // Color-code by emotional tone
            Gizmos.color = beat.GetTone() switch
            {
                EmotionalTone.Gentle => new Color(0.5f, 0.8f, 0.5f, 0.3f),
                EmotionalTone.Hopeful => new Color(0.9f, 0.8f, 0.3f, 0.3f),
                EmotionalTone.Melancholic => new Color(0.4f, 0.4f, 0.7f, 0.3f),
                EmotionalTone.Grounded => new Color(0.6f, 0.5f, 0.4f, 0.3f),
                _ => new Color(1f, 1f, 1f, 0.3f)
            };

            var col = GetComponent<Collider>();
            if (col is BoxCollider box)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(box.center, box.size);
                Gizmos.DrawWireCube(box.center, box.size);
            }
        }
    }
}
