using UnityEngine;
using SFS.Core;

namespace SFS.Camera
{
    /// <summary>
    /// Modifies camera behavior based on current story beat.
    /// Handles the emotional camera language: distance = uncertainty, closeness = tension.
    /// </summary>
    public class StoryBeatCameraModifier : MonoBehaviour
    {
        [Header("Reference")]
        public ThirdPersonCameraRig cameraRig;

        [Header("Beat-Specific Offsets")]
        [Tooltip("Beat 1: Arrival - Camera pulls back (distance = uncertainty)")]
        public Vector3 arrivalOffset = new Vector3(0, 2.5f, -5.5f);

        [Tooltip("Beat 2: First Contact - Neutral, engaged")]
        public Vector3 firstContactOffset = new Vector3(0, 2.2f, -4.2f);

        [Tooltip("Beat 3: Compression - Camera closer (tension)")]
        public Vector3 compressionOffset = new Vector3(0, 1.8f, -3.0f);

        [Tooltip("Beat 4: Choosing Rest - Stable, respectful")]
        public Vector3 restOffset = new Vector3(0, 2.4f, -4.5f);

        [Tooltip("Beat 5: Society - Frames player within group")]
        public Vector3 societyOffset = new Vector3(0, 2.8f, -5.0f);

        [Tooltip("Beat 6: Shared Difficulty - Active but supported")]
        public Vector3 sharedDifficultyOffset = new Vector3(0, 2.0f, -3.8f);

        [Tooltip("Beat 7: Belonging - Gently pulls back, then holds")]
        public Vector3 belongingOffset = new Vector3(0, 3.0f, -6.0f);

        [Header("Transition")]
        public float transitionSpeed = 2f;

        Vector3 targetOffset;
        Vector3 currentOffset;

        void Start()
        {
            if (cameraRig)
            {
                currentOffset = cameraRig.offset;
                targetOffset = currentOffset;
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
            targetOffset = current switch
            {
                StoryBeat.Arrival => arrivalOffset,
                StoryBeat.FirstContact => firstContactOffset,
                StoryBeat.Compression => compressionOffset,
                StoryBeat.ChoosingRest => restOffset,
                StoryBeat.TheSociety => societyOffset,
                StoryBeat.SharedDifficulty => sharedDifficultyOffset,
                StoryBeat.QuietBelonging => belongingOffset,
                _ => firstContactOffset
            };
        }

        void Update()
        {
            if (!cameraRig) return;

            // Smooth transition between camera positions
            currentOffset = Vector3.Lerp(currentOffset, targetOffset, transitionSpeed * Time.deltaTime);
            cameraRig.offset = currentOffset;
        }
    }
}
