#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;

namespace SFS.Editor
{
    /// <summary>
    /// Editor wizard that creates the complete Player Animator Controller
    /// with all parameters, states, and transitions configured.
    /// </summary>
    public class AnimatorSetupWizard : EditorWindow
    {
        [MenuItem("SFS/Setup/Create Player Animator Controller")]
        public static void CreatePlayerAnimator()
        {
            string path = "Assets/_SFS/Animations/PlayerAnimator.controller";

            // Ensure directory exists
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            // Create controller
            var controller = AnimatorController.CreateAnimatorControllerAtPath(path);

            // Add parameters
            controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
            controller.AddParameter("Grounded", AnimatorControllerParameterType.Bool);
            controller.AddParameter("YVelocity", AnimatorControllerParameterType.Float);
            controller.AddParameter("IdleIntensity", AnimatorControllerParameterType.Float);
            controller.AddParameter("EmotionTone", AnimatorControllerParameterType.Int);
            controller.AddParameter("InRestZone", AnimatorControllerParameterType.Bool);
            controller.AddParameter("WithCompanion", AnimatorControllerParameterType.Bool);
            controller.AddParameter("Jump", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Land", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Die", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("SyncPhase", AnimatorControllerParameterType.Float);
            controller.AddParameter("IdleVariant", AnimatorControllerParameterType.Int);
            controller.AddParameter("Acknowledged", AnimatorControllerParameterType.Bool);

            // Get the base layer
            var rootStateMachine = controller.layers[0].stateMachine;

            // Create states
            var idleState = rootStateMachine.AddState("Idle", new Vector3(300, 0, 0));
            var walkState = rootStateMachine.AddState("Walk", new Vector3(300, 60, 0));
            var runState = rootStateMachine.AddState("Run", new Vector3(300, 120, 0));
            var jumpState = rootStateMachine.AddState("Jump", new Vector3(500, 0, 0));
            var fallState = rootStateMachine.AddState("Fall", new Vector3(500, 60, 0));
            var landState = rootStateMachine.AddState("Land", new Vector3(500, 120, 0));
            var dieState = rootStateMachine.AddState("Die", new Vector3(700, 60, 0));

            // Set default state
            rootStateMachine.defaultState = idleState;

            // Create transitions: Idle -> Walk
            var idleToWalk = idleState.AddTransition(walkState);
            idleToWalk.AddCondition(AnimatorConditionMode.Greater, 0.1f, "Speed");
            idleToWalk.hasExitTime = false;
            idleToWalk.duration = 0.15f;

            // Walk -> Run
            var walkToRun = walkState.AddTransition(runState);
            walkToRun.AddCondition(AnimatorConditionMode.Greater, 0.6f, "Speed");
            walkToRun.hasExitTime = false;
            walkToRun.duration = 0.2f;

            // Run -> Walk
            var runToWalk = runState.AddTransition(walkState);
            runToWalk.AddCondition(AnimatorConditionMode.Less, 0.6f, "Speed");
            runToWalk.hasExitTime = false;
            runToWalk.duration = 0.2f;

            // Walk -> Idle
            var walkToIdle = walkState.AddTransition(idleState);
            walkToIdle.AddCondition(AnimatorConditionMode.Less, 0.1f, "Speed");
            walkToIdle.hasExitTime = false;
            walkToIdle.duration = 0.15f;

            // Run -> Idle
            var runToIdle = runState.AddTransition(idleState);
            runToIdle.AddCondition(AnimatorConditionMode.Less, 0.1f, "Speed");
            runToIdle.hasExitTime = false;
            runToIdle.duration = 0.2f;

            // Any -> Jump (trigger)
            var anyToJump = rootStateMachine.AddAnyStateTransition(jumpState);
            anyToJump.AddCondition(AnimatorConditionMode.If, 0, "Jump");
            anyToJump.hasExitTime = false;
            anyToJump.duration = 0.1f;

            // Jump -> Fall
            var jumpToFall = jumpState.AddTransition(fallState);
            jumpToFall.AddCondition(AnimatorConditionMode.Less, 0f, "YVelocity");
            jumpToFall.hasExitTime = false;
            jumpToFall.duration = 0.15f;

            // Fall -> Land (when grounded)
            var fallToLand = fallState.AddTransition(landState);
            fallToLand.AddCondition(AnimatorConditionMode.If, 0, "Grounded");
            fallToLand.hasExitTime = false;
            fallToLand.duration = 0.1f;

            // Idle/Walk/Run -> Fall (when not grounded)
            var idleToFall = idleState.AddTransition(fallState);
            idleToFall.AddCondition(AnimatorConditionMode.IfNot, 0, "Grounded");
            idleToFall.AddCondition(AnimatorConditionMode.Less, 0f, "YVelocity");
            idleToFall.hasExitTime = false;
            idleToFall.duration = 0.1f;

            var walkToFall = walkState.AddTransition(fallState);
            walkToFall.AddCondition(AnimatorConditionMode.IfNot, 0, "Grounded");
            walkToFall.AddCondition(AnimatorConditionMode.Less, 0f, "YVelocity");
            walkToFall.hasExitTime = false;
            walkToFall.duration = 0.1f;

            var runToFall = runState.AddTransition(fallState);
            runToFall.AddCondition(AnimatorConditionMode.IfNot, 0, "Grounded");
            runToFall.AddCondition(AnimatorConditionMode.Less, 0f, "YVelocity");
            runToFall.hasExitTime = false;
            runToFall.duration = 0.1f;

            // Land -> Idle
            var landToIdle = landState.AddTransition(idleState);
            landToIdle.hasExitTime = true;
            landToIdle.exitTime = 0.9f;
            landToIdle.duration = 0.1f;

            // Any -> Die
            var anyToDie = rootStateMachine.AddAnyStateTransition(dieState);
            anyToDie.AddCondition(AnimatorConditionMode.If, 0, "Die");
            anyToDie.hasExitTime = false;
            anyToDie.duration = 0.1f;

            // Save
            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();

            Debug.Log($"[SFS] Created Player Animator Controller at {path}");
            Debug.Log("[SFS] Remember to assign animation clips to each state!");

            Selection.activeObject = controller;
        }

        [MenuItem("SFS/Setup/Create NPC Animator Controller")]
        public static void CreateNPCAnimator()
        {
            string path = "Assets/_SFS/Animations/NPCAnimator.controller";

            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var controller = AnimatorController.CreateAnimatorControllerAtPath(path);

            // NPC-specific parameters
            controller.AddParameter("IdleVariant", AnimatorControllerParameterType.Int);
            controller.AddParameter("SyncPhase", AnimatorControllerParameterType.Float);
            controller.AddParameter("Acknowledged", AnimatorControllerParameterType.Bool);
            controller.AddParameter("BelongingIdle", AnimatorControllerParameterType.Trigger);

            var rootStateMachine = controller.layers[0].stateMachine;

            // Create idle variants
            var idle1 = rootStateMachine.AddState("Idle_Breathe", new Vector3(300, 0, 0));
            var idle2 = rootStateMachine.AddState("Idle_Shift", new Vector3(300, 60, 0));
            var idle3 = rootStateMachine.AddState("Idle_LookAround", new Vector3(300, 120, 0));
            var acknowledged = rootStateMachine.AddState("Acknowledged", new Vector3(500, 60, 0));
            var belongingIdle = rootStateMachine.AddState("BelongingIdle", new Vector3(700, 60, 0));

            rootStateMachine.defaultState = idle1;

            // Idle variant transitions (based on IdleVariant int)
            var to2 = idle1.AddTransition(idle2);
            to2.AddCondition(AnimatorConditionMode.Equals, 1, "IdleVariant");
            to2.hasExitTime = false;

            var to3 = idle1.AddTransition(idle3);
            to3.AddCondition(AnimatorConditionMode.Equals, 2, "IdleVariant");
            to3.hasExitTime = false;

            // Acknowledged transition
            var toAck = rootStateMachine.AddAnyStateTransition(acknowledged);
            toAck.AddCondition(AnimatorConditionMode.If, 0, "Acknowledged");
            toAck.hasExitTime = false;
            toAck.duration = 0.3f;

            // Belonging idle (final beat)
            var toBelong = rootStateMachine.AddAnyStateTransition(belongingIdle);
            toBelong.AddCondition(AnimatorConditionMode.If, 0, "BelongingIdle");
            toBelong.hasExitTime = false;
            toBelong.duration = 0.5f;

            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();

            Debug.Log($"[SFS] Created NPC Animator Controller at {path}");
            Selection.activeObject = controller;
        }
    }
}
#endif
