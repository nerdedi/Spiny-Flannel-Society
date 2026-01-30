using UnityEngine;
using SFS.Core;

namespace SFS.World
{
    public class Checkpoint : MonoBehaviour
    {
        public Transform respawnPoint;

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            Vector3 pos = respawnPoint ? respawnPoint.position : transform.position;
            GameEvents.CheckpointReached(pos);
        }
    }
}
