using System;
using UnityEngine;

namespace SFS.Core
{
    [Serializable]
    public class SettingsData
    {
        // Camera
        public float cameraSensitivity = 1.0f;
        public bool reducedMotion = false;

        // Movement assists
        public float coyoteTime = 0.12f;
        public float jumpBuffer = 0.12f;

        // Sensory
        public bool lowSensory = false; // e.g., reduce particles/screenshake

        // UI
        public bool highContrastUI = false;
    }
}
