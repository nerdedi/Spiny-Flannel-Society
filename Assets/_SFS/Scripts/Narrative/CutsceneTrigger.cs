using UnityEngine;
using UnityEngine.Playables;
using SFS.Player;

namespace SFS.Narrative
{
    public class CutsceneTrigger : MonoBehaviour
    {
        public PlayableDirector director;
        public bool playOnce = true;

        bool played;

        void OnTriggerEnter(Collider other)
        {
            if (played && playOnce) return;
            if (!other.CompareTag("Player")) return;
            if (!director) return;

            played = true;

            // Lock player controls during cutscene
            var player = other.GetComponentInParent<PlayerController>();
            if (player) player.LockControls(true);

            director.stopped += (_) =>
            {
                if (player) player.LockControls(false);
            };

            director.Play();
        }
    }
}
