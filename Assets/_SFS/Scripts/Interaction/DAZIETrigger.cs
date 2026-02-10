using UnityEngine;
using TMPro;

namespace SFS.Interaction
{
    /// <summary>
    /// DAZIE dialogue trigger — plays one line of dialogue when
    /// the player enters a trigger volume. Used for contextual
    /// narration throughout the game.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class DAZIETrigger : MonoBehaviour
    {
        [Header("Dialogue")]
        [TextArea(3, 6)]
        public string DialogueLine;
        public float DisplayDuration = 5f;
        public bool PlayOnce = true;

        [Header("UI")]
        public TextMeshProUGUI DialogueUI;

        bool _hasPlayed;
        float _timer;
        bool _showing;

        void Update()
        {
            if (!_showing) return;
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                _showing = false;
                if (DialogueUI != null) DialogueUI.text = "";
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (PlayOnce && _hasPlayed) return;

            Show();
            _hasPlayed = true;
        }

        void Show()
        {
            _showing = true;
            _timer = DisplayDuration;
            if (DialogueUI != null)
                DialogueUI.text = DialogueLine;

            Debug.Log($"[DAZIE] {DialogueLine}");
        }
    }
}
