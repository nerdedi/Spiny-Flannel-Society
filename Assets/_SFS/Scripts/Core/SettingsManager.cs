using UnityEngine;

namespace SFS.Core
{
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance { get; private set; }
        public SettingsData Data { get; private set; } = new SettingsData();

        const string Key = "SFS_SETTINGS_JSON";

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }

        public void Load()
        {
            if (PlayerPrefs.HasKey(Key))
            {
                var json = PlayerPrefs.GetString(Key);
                Data = JsonUtility.FromJson<SettingsData>(json) ?? new SettingsData();
            }
            else Data = new SettingsData();
            GameEvents.SettingsChanged();
        }

        public void Save()
        {
            var json = JsonUtility.ToJson(Data);
            PlayerPrefs.SetString(Key, json);
            PlayerPrefs.Save();
            GameEvents.SettingsChanged();
        }

        public void SetReducedMotion(bool value) { Data.reducedMotion = value; Save(); }
        public void SetCameraSensitivity(float value) { Data.cameraSensitivity = value; Save(); }
        public void SetCoyoteTime(float value) { Data.coyoteTime = value; Save(); }
        public void SetJumpBuffer(float value) { Data.jumpBuffer = value; Save(); }
        public void SetLowSensory(bool value) { Data.lowSensory = value; Save(); }
        public void SetHighContrastUI(bool value) { Data.highContrastUI = value; Save(); }
    }
}
