using UnityEngine;
using SFS.Core;

namespace SFS.Narrative
{
    /// <summary>
    /// Beat 5: The Society
    /// Controls synchronized motion for a group of SocietyMembers.
    /// "Community without pressure."
    /// </summary>
    public class SocietyGroup : MonoBehaviour
    {
        [Header("Sync Behavior")]
        [Tooltip("Duration of one complete sync cycle in seconds")]
        public float syncCycleDuration = 4f;

        [Tooltip("Current phase in the sync cycle (0-1)")]
        public float SyncPhase { get; private set; }

        [Header("Reveal")]
        [Tooltip("If true, members start hidden and reveal together")]
        public bool revealOnBeat = true;
        public float revealDelay = 0.5f;

        [Header("Members")]
        public SocietyMember[] members;

        bool revealed;
        float revealTimer;

        void Start()
        {
            if (revealOnBeat && members != null)
            {
                foreach (var member in members)
                {
                    if (member) member.gameObject.SetActive(false);
                }
            }
        }

        void OnEnable()
        {
            StoryBeatEvents.OnBeatChanged += OnBeatChanged;
        }

        void OnDisable()
        {
            StoryBeatEvents.OnBeatChanged -= OnBeatChanged;
        }

        void OnBeatChanged(StoryBeat previous, StoryBeat current)
        {
            // Reveal society on Beat 5
            if (current == StoryBeat.TheSociety && !revealed)
            {
                revealTimer = revealDelay;
            }
        }

        void Update()
        {
            // Update sync phase
            SyncPhase = (SyncPhase + Time.deltaTime / syncCycleDuration) % 1f;

            // Handle delayed reveal
            if (revealTimer > 0f)
            {
                revealTimer -= Time.deltaTime;
                if (revealTimer <= 0f)
                {
                    RevealMembers();
                }
            }
        }

        void RevealMembers()
        {
            revealed = true;

            if (members != null)
            {
                foreach (var member in members)
                {
                    if (member) member.gameObject.SetActive(true);
                }
            }

            StoryBeatEvents.SocietyRevealed();
        }

        /// <summary>
        /// Call to trigger synchronised micro-movement (sit, sway, turn).
        /// </summary>
        public void TriggerSyncedMotion(string triggerName)
        {
            int hash = Animator.StringToHash(triggerName);

            if (members != null)
            {
                foreach (var member in members)
                {
                    if (member && member.animator)
                    {
                        member.animator.SetTrigger(hash);
                    }
                }
            }
        }
    }
}
