using TMPro;
using UnityEngine;
using SFS.Core;

namespace SFS.UI
{
    public class CollectibleCounterUI : MonoBehaviour
    {
        public TMP_Text label;

        void OnEnable()
        {
            GameEvents.OnCollectibleChanged += OnChanged;
        }

        void OnDisable()
        {
            GameEvents.OnCollectibleChanged -= OnChanged;
        }

        void OnChanged(int total)
        {
            if (label) label.text = $"Flannel: {total}";
        }
    }
}
