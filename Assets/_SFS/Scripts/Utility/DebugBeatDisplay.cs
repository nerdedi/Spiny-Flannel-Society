using UnityEngine;
using SFS.Core;

namespace SFS.Utility
{
    /// <summary>
    /// Runtime debug display showing current story beat info.
    /// Toggle with F1.
    /// </summary>
    public class DebugBeatDisplay : MonoBehaviour
    {
        [Header("Settings")]
        public KeyCode toggleKey = KeyCode.F1;
        public bool showOnStart = true;

        bool isVisible;
        GUIStyle boxStyle;
        GUIStyle labelStyle;

        void Start()
        {
            isVisible = showOnStart;
        }

        void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                isVisible = !isVisible;
            }
        }

        void OnGUI()
        {
            if (!isVisible) return;

            // Initialize styles
            if (boxStyle == null)
            {
                boxStyle = new GUIStyle(GUI.skin.box);
                boxStyle.normal.background = MakeTexture(2, 2, new Color(0, 0, 0, 0.7f));
            }

            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.fontSize = 14;
                labelStyle.normal.textColor = Color.white;
            }

            // Draw debug panel
            float width = 280;
            float height = 180;
            float x = 10;
            float y = Screen.height - height - 10;

            GUI.Box(new Rect(x, y, width, height), "", boxStyle);

            float lineHeight = 22;
            float currentY = y + 10;
            float labelX = x + 10;

            GUI.Label(new Rect(labelX, currentY, width - 20, lineHeight), "<b>STORY BEAT DEBUG</b>", labelStyle);
            currentY += lineHeight;

            if (StoryBeatManager.Instance)
            {
                var beat = StoryBeatManager.Instance.CurrentBeat;
                var tone = StoryBeatManager.Instance.CurrentTone;
                var intensity = StoryBeatManager.Instance.CurrentIdleIntensity;

                GUI.Label(new Rect(labelX, currentY, width - 20, lineHeight), $"Beat: <color=yellow>{beat}</color>", labelStyle);
                currentY += lineHeight;

                string toneColor = tone switch
                {
                    EmotionalTone.Gentle => "cyan",
                    EmotionalTone.Hopeful => "yellow",
                    EmotionalTone.Melancholic => "magenta",
                    EmotionalTone.Grounded => "green",
                    _ => "white"
                };
                GUI.Label(new Rect(labelX, currentY, width - 20, lineHeight), $"Tone: <color={toneColor}>{tone}</color>", labelStyle);
                currentY += lineHeight;

                GUI.Label(new Rect(labelX, currentY, width - 20, lineHeight), $"Idle Intensity: {intensity:F2}", labelStyle);
                currentY += lineHeight;
            }
            else
            {
                GUI.Label(new Rect(labelX, currentY, width - 20, lineHeight), "<color=red>No StoryBeatManager</color>", labelStyle);
                currentY += lineHeight;
            }

            currentY += 5;

            // Settings info
            if (SettingsManager.Instance)
            {
                var data = SettingsManager.Instance.Data;
                GUI.Label(new Rect(labelX, currentY, width - 20, lineHeight), $"Coyote: {data.coyoteTime:F2}s | Buffer: {data.jumpBuffer:F2}s", labelStyle);
                currentY += lineHeight;
                GUI.Label(new Rect(labelX, currentY, width - 20, lineHeight), $"Reduced Motion: {data.reducedMotion}", labelStyle);
            }

            currentY += lineHeight;
            GUI.Label(new Rect(labelX, currentY, width - 20, lineHeight), $"<i>Press {toggleKey} to hide</i>", labelStyle);
        }

        Texture2D MakeTexture(int width, int height, Color color)
        {
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = color;

            Texture2D tex = new Texture2D(width, height);
            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }
    }
}
