#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace SFS.Editor
{
    /// <summary>
    /// One-click generator for placeholder animation clips.
    /// These clips contain minimal keyframes so the Animator states work immediately.
    /// Replace with real animations later when ready.
    /// </summary>
    public static class PlaceholderAnimationGenerator
    {
        const string ANIM_PATH = "Assets/_SFS/Animations/Clips";

        [MenuItem("SFS/Setup/Generate All Placeholder Animations (One Click!)")]
        public static void GenerateAll()
        {
            EnsureDirectory();

            // Player animations
            CreateBobAnimation("Player_Idle", 2f, 0.05f);
            CreateBobAnimation("Player_Walk", 8f, 0.08f);
            CreateBobAnimation("Player_Run", 12f, 0.12f);
            CreateJumpAnimation("Player_Jump");
            CreateFallAnimation("Player_Fall");
            CreateLandAnimation("Player_Land");
            CreateDeathAnimation("Player_Die");

            // NPC animations
            CreateBobAnimation("NPC_Idle_Breathe", 1.5f, 0.03f);
            CreateBobAnimation("NPC_Idle_Shift", 0.8f, 0.05f);
            CreateBobAnimation("NPC_Idle_LookAround", 0.5f, 0.02f);
            CreateAcknowledgeAnimation("NPC_Acknowledged");
            CreateBobAnimation("NPC_BelongingIdle", 1f, 0.04f);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("<color=green>[SFS] All placeholder animations created!</color>");
            Debug.Log($"[SFS] Find them at: {ANIM_PATH}");
            Debug.Log("[SFS] Now run 'SFS > Setup > Assign Animations to Controllers' to hook them up!");
        }

        [MenuItem("SFS/Setup/Assign Animations to Controllers")]
        public static void AssignToControllers()
        {
            AssignPlayerAnimations();
            AssignNPCAnimations();
            AssetDatabase.SaveAssets();
            Debug.Log("<color=green>[SFS] Animations assigned to controllers!</color>");
        }

        static void EnsureDirectory()
        {
            if (!Directory.Exists(ANIM_PATH))
            {
                Directory.CreateDirectory(ANIM_PATH);
            }
        }

        static void CreateBobAnimation(string name, float speed, float amount)
        {
            string path = $"{ANIM_PATH}/{name}.anim";
            if (File.Exists(path))
            {
                Debug.Log($"[SFS] Skipping {name} (already exists)");
                return;
            }

            var clip = new AnimationClip();
            clip.name = name;

            // Loop settings
            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            // Create a simple Y-position bob curve
            float duration = 1f / (speed / 4f); // Adjust for visual speed
            var curve = new AnimationCurve();
            curve.AddKey(0f, 0f);
            curve.AddKey(duration * 0.25f, amount);
            curve.AddKey(duration * 0.5f, 0f);
            curve.AddKey(duration * 0.75f, amount);
            curve.AddKey(duration, 0f);

            // Make it smooth
            for (int i = 0; i < curve.keys.Length; i++)
            {
                AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Auto);
                AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Auto);
            }

            clip.SetCurve("", typeof(Transform), "localPosition.y", curve);

            AssetDatabase.CreateAsset(clip, path);
            Debug.Log($"[SFS] Created: {name}");
        }

        static void CreateJumpAnimation(string name)
        {
            string path = $"{ANIM_PATH}/{name}.anim";
            if (File.Exists(path)) return;

            var clip = new AnimationClip();
            clip.name = name;

            // Non-looping
            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = false;
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            // Stretch up on Y scale
            var scaleY = new AnimationCurve();
            scaleY.AddKey(0f, 1f);
            scaleY.AddKey(0.1f, 0.8f);  // Squat
            scaleY.AddKey(0.3f, 1.2f);  // Stretch
            scaleY.AddKey(0.5f, 1.1f);
            clip.SetCurve("", typeof(Transform), "localScale.y", scaleY);

            // Compress X/Z
            var scaleXZ = new AnimationCurve();
            scaleXZ.AddKey(0f, 1f);
            scaleXZ.AddKey(0.1f, 1.1f);
            scaleXZ.AddKey(0.3f, 0.9f);
            scaleXZ.AddKey(0.5f, 0.95f);
            clip.SetCurve("", typeof(Transform), "localScale.x", scaleXZ);
            clip.SetCurve("", typeof(Transform), "localScale.z", scaleXZ);

            AssetDatabase.CreateAsset(clip, path);
            Debug.Log($"[SFS] Created: {name}");
        }

        static void CreateFallAnimation(string name)
        {
            string path = $"{ANIM_PATH}/{name}.anim";
            if (File.Exists(path)) return;

            var clip = new AnimationClip();
            clip.name = name;

            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            // Slight anticipation pose
            var scaleY = new AnimationCurve();
            scaleY.AddKey(0f, 1.05f);
            scaleY.AddKey(0.5f, 0.95f);
            scaleY.AddKey(1f, 1.05f);
            clip.SetCurve("", typeof(Transform), "localScale.y", scaleY);

            AssetDatabase.CreateAsset(clip, path);
            Debug.Log($"[SFS] Created: {name}");
        }

        static void CreateLandAnimation(string name)
        {
            string path = $"{ANIM_PATH}/{name}.anim";
            if (File.Exists(path)) return;

            var clip = new AnimationClip();
            clip.name = name;

            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = false;
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            // Squash on land
            var scaleY = new AnimationCurve();
            scaleY.AddKey(0f, 1f);
            scaleY.AddKey(0.1f, 0.7f);  // Squash
            scaleY.AddKey(0.3f, 1.1f);  // Overshoot
            scaleY.AddKey(0.5f, 1f);    // Settle
            clip.SetCurve("", typeof(Transform), "localScale.y", scaleY);

            var scaleXZ = new AnimationCurve();
            scaleXZ.AddKey(0f, 1f);
            scaleXZ.AddKey(0.1f, 1.2f);
            scaleXZ.AddKey(0.3f, 0.95f);
            scaleXZ.AddKey(0.5f, 1f);
            clip.SetCurve("", typeof(Transform), "localScale.x", scaleXZ);
            clip.SetCurve("", typeof(Transform), "localScale.z", scaleXZ);

            AssetDatabase.CreateAsset(clip, path);
            Debug.Log($"[SFS] Created: {name}");
        }

        static void CreateDeathAnimation(string name)
        {
            string path = $"{ANIM_PATH}/{name}.anim";
            if (File.Exists(path)) return;

            var clip = new AnimationClip();
            clip.name = name;

            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = false;
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            // Dramatic collapse
            var scaleY = new AnimationCurve();
            scaleY.AddKey(0f, 1f);
            scaleY.AddKey(0.2f, 0.3f);
            scaleY.AddKey(1f, 0.1f);
            clip.SetCurve("", typeof(Transform), "localScale.y", scaleY);

            var scaleXZ = new AnimationCurve();
            scaleXZ.AddKey(0f, 1f);
            scaleXZ.AddKey(0.2f, 1.5f);
            scaleXZ.AddKey(1f, 1.8f);
            clip.SetCurve("", typeof(Transform), "localScale.x", scaleXZ);
            clip.SetCurve("", typeof(Transform), "localScale.z", scaleXZ);

            // Fade or fall - rotation
            var rotX = new AnimationCurve();
            rotX.AddKey(0f, 0f);
            rotX.AddKey(0.5f, 45f);
            rotX.AddKey(1f, 90f);
            clip.SetCurve("", typeof(Transform), "localEulerAngles.x", rotX);

            AssetDatabase.CreateAsset(clip, path);
            Debug.Log($"[SFS] Created: {name}");
        }

        static void CreateAcknowledgeAnimation(string name)
        {
            string path = $"{ANIM_PATH}/{name}.anim";
            if (File.Exists(path)) return;

            var clip = new AnimationClip();
            clip.name = name;

            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = false;
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            // Quick nod - bob and scale
            var posY = new AnimationCurve();
            posY.AddKey(0f, 0f);
            posY.AddKey(0.15f, -0.1f);  // Nod down
            posY.AddKey(0.3f, 0.05f);   // Up
            posY.AddKey(0.5f, 0f);
            clip.SetCurve("", typeof(Transform), "localPosition.y", posY);

            var scaleY = new AnimationCurve();
            scaleY.AddKey(0f, 1f);
            scaleY.AddKey(0.15f, 0.95f);
            scaleY.AddKey(0.35f, 1.05f);
            scaleY.AddKey(0.5f, 1f);
            clip.SetCurve("", typeof(Transform), "localScale.y", scaleY);

            AssetDatabase.CreateAsset(clip, path);
            Debug.Log($"[SFS] Created: {name}");
        }

        static void AssignPlayerAnimations()
        {
            string controllerPath = "Assets/_SFS/Animations/PlayerAnimator.controller";
            var controller = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(controllerPath);
            if (!controller)
            {
                Debug.LogError("[SFS] PlayerAnimator.controller not found! Run 'Create Player Animator Controller' first.");
                return;
            }

            var stateMachine = controller.layers[0].stateMachine;
            foreach (var childState in stateMachine.states)
            {
                var state = childState.state;
                string clipName = $"Player_{state.name}";
                string clipPath = $"{ANIM_PATH}/{clipName}.anim";
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);

                if (clip)
                {
                    state.motion = clip;
                    Debug.Log($"[SFS] Assigned {clipName} to state {state.name}");
                }
            }

            EditorUtility.SetDirty(controller);
        }

        static void AssignNPCAnimations()
        {
            string controllerPath = "Assets/_SFS/Animations/NPCAnimator.controller";
            var controller = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(controllerPath);
            if (!controller)
            {
                Debug.LogError("[SFS] NPCAnimator.controller not found! Run 'Create NPC Animator Controller' first.");
                return;
            }

            var stateMachine = controller.layers[0].stateMachine;
            foreach (var childState in stateMachine.states)
            {
                var state = childState.state;
                string clipName = $"NPC_{state.name}";
                string clipPath = $"{ANIM_PATH}/{clipName}.anim";
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);

                if (clip)
                {
                    state.motion = clip;
                    Debug.Log($"[SFS] Assigned {clipName} to state {state.name}");
                }
            }

            EditorUtility.SetDirty(controller);
        }
    }
}
#endif
