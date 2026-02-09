using UnityEngine;
using System;
using System.Collections.Generic;

namespace SFS.Animation
{
    /// <summary>
    /// Advanced animation state machine with layered blending, transitions, and story beat response.
    /// The core engine for all SFS character animation.
    /// </summary>
    public class AnimationStateMachine : MonoBehaviour
    {
        #region Configuration

        [Header("═══ STATE MACHINE ═══")]
        [Tooltip("Enable debug visualization and logging")]
        public bool debugMode = false;

        [Tooltip("Default transition blend time")]
        [Range(0.05f, 1f)]
        public float defaultBlendTime = 0.15f;

        [Tooltip("Maximum number of active layers")]
        [Range(1, 8)]
        public int maxLayers = 4;

        [Header("═══ STORY BEAT RESPONSE ═══")]
        [Tooltip("Enable automatic response to story beats")]
        public bool respondToStoryBeats = true;

        [Tooltip("Speed multiplier transition time when story beat changes")]
        [Range(0.1f, 3f)]
        public float beatTransitionTime = 1.5f;

        #endregion

        #region Runtime State

        // Layer management
        AnimationLayer[] layers;

        // Current states per layer
        Dictionary<int, AnimationStateBase> currentStates = new Dictionary<int, AnimationStateBase>();
        Dictionary<int, AnimationStateBase> previousStates = new Dictionary<int, AnimationStateBase>();
        Dictionary<int, float> transitionProgress = new Dictionary<int, float>();
        Dictionary<int, float> transitionDuration = new Dictionary<int, float>();

        // Story beat state
        EmotionalTone currentTone = EmotionalTone.Gentle;
        float currentSpeedMultiplier = 1f;
        float targetSpeedMultiplier = 1f;

        // Animation output
        Vector3 outputPosition;
        Quaternion outputRotation;
        Vector3 outputScale;

        // Events
        public event Action<int, AnimationStateBase> OnStateEntered;
        public event Action<int, AnimationStateBase> OnStateExited;
        public event Action<EmotionalTone> OnToneChanged;

        #endregion

        #region Lifecycle

        void Awake()
        {
            InitializeLayers();
        }

        void OnEnable()
        {
            if (respondToStoryBeats)
            {
                AnimationEvents.OnStoryBeatAnimation += HandleStoryBeat;
            }
        }

        void OnDisable()
        {
            AnimationEvents.OnStoryBeatAnimation -= HandleStoryBeat;
        }

        void Update()
        {
            float deltaTime = Time.deltaTime;

            // Update speed multiplier
            currentSpeedMultiplier = Mathf.MoveTowards(
                currentSpeedMultiplier,
                targetSpeedMultiplier,
                deltaTime / beatTransitionTime
            );

            // Update all layers
            for (int i = 0; i < layers.Length; i++)
            {
                UpdateLayer(i, deltaTime * currentSpeedMultiplier);
            }

            // Blend outputs from all layers
            BlendLayerOutputs();
        }

        #endregion

        #region Layer Management

        void InitializeLayers()
        {
            layers = new AnimationLayer[maxLayers];

            for (int i = 0; i < maxLayers; i++)
            {
                layers[i] = new AnimationLayer
                {
                    Index = i,
                    Weight = i == 0 ? 1f : 0f,
                    BlendMode = i == 0 ? LayerBlendMode.Override : LayerBlendMode.Additive,
                    IsActive = i == 0
                };

                currentStates[i] = null;
                previousStates[i] = null;
                transitionProgress[i] = 1f;
                transitionDuration[i] = 0f;
            }
        }

        /// <summary>Set the weight of a layer (0-1)</summary>
        public void SetLayerWeight(int layerIndex, float weight)
        {
            if (layerIndex >= 0 && layerIndex < maxLayers)
            {
                layers[layerIndex].Weight = Mathf.Clamp01(weight);
                layers[layerIndex].IsActive = weight > 0.01f;
            }
        }

        /// <summary>Set the blend mode for a layer</summary>
        public void SetLayerBlendMode(int layerIndex, LayerBlendMode mode)
        {
            if (layerIndex >= 0 && layerIndex < maxLayers)
            {
                layers[layerIndex].BlendMode = mode;
            }
        }

        void UpdateLayer(int layerIndex, float deltaTime)
        {
            var layer = layers[layerIndex];
            if (!layer.IsActive) return;

            var current = currentStates[layerIndex];
            var previous = previousStates[layerIndex];

            // Update transition
            if (transitionProgress[layerIndex] < 1f)
            {
                transitionProgress[layerIndex] += deltaTime / transitionDuration[layerIndex];
                transitionProgress[layerIndex] = Mathf.Clamp01(transitionProgress[layerIndex]);

                // Update both states during transition
                previous?.OnUpdate(deltaTime);
            }

            // Update current state
            current?.OnUpdate(deltaTime);

            // Check if transition complete
            if (transitionProgress[layerIndex] >= 1f && previous != null)
            {
                previous.OnExit();
                OnStateExited?.Invoke(layerIndex, previous);
                previousStates[layerIndex] = null;
            }
        }

        void BlendLayerOutputs()
        {
            outputPosition = Vector3.zero;
            outputRotation = Quaternion.identity;
            outputScale = Vector3.one;

            for (int i = 0; i < maxLayers; i++)
            {
                var layer = layers[i];
                if (!layer.IsActive) continue;

                var current = currentStates[i];
                var previous = previousStates[i];

                if (current == null) continue;

                // Get blended output from current/previous states
                Vector3 layerPos;
                Quaternion layerRot;
                Vector3 layerScale;

                if (previous != null && transitionProgress[i] < 1f)
                {
                    // Blend between previous and current
                    float t = EaseTransition(transitionProgress[i]);
                    layerPos = Vector3.Lerp(previous.OutputPosition, current.OutputPosition, t);
                    layerRot = Quaternion.Slerp(previous.OutputRotation, current.OutputRotation, t);
                    layerScale = Vector3.Lerp(previous.OutputScale, current.OutputScale, t);
                }
                else
                {
                    layerPos = current.OutputPosition;
                    layerRot = current.OutputRotation;
                    layerScale = current.OutputScale;
                }

                // Apply layer to output based on blend mode
                ApplyLayerToOutput(layer, layerPos, layerRot, layerScale);
            }
        }

        void ApplyLayerToOutput(AnimationLayer layer, Vector3 pos, Quaternion rot, Vector3 scale)
        {
            switch (layer.BlendMode)
            {
                case LayerBlendMode.Override:
                    outputPosition = Vector3.Lerp(outputPosition, pos, layer.Weight);
                    outputRotation = Quaternion.Slerp(outputRotation, rot, layer.Weight);
                    outputScale = Vector3.Lerp(outputScale, scale, layer.Weight);
                    break;

                case LayerBlendMode.Additive:
                    outputPosition += pos * layer.Weight;
                    outputRotation = Quaternion.Slerp(Quaternion.identity, rot, layer.Weight) * outputRotation;
                    outputScale = Vector3.Scale(outputScale, Vector3.Lerp(Vector3.one, scale, layer.Weight));
                    break;

                case LayerBlendMode.Multiply:
                    outputScale = Vector3.Scale(outputScale, Vector3.Lerp(Vector3.one, scale, layer.Weight));
                    break;
            }
        }

        float EaseTransition(float t)
        {
            // Smooth ease in-out
            return t < 0.5f
                ? 2f * t * t
                : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
        }

        #endregion

        #region State Transitions

        /// <summary>Transition to a new state on a layer</summary>
        public void TransitionTo(AnimationStateBase newState, int layerIndex = 0, float blendTime = -1f)
        {
            if (layerIndex < 0 || layerIndex >= maxLayers) return;

            var current = currentStates[layerIndex];

            // Don't transition to same state type
            if (current != null && current.GetType() == newState.GetType()) return;

            // Set up transition
            if (current != null)
            {
                previousStates[layerIndex] = current;
            }

            currentStates[layerIndex] = newState;
            transitionProgress[layerIndex] = 0f;
            transitionDuration[layerIndex] = blendTime >= 0 ? blendTime : defaultBlendTime;

            // Enter new state
            newState.OnEnter();
            OnStateEntered?.Invoke(layerIndex, newState);

            if (debugMode)
            {
                Debug.Log($"[SFS Animation] Layer {layerIndex}: {current?.StateName ?? "None"} → {newState.StateName}");
            }
        }

        /// <summary>Immediately set a state without transition</summary>
        public void SetStateImmediate(AnimationStateBase newState, int layerIndex = 0)
        {
            if (layerIndex < 0 || layerIndex >= maxLayers) return;

            var current = currentStates[layerIndex];
            current?.OnExit();

            currentStates[layerIndex] = newState;
            previousStates[layerIndex] = null;
            transitionProgress[layerIndex] = 1f;

            newState.OnEnter();
            OnStateEntered?.Invoke(layerIndex, newState);
        }

        /// <summary>Get the current state on a layer</summary>
        public AnimationStateBase GetCurrentState(int layerIndex = 0)
        {
            return currentStates.TryGetValue(layerIndex, out var state) ? state : null;
        }

        /// <summary>Check if currently transitioning on a layer</summary>
        public bool IsTransitioning(int layerIndex = 0)
        {
            return transitionProgress.TryGetValue(layerIndex, out var progress) && progress < 1f;
        }

        #endregion

        #region Output Access

        /// <summary>Get the final blended position offset</summary>
        public Vector3 GetOutputPosition() => outputPosition;

        /// <summary>Get the final blended rotation</summary>
        public Quaternion GetOutputRotation() => outputRotation;

        /// <summary>Get the final blended scale</summary>
        public Vector3 GetOutputScale() => outputScale;

        #endregion

        #region Story Beat Response

        void HandleStoryBeat(int chapter, EmotionalTone tone)
        {
            if (!respondToStoryBeats) return;

            var previousTone = currentTone;
            currentTone = tone;

            // Adjust animation speed based on emotional tone
            targetSpeedMultiplier = GetSpeedMultiplierForTone(tone);

            OnToneChanged?.Invoke(tone);

            if (debugMode)
            {
                Debug.Log($"[SFS Animation] Story beat: Chapter {chapter}, Tone: {tone}, Speed: {targetSpeedMultiplier:F2}x");
            }
        }

        float GetSpeedMultiplierForTone(EmotionalTone tone)
        {
            return tone switch
            {
                EmotionalTone.Gentle => 0.85f,
                EmotionalTone.Hopeful => 1.1f,
                EmotionalTone.Melancholic => 0.7f,
                EmotionalTone.Grounded => 0.95f,
                EmotionalTone.Tender => 0.8f,
                _ => 1f
            };
        }

        /// <summary>Get current emotional tone</summary>
        public EmotionalTone GetCurrentTone() => currentTone;

        /// <summary>Get current speed multiplier from emotional tone</summary>
        public float GetSpeedMultiplier() => currentSpeedMultiplier;

        #endregion
    }

    #region Supporting Classes

    /// <summary>Animation layer data</summary>
    public class AnimationLayer
    {
        public int Index;
        public float Weight = 1f;
        public LayerBlendMode BlendMode = LayerBlendMode.Override;
        public bool IsActive = true;
    }

    /// <summary>Layer blend modes</summary>
    public enum LayerBlendMode
    {
        Override,   // Replace lower layers
        Additive,   // Add to lower layers
        Multiply    // Multiply with lower layers (for scale)
    }

    /// <summary>Base class for all animation states</summary>
    public abstract class AnimationStateBase
    {
        public abstract string StateName { get; }

        // Output values
        public Vector3 OutputPosition { get; protected set; }
        public Quaternion OutputRotation { get; protected set; }
        public Vector3 OutputScale { get; protected set; } = Vector3.one;

        // Time tracking
        protected float stateTime;
        protected float normalizedTime;

        // Emotional response
        protected EmotionalTone currentTone = EmotionalTone.Gentle;
        protected float toneMultiplier = 1f;

        /// <summary>Called when entering this state</summary>
        public virtual void OnEnter()
        {
            stateTime = 0f;
            normalizedTime = 0f;
            OutputPosition = Vector3.zero;
            OutputRotation = Quaternion.identity;
            OutputScale = Vector3.one;
        }

        /// <summary>Called every frame while in this state</summary>
        public virtual void OnUpdate(float deltaTime)
        {
            stateTime += deltaTime;
        }

        /// <summary>Called when exiting this state</summary>
        public virtual void OnExit()
        {
        }

        /// <summary>Set the emotional tone for this state</summary>
        public virtual void SetTone(EmotionalTone tone)
        {
            currentTone = tone;
            toneMultiplier = GetToneMultiplier(tone);
        }

        protected virtual float GetToneMultiplier(EmotionalTone tone)
        {
            return tone switch
            {
                EmotionalTone.Gentle => 0.7f,
                EmotionalTone.Hopeful => 1.15f,
                EmotionalTone.Melancholic => 0.5f,
                EmotionalTone.Grounded => 0.9f,
                EmotionalTone.Tender => 0.6f,
                _ => 1f
            };
        }

        /// <summary>Get animation intensity for current tone</summary>
        protected float GetIntensity(float baseValue)
        {
            return baseValue * toneMultiplier;
        }
    }

    #endregion
}
