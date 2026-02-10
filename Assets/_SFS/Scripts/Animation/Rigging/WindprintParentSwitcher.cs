using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace SFS.Animation.Rigging
{
    /// <summary>
    /// Controls a MultiParentConstraint to switch the Windprint Rig
    /// (visual floating object) between different follow targets:
    ///
    ///   Source 0: Shoulder (Cushion mode — flows behind the player)
    ///   Source 1: Hip      (Guard mode — anchored low, protective)
    ///   Source 2: Hand     (Active verb — held in hand during Read/Rewrite)
    ///   Source 3: World    (Detached — floating free during transitions)
    ///
    /// Pattern from the AnimationRigging-AdvancedSetups repo: multi-source
    /// weight blending for smooth re-parenting without actual hierarchy changes.
    ///
    /// SETUP (done by RiggingSetupWizard):
    ///   - A "WindprintRigVisual" GameObject with MultiParentConstraint
    ///   - Source objects: ShoulderTarget, HipTarget, HandTarget, WorldAnchor
    ///   - This script blends between them
    /// </summary>
    public class WindprintParentSwitcher : MonoBehaviour
    {
        [Header("Constraint")]
        public MultiParentConstraint parentConstraint;

        [Header("Mode")]
        public WindprintAnchor currentAnchor = WindprintAnchor.Shoulder;

        [Header("Blending")]
        [Tooltip("How fast the rig visual transitions between anchors")]
        public float transitionSpeed = 5f;

        // Target weights for each source (indexed 0-3)
        float[] targetWeights = new float[4];
        float[] currentWeights = new float[4];

        public enum WindprintAnchor
        {
            Shoulder = 0, // Cushion mode
            Hip      = 1, // Guard mode
            Hand     = 2, // Active verb
            World    = 3  // Detached / transitioning
        }

        void Start()
        {
            SetAnchor(currentAnchor);
            // Snap to initial weights
            for (int i = 0; i < 4; i++)
                currentWeights[i] = targetWeights[i];
            ApplyWeights();
        }

        void LateUpdate()
        {
            if (parentConstraint == null) return;

            bool changed = false;
            for (int i = 0; i < 4; i++)
            {
                if (!Mathf.Approximately(currentWeights[i], targetWeights[i]))
                {
                    currentWeights[i] = Mathf.MoveTowards(
                        currentWeights[i], targetWeights[i],
                        transitionSpeed * Time.deltaTime);
                    changed = true;
                }
            }

            if (changed)
                ApplyWeights();
        }

        void ApplyWeights()
        {
            if (parentConstraint == null) return;

            var data = parentConstraint.data;
            var sources = data.sourceObjects;

            for (int i = 0; i < Mathf.Min(sources.Count, 4); i++)
            {
                var src = sources[i];
                src.weight = currentWeights[i];
                sources.SetWeight(i, currentWeights[i]);
            }

            data.sourceObjects = sources;
        }

        // ═════════════════════════════════════════════════════════
        //  PUBLIC API
        // ═════════════════════════════════════════════════════════

        /// <summary>Smoothly transition to a new anchor point.</summary>
        public void SetAnchor(WindprintAnchor anchor)
        {
            currentAnchor = anchor;

            for (int i = 0; i < 4; i++)
                targetWeights[i] = 0f;

            targetWeights[(int)anchor] = 1f;
        }

        /// <summary>Switch to Cushion mode (shoulder follow).</summary>
        public void SetCushionMode() => SetAnchor(WindprintAnchor.Shoulder);

        /// <summary>Switch to Guard mode (hip anchor).</summary>
        public void SetGuardMode() => SetAnchor(WindprintAnchor.Hip);

        /// <summary>Move to hand during active verb.</summary>
        public void SetHandMode() => SetAnchor(WindprintAnchor.Hand);

        /// <summary>Detach to world space.</summary>
        public void Detach() => SetAnchor(WindprintAnchor.World);

        /// <summary>Snap immediately (no blend).</summary>
        public void SnapToAnchor(WindprintAnchor anchor)
        {
            SetAnchor(anchor);
            for (int i = 0; i < 4; i++)
                currentWeights[i] = targetWeights[i];
            ApplyWeights();
        }
    }
}
