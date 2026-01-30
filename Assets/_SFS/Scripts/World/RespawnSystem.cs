using UnityEngine;
using SFS.Core;
using SFS.Player;

namespace SFS.World
{
    public class RespawnSystem : MonoBehaviour
    {
        public Transform defaultSpawn;
        Vector3 currentSpawn;

        void OnEnable()
        {
            GameEvents.OnCheckpointReached += SetSpawn;
            GameEvents.OnPlayerDied += Respawn;
        }

        void OnDisable()
        {
            GameEvents.OnCheckpointReached -= SetSpawn;
            GameEvents.OnPlayerDied -= Respawn;
        }

        void Start()
        {
            currentSpawn = defaultSpawn ? defaultSpawn.position : Vector3.zero;
        }

        void SetSpawn(Vector3 pos) => currentSpawn = pos;

        void Respawn()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (!player) return;

            // Lock controls briefly and reposition cleanly
            var controller = player.GetComponent<PlayerController>();
            if (controller) controller.LockControls(true);

            var cc = player.GetComponent<CharacterController>();
            if (cc) cc.enabled = false;

            player.transform.position = currentSpawn;

            if (cc) cc.enabled = true;
            if (controller) controller.LockControls(false);
        }
    }
}
