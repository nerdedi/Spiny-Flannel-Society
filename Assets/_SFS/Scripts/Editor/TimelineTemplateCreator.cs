#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.IO;

namespace SFS.Editor
{
    /// <summary>
    /// Creates Timeline templates for each story beat cutscene.
    /// These provide the structure - you add the actual clips.
    /// </summary>
    public class TimelineTemplateCreator : EditorWindow
    {
        [MenuItem("SFS/Setup/Create Timeline Templates")]
        public static void CreateAllTimelines()
        {
            string dir = "Assets/_SFS/Timeline";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            CreateArrivalTimeline(dir);
            CreateCompressionTimeline(dir);
            CreateRestTimeline(dir);
            CreateSocietyRevealTimeline(dir);
            CreateBelongingTimeline(dir);

            AssetDatabase.Refresh();
            Debug.Log("[SFS] Created all Timeline templates in Assets/_SFS/Timeline/");
            Debug.Log("[SFS] Add Cinemachine package for camera tracks, and assign animation clips!");
        }

        static void CreateArrivalTimeline(string dir)
        {
            var timeline = ScriptableObject.CreateInstance<TimelineAsset>();

            // Animation track for player idle
            var animTrack = timeline.CreateTrack<AnimationTrack>(null, "Player Animation");

            // Audio track for ambient swell
            var audioTrack = timeline.CreateTrack<AudioTrack>(null, "Ambient Audio");

            // Marker track for events
            var markerTrack = timeline.markerTrack;

            AssetDatabase.CreateAsset(timeline, $"{dir}/Beat1_Arrival.playable");

            Debug.Log("[SFS] Beat 1 Timeline: Camera should pull back from close to wide over 2-3 seconds");
        }

        static void CreateCompressionTimeline(string dir)
        {
            var timeline = ScriptableObject.CreateInstance<TimelineAsset>();

            // This is mostly driven by gameplay, but can have a subtle sting
            var audioTrack = timeline.CreateTrack<AudioTrack>(null, "Tension Sting");

            AssetDatabase.CreateAsset(timeline, $"{dir}/Beat3_Compression.playable");

            Debug.Log("[SFS] Beat 3 Timeline: Optional tension audio sting on entry");
        }

        static void CreateRestTimeline(string dir)
        {
            var timeline = ScriptableObject.CreateInstance<TimelineAsset>();

            // Gentle camera settle
            var animTrack = timeline.CreateTrack<AnimationTrack>(null, "Player Rest Idle");
            var audioTrack = timeline.CreateTrack<AudioTrack>(null, "Calm Layer Fade In");

            AssetDatabase.CreateAsset(timeline, $"{dir}/Beat4_Rest.playable");

            Debug.Log("[SFS] Beat 4 Timeline: Camera stabilizes, ambient softens");
        }

        static void CreateSocietyRevealTimeline(string dir)
        {
            var timeline = ScriptableObject.CreateInstance<TimelineAsset>();

            // Camera reframes to show group
            // NPC animations begin
            // Warm audio layer
            var animTrack1 = timeline.CreateTrack<AnimationTrack>(null, "Player");
            var animTrack2 = timeline.CreateTrack<AnimationTrack>(null, "Society Members");
            var audioTrack = timeline.CreateTrack<AudioTrack>(null, "Society Theme");

            AssetDatabase.CreateAsset(timeline, $"{dir}/Beat5_SocietyReveal.playable");

            Debug.Log("[SFS] Beat 5 Timeline: Camera widens to frame player among group, NPCs animate in sync");
        }

        static void CreateBelongingTimeline(string dir)
        {
            var timeline = ScriptableObject.CreateInstance<TimelineAsset>();

            // Final camera pullback and hold
            // Group idle loops
            // Ambient resolves to calm
            var animTrack1 = timeline.CreateTrack<AnimationTrack>(null, "Player Final Idle");
            var animTrack2 = timeline.CreateTrack<AnimationTrack>(null, "Society Final Idle");
            var audioTrack = timeline.CreateTrack<AudioTrack>(null, "Ambient Resolve");

            AssetDatabase.CreateAsset(timeline, $"{dir}/Beat7_Belonging.playable");

            Debug.Log("[SFS] Beat 7 Timeline: Camera slowly pulls back then holds. No fanfare. Just presence.");
        }

        [MenuItem("SFS/Setup/Create Cutscene Director Prefab")]
        public static void CreateDirectorPrefab()
        {
            string dir = "Assets/_SFS/Prefabs";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var directorObj = new GameObject("CutsceneDirector");
            var director = directorObj.AddComponent<PlayableDirector>();
            director.playOnAwake = false;
            director.extrapolationMode = DirectorWrapMode.Hold;

            // Save as prefab
            string path = $"{dir}/CutsceneDirector.prefab";
            PrefabUtility.SaveAsPrefabAsset(directorObj, path);
            DestroyImmediate(directorObj);

            Debug.Log($"[SFS] Created Cutscene Director prefab at {path}");
            Debug.Log("[SFS] Drag into scene, assign Timeline asset, and bind tracks to scene objects");
        }
    }
}
#endif
