using UnityEngine;
using TMPro;

namespace SFS.Interaction
{
    /// <summary>
    /// A world object that contains a readable Default.
    /// Walk near it to highlight; interact to Read; interact again to Rewrite.
    ///
    /// Drives the TranslationVerbBridge and DefaultsRegistry.
    /// Visual state managed by its DistortionZone child.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ReadableObject : MonoBehaviour
    {
        [Header("Default Binding")]
        [Tooltip("Key from DefaultsRegistry (e.g. 'timing_window').")]
        public string DefaultKey;

        [Header("Visuals")]
        public GameObject HighlightEffect;
        public GameObject ReadOverlay;
        public TextMeshPro LabelText;
        public TextMeshPro DescriptionText;

        [Header("Interaction")]
        public float InteractionRadius = 3f;
        public KeyCode InteractKey = KeyCode.E;

        enum State { Idle, Highlighted, Reading, ReadComplete, Rewriting, Rewritten }
        State _state = State.Idle;
        Transform _player;
        bool _playerInRange;

        void Start()
        {
            if (HighlightEffect != null) HighlightEffect.SetActive(false);
            if (ReadOverlay != null) ReadOverlay.SetActive(false);

            _player = FindPlayerTransform();
        }

        void Update()
        {
            if (_player == null) return;

            float dist = Vector3.Distance(transform.position, _player.position);
            bool inRange = dist <= InteractionRadius;

            if (inRange && !_playerInRange)
                OnPlayerEnterRange();
            else if (!inRange && _playerInRange)
                OnPlayerExitRange();

            _playerInRange = inRange;

            if (_playerInRange && Input.GetKeyDown(InteractKey))
                HandleInteract();
        }

        void OnPlayerEnterRange()
        {
            if (_state == State.Rewritten) return;

            _state = State.Highlighted;
            if (HighlightEffect != null) HighlightEffect.SetActive(true);

            // Show the label
            if (LabelText != null)
            {
                var def = Core.DefaultsRegistry.Instance?.GetDefault(DefaultKey);
                if (def != null) LabelText.text = def.Label;
            }
        }

        void OnPlayerExitRange()
        {
            if (_state == State.Highlighted)
            {
                _state = State.Idle;
                if (HighlightEffect != null) HighlightEffect.SetActive(false);
            }
        }

        void HandleInteract()
        {
            var registry = Core.DefaultsRegistry.Instance;
            if (registry == null) return;

            switch (_state)
            {
                case State.Highlighted:
                    // Read the default
                    string desc = registry.Read(DefaultKey);
                    if (desc != null)
                    {
                        _state = State.ReadComplete;
                        if (ReadOverlay != null) ReadOverlay.SetActive(true);
                        if (DescriptionText != null) DescriptionText.text = desc;

                        // Notify audio
                        Audio.AudioTriggerManager.Instance?.PlayReadReveal();

                        Debug.Log($"[SFS] Read default: {DefaultKey}");
                    }
                    break;

                case State.ReadComplete:
                    // Rewrite the default
                    bool ok = registry.Rewrite(DefaultKey);
                    if (ok)
                    {
                        _state = State.Rewritten;
                        if (ReadOverlay != null) ReadOverlay.SetActive(false);
                        if (HighlightEffect != null) HighlightEffect.SetActive(false);

                        // Audio handles its own event via DefaultsRegistry.OnDefaultRewritten
                        Debug.Log($"[SFS] Rewrote default: {DefaultKey}");
                    }
                    break;
            }
        }

        Transform FindPlayerTransform()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            return player != null ? player.transform : null;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.7f, 0.2f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, InteractionRadius);
        }
    }
}
