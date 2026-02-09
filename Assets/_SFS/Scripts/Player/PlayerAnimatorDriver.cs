using UnityEngine;
using using UnityEngine;

public class PlayerAnimatorDriver : MonoBehaviour
{
using UnityEngine;

public class PlayerAnimatorDriver : MonoBehaviour
{
using UnityEngine;

public class PlayerAnimatorDriver : MonoBehaviour
{
using UnityEngine;

public class PlayerAnimatorDriver : MonoBehaviour
{
using UnityEngine;

[CreateAssetMenu(menuName = "Spiny-Flannel-Society/using UnityEngine;

public class PlayerAnimatorDriver : MonoBehaviour
{
using UnityEngine;

public class PlayerAnimatorDriver : MonoBehaviour
{
using UnityEngine;

public class PlayerAnimatorDriver : MonoBehaviour
{

}
}
}")]
public class PlayerAnimatorDriver : ScriptableObject
{

}
}
}
}
}SFS.Core;

namespace SFS.Player
{
    /// <summary>
    /// Sophisticated animation driver that responds to story beats.
    /// Uses damping for smooth transitions and adjusts idle intensity per beat.
    /// Falls back to SimpleProceduralAnimator if no Animator clips are assigned.
    /// </summary>
    public class PlayerAnimatorDriver : MonoBehaviour
    {
        [Header("Animator (Optional - leave empty to use procedural)")]
        public Animator animator;

        [Header("Fallback Procedural Animation")]
        [Tooltip("Used when Animator is missing or has no clips")]
        public SimpleProceduralAnimator proceduralAnimator;

        [Header("Damping")]
        public float speedDamp = 0.08f;
        public float yVelDamp = 0.08f;
        public float idleIntensityDamp = 0.3f;

        [Header("Story Beat Response")]
        [Tooltip("How much story beat affects idle animation intensity")]
        public float beatInfluence = 1f;

        bool useProceduralFallback;

        // Animator parameter hashes
        static readonly int Speed = Animator.StringToHash("Speed");
        static readonly int Grounded = Animator.StringToHash("Grounded");
        static readonly int YVelocity = Animator.StringToHash("YVelocity");
        static readonly int IdleIntensity = Animator.StringToHash("IdleIntensity");
        static readonly int Jump = Animator.StringToHash("Jump");
        static readonly int Land = Animator.StringToHash("Land");
        static readonly int Die = Animator.StringToHash("Die");
        static readonly int InRestZone = Animator.StringToHash("InRestZone");
        static readonly int WithCompanion = Animator.StringToHash("WithCompanion");
        static readonly int EmotionTone = Animator.StringToHash("EmotionTone"); // 0=Gentle, 1=Hopeful, 2=Melancholic, 3=Grounded

        bool wasGrounded;
        float currentIdleIntensity = 0.3f;
        bool inRestZone;
        bool hasCompanion;

        void Reset()
        {
            animator = GetComponentInChildren<Animator>();
            proceduralAnimator = GetComponentInChildren<SimpleProceduralAnimator>();
        }

        void Start()
        {
            // Auto-detect if we need procedural fallback
            useProceduralFallback = (animator == null || !HasValidAnimator());

            if (useProceduralFallback && proceduralAnimator == null)
            {
                // Try to find or create procedural animator
                proceduralAnimator = GetComponentInChildren<SimpleProceduralAnimator>();
                if (proceduralAnimator == null)
                {
                    Debug.LogWarning("[SFS] No Animator clips found and no SimpleProceduralAnimator. " +
                        "Add SimpleProceduralAnimator component or run SFS > Setup > Generate All Placeholder Animations");
                }
            }

            if (useProceduralFallback)
                Debug.Log("[SFS] Using procedural animation fallback");
        }

        bool HasValidAnimator()
        {
            if (!animator) return false;
            var controller = animator.runtimeAnimatorController;
            return controller != null;
        }

        void OnEnable()
        {
            StoryBeatEvents.OnBeatChanged += OnBeatChanged;
            StoryBeatEvents.OnRestZoneEntered += OnRestEnter;
            StoryBeatEvents.OnRestZoneExited += OnRestExit;
            StoryBeatEvents.OnCompanionJoined += OnCompanionJoin;
            StoryBeatEvents.OnCompanionLeft += OnCompanionLeave;
        }

        void OnDisable()
        {
            StoryBeatEvents.OnBeatChanged -= OnBeatChanged;
            StoryBeatEvents.OnRestZoneEntered -= OnRestEnter;
            StoryBeatEvents.OnRestZoneExited -= OnRestExit;
            StoryBeatEvents.OnCompanionJoined -= OnCompanionJoin;
            StoryBeatEvents.OnCompanionLeft -= OnCompanionLeave;
        }

        void OnBeatChanged(StoryBeat previous, StoryBeat current)
        {
            if (!animator) return;

            // Set emotion tone for blend tree selection
            animator.SetInteger(EmotionTone, (int)current.GetTone());
        }

        void OnRestEnter()
        {
            inRestZone = true;
            if (animator) animator.SetBool(InRestZone, true);
        }

        void OnRestExit()
        {
            inRestZone = false;
            if (animator) animator.SetBool(InRestZone, false);
        }

        void OnCompanionJoin(Transform companion)
        {
            hasCompanion = true;
            if (animator) animator.SetBool(WithCompanion, true);
        }

        void OnCompanionLeave()
        {
            hasCompanion = false;
            if (animator) animator.SetBool(WithCompanion, false);
        }

        void Update()
        {
            if (!animator || useProceduralFallback) return;

            // Get target idle intensity from story beat manager
            float targetIntensity = StoryBeatManager.Instance
                ? StoryBeatManager.Instance.CurrentIdleIntensity
                : 0.3f;

            // Rest zone further reduces intensity
            if (inRestZone)
                targetIntensity *= 0.5f;

            currentIdleIntensity = Mathf.Lerp(currentIdleIntensity, targetIntensity, idleIntensityDamp);
            animator.SetFloat(IdleIntensity, currentIdleIntensity * beatInfluence);
        }

        public void SetMove(float speed01, bool grounded, float yVel)
        {
            // Update procedural animator
            if (proceduralAnimator)
            {
                proceduralAnimator.SetState(speed01, grounded, yVel);
            }

            // Update Unity Animator if available
            if (animator && !useProceduralFallback)
            {
                animator.SetFloat(Speed, speed01, speedDamp, Time.deltaTime);
                animator.SetBool(Grounded, grounded);
                animator.SetFloat(YVelocity, yVel, yVelDamp, Time.deltaTime);
            }

            // Detect landing
            if (grounded && !wasGrounded)
            {
                TriggerLand();
            }

            wasGrounded = grounded;
        }

        public void TriggerJump()
        {
            if (proceduralAnimator)
                proceduralAnimator.OnJump();

            if (animator && !useProceduralFallback)
            {
                animator.ResetTrigger(Land);
                animator.SetTrigger(Jump);
            }
        }

        public void TriggerLandIfNeeded()
        {
            // Called from controller - we handle this via wasGrounded tracking
        }

        public void TriggerLand()
        {
            if (animator && !useProceduralFallback)
            {
                animator.ResetTrigger(Jump);
                animator.SetTrigger(Land);
            }
        }

        public void TriggerDie()
        {
            if (proceduralAnimator)
                proceduralAnimator.OnDamage();

            if (animator && !useProceduralFallback)
            {
                animator.SetTrigger(Die);
            }
        }
    }
}
