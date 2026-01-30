using UnityEngine;
using SFS.Core;

namespace SFS.World
{
    public class CollectibleTracker : MonoBehaviour
    {
        public int Total { get; private set; }

        public void Add(int amount)
        {
            Total += amount;
            GameEvents.CollectibleChanged(Total);
        }

        public void ResetTotal()
        {
            Total = 0;
            GameEvents.CollectibleChanged(Total);
        }
    }
}
