using UnityEngine;
using TMPro;

namespace SFS.Interaction
{
    /// <summary>
    /// NPC that reacts to defaults being rewritten.
    /// In the first playable minute: sits on a bench, stands up and
    /// walks through the corridor when timing_window is rewritten.
    /// </summary>
    public class NPCReactor : MonoBehaviour
    {
        [Header("Behaviour")]
        [Tooltip("Default key that triggers this NPC's reaction.")]
        public string TriggerDefaultKey = "timing_window";

        [Header("Dialogue")]
        public string BlockedDialogue = "";
        public string ReactionDialogue = "Oh — the gears slowed. I've been waiting here for… I don't know how long.";

        [Header("References")]
        public TextMeshPro DialogueText;
        public Transform WalkTarget;
        public Animator NPCAnimator;

        [Header("Movement")]
        public float WalkSpeed = 2f;

        enum NPCState { Waiting, Reacting, Walking, Done }
        NPCState _state = NPCState.Waiting;
        float _dialogueTimer;

        void Start()
        {
            Core.DefaultsRegistry.OnDefaultRewritten += HandleRewritten;
            if (DialogueText != null) DialogueText.text = BlockedDialogue;
        }

        void OnDestroy()
        {
            Core.DefaultsRegistry.OnDefaultRewritten -= HandleRewritten;
        }

        void Update()
        {
            switch (_state)
            {
                case NPCState.Reacting:
                    _dialogueTimer -= Time.deltaTime;
                    if (_dialogueTimer <= 0f)
                    {
                        _state = NPCState.Walking;
                        if (NPCAnimator != null) NPCAnimator.SetBool("Walking", true);
                    }
                    break;

                case NPCState.Walking:
                    if (WalkTarget != null)
                    {
                        transform.position = Vector3.MoveTowards(
                            transform.position, WalkTarget.position,
                            WalkSpeed * Time.deltaTime);
                        transform.LookAt(new Vector3(WalkTarget.position.x, transform.position.y, WalkTarget.position.z));

                        if (Vector3.Distance(transform.position, WalkTarget.position) < 0.3f)
                        {
                            _state = NPCState.Done;
                            if (NPCAnimator != null) NPCAnimator.SetBool("Walking", false);
                            if (DialogueText != null) DialogueText.text = "";
                        }
                    }
                    break;
            }
        }

        void HandleRewritten(string key)
        {
            if (key != TriggerDefaultKey || _state != NPCState.Waiting) return;

            _state = NPCState.Reacting;
            _dialogueTimer = 4f;

            if (DialogueText != null) DialogueText.text = ReactionDialogue;
            if (NPCAnimator != null) NPCAnimator.SetTrigger("React");

            Debug.Log($"[SFS] NPC reacted to default rewrite: {key}");
        }
    }
}
