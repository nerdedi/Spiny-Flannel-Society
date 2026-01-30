using UnityEngine;
using SFS.Core;

namespace SFS.World
{
    public class Collectible : MonoBehaviour
    {
        public int value = 1;
        public AudioClip pickupSfx;

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            var tracker = other.GetComponentInParent<CollectibleTracker>();
            if (tracker) tracker.Add(value);

            // Optional: play sound at position
            if (pickupSfx) AudioSource.PlayClipAtPoint(pickupSfx, transform.position, 0.8f);

            Destroy(gameObject);
        }
    }
}
