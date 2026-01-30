using UnityEngine;
using SFS.Core;

namespace SFS.UI
{
    public class PauseManager : MonoBehaviour
    {
        public GameObject pauseMenuRoot;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        public void TogglePause()
        {
            bool willPause = Time.timeScale > 0.5f;
            SetPause(willPause);
        }

        public void SetPause(bool paused)
        {
            Time.timeScale = paused ? 0f : 1f;
            if (pauseMenuRoot) pauseMenuRoot.SetActive(paused);
            GameEvents.PauseChanged(paused);
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}
