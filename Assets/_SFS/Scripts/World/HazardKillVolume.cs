using UnityEngine;
using SFS.Core;

namespace SFS.World
{
    public class HazardKillVolume : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            GameEvents.PlayerDied();
        }
    }
}
