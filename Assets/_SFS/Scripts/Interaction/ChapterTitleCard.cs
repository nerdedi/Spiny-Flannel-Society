using UnityEngine;
using TMPro;

namespace SFS.Interaction
{
    /// <summary>
    /// Chapter title card — fades in and out over the screen
    /// when triggered by a trigger volume.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ChapterTitleCard : MonoBehaviour
    {
        [Header("Content")]
        public string ChapterTitle = "Chapter 1: Bract Theory";
        public string Subtitle = "Supports without proof.";

        [Header("UI")]
        public TextMeshProUGUI TitleUI;
        public TextMeshProUGUI SubtitleUI;
        public CanvasGroup FadeGroup;

        [Header("Timing")]
        public float FadeInDuration = 1.5f;
        public float HoldDuration = 3f;
        public float FadeOutDuration = 2f;

        enum State { Waiting, FadingIn, Holding, FadingOut, Done }
        State _state = State.Waiting;
        float _timer;
        bool _triggered;

        void Start()
        {
            if (FadeGroup != null) FadeGroup.alpha = 0f;
        }

        void Update()
        {
            if (_state == State.Waiting || _state == State.Done) return;

            _timer -= Time.deltaTime;

            switch (_state)
            {
                case State.FadingIn:
                    float fadeIn = 1f - (_timer / FadeInDuration);
                    if (FadeGroup != null) FadeGroup.alpha = Mathf.Clamp01(fadeIn);
                    if (_timer <= 0f) { _state = State.Holding; _timer = HoldDuration; }
                    break;

                case State.Holding:
                    if (_timer <= 0f) { _state = State.FadingOut; _timer = FadeOutDuration; }
                    break;

                case State.FadingOut:
                    float fadeOut = _timer / FadeOutDuration;
                    if (FadeGroup != null) FadeGroup.alpha = Mathf.Clamp01(fadeOut);
                    if (_timer <= 0f) { _state = State.Done; }
                    break;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") || _triggered) return;
            _triggered = true;

            if (TitleUI != null) TitleUI.text = ChapterTitle;
            if (SubtitleUI != null) SubtitleUI.text = Subtitle;

            _state = State.FadingIn;
            _timer = FadeInDuration;

            Debug.Log($"[SFS] Chapter title: {ChapterTitle} — {Subtitle}");
        }
    }
}
