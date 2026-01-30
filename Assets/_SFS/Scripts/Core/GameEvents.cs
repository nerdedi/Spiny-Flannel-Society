using System;

namespace SFS.Core
{
    public static class GameEvents
    {
        public static event Action<int> OnCollectibleChanged;
        public static event Action OnPlayerDied;
        public static event Action<UnityEngine.Vector3> OnCheckpointReached;
        public static event Action<bool> OnPauseChanged;
        public static event Action OnSettingsChanged;

        public static void CollectibleChanged(int total) => OnCollectibleChanged?.Invoke(total);
        public static void PlayerDied() => OnPlayerDied?.Invoke();
        public static void CheckpointReached(UnityEngine.Vector3 pos) => OnCheckpointReached?.Invoke(pos);
        public static void PauseChanged(bool paused) => OnPauseChanged?.Invoke(paused);
        public static void SettingsChanged() => OnSettingsChanged?.Invoke();
    }
}
