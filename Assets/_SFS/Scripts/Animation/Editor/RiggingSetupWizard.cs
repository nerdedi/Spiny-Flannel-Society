#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Animations.Rigging;
using SFS.Animation.Rigging;

namespace SFS.Animation.Editor
{
    /// <summary>
    /// One-click wizard that builds the full Animation Rigging hierarchy
    /// on the player character:
    ///
    ///   Player (root)
    ///   └── PlayerRig
    ///       ├── GazeRig
    ///       │   ├── [MultiAimConstraint on head bone]
    ///       │   └── GazeTarget (empty transform)
    ///       ├── RightHandIK
    ///       │   ├── [TwoBoneIKConstraint]
    ///       │   └── RightHandTarget
    ///       ├── LeftHandIK
    ///       │   ├── [TwoBoneIKConstraint]
    ///       │   └── LeftHandTarget
    ///       └── WindprintRigVisual
    ///           ├── [MultiParentConstraint]
    ///           └── Anchor targets (Shoulder, Hip, Hand, World)
    ///
    /// USAGE: Select your player GameObject, then SFS > Rigging > Setup Player Rig.
    /// The wizard creates everything, wires references, and adds the controller scripts.
    ///
    /// If bones aren't found (no skeleton yet), it creates the hierarchy with
    /// placeholder transforms and logs instructions.
    /// </summary>
    public class RiggingSetupWizard : EditorWindow
    {
        // ═════════════════════════════════════════════════════════
        //  MAIN SETUP ENTRY POINT
        // ═════════════════════════════════════════════════════════

        [MenuItem("SFS/Rigging/Setup Player Rig (One Click)")]
        public static void SetupPlayerRig()
        {
            var player = Selection.activeGameObject;
            if (player == null)
            {
                EditorUtility.DisplayDialog(
                    "SFS Rigging Setup",
                    "Select the Player GameObject in the Hierarchy first, then run this again.",
                    "OK");
                return;
            }

            Undo.RegisterCompleteObjectUndo(player, "SFS Rigging Setup");

            // 1. Add RigBuilder if not present
            var rigBuilder = player.GetComponent<RigBuilder>();
            if (!rigBuilder)
                rigBuilder = Undo.AddComponent<RigBuilder>(player);

            // 2. Create the Rig root
            var rigObj = FindOrCreateChild(player.transform, "PlayerRig");
            var rig = rigObj.GetComponent<Rig>();
            if (!rig)
                rig = Undo.AddComponent<Rig>(rigObj);

            // Register this rig with the builder
            if (!rigBuilder.layers.Exists(l => l.rig == rig))
            {
                rigBuilder.layers.Add(new RigLayer(rig, true));
                EditorUtility.SetDirty(rigBuilder);
            }

            // 3. Find bones (best effort — works with any humanoid rig)
            var animator = player.GetComponent<Animator>();
            Transform headBone = null, chest = null, spine = null;
            Transform rUpperArm = null, rForearm = null, rHand = null;
            Transform lUpperArm = null, lForearm = null, lHand = null;
            Transform rHip = null, lHip = null;

            bool hasBones = false;

            if (animator != null && animator.isHuman)
            {
                headBone  = animator.GetBoneTransform(HumanBodyBones.Head);
                chest     = animator.GetBoneTransform(HumanBodyBones.Chest);
                spine     = animator.GetBoneTransform(HumanBodyBones.Spine);
                rUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
                rForearm  = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
                rHand     = animator.GetBoneTransform(HumanBodyBones.RightHand);
                lUpperArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
                lForearm  = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
                lHand     = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                rHip      = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
                lHip      = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);

                hasBones = headBone != null;
            }

            // ───────────────────────────────────────────────────
            //  GAZE RIG
            // ───────────────────────────────────────────────────
            var gazeObj = FindOrCreateChild(rigObj.transform, "GazeRig");

            var aimConstraint = gazeObj.GetComponent<MultiAimConstraint>();
            if (!aimConstraint)
                aimConstraint = Undo.AddComponent<MultiAimConstraint>(gazeObj);

            var gazeTargetObj = FindOrCreateChild(rigObj.transform, "GazeTarget");
            gazeTargetObj.transform.localPosition = new Vector3(0, 1.6f, 3f);

            // Configure the aim constraint
            var aimData = aimConstraint.data;
            if (headBone != null)
            {
                aimData.constrainedObject = headBone;
            }

            // Set aim axis
            aimData.aimAxis = MultiAimConstraintData.Axis.Z;
            aimData.upAxis = MultiAimConstraintData.Axis.Y;

            // Add gaze target as source
            var aimSources = aimData.sourceObjects;
            if (aimSources.Count == 0)
            {
                aimSources.Add(new WeightedTransform(gazeTargetObj.transform, 1f));
            }
            aimData.sourceObjects = aimSources;
            aimConstraint.data = aimData;

            // Set initial weight low so it doesn't override everything
            aimConstraint.weight = 0f;

            // ───────────────────────────────────────────────────
            //  RIGHT HAND IK
            // ───────────────────────────────────────────────────
            var rightIKObj = FindOrCreateChild(rigObj.transform, "RightHandIK");
            var rightIK = rightIKObj.GetComponent<TwoBoneIKConstraint>();
            if (!rightIK)
                rightIK = Undo.AddComponent<TwoBoneIKConstraint>(rightIKObj);

            var rightTargetObj = FindOrCreateChild(rigObj.transform, "RightHandTarget");
            rightTargetObj.transform.localPosition = new Vector3(0.3f, 1.2f, 0.5f);

            var rightHintObj = FindOrCreateChild(rigObj.transform, "RightElbowHint");
            rightHintObj.transform.localPosition = new Vector3(0.4f, 0.9f, -0.2f);

            var rightData = rightIK.data;
            rightData.target = rightTargetObj.transform;
            rightData.hint = rightHintObj.transform;
            if (rHand != null)     rightData.tip  = rHand;
            if (rForearm != null)  rightData.mid  = rForearm;
            if (rUpperArm != null) rightData.root = rUpperArm;
            rightIK.data = rightData;
            rightIK.weight = 0f;

            // ───────────────────────────────────────────────────
            //  LEFT HAND IK
            // ───────────────────────────────────────────────────
            var leftIKObj = FindOrCreateChild(rigObj.transform, "LeftHandIK");
            var leftIK = leftIKObj.GetComponent<TwoBoneIKConstraint>();
            if (!leftIK)
                leftIK = Undo.AddComponent<TwoBoneIKConstraint>(leftIKObj);

            var leftTargetObj = FindOrCreateChild(rigObj.transform, "LeftHandTarget");
            leftTargetObj.transform.localPosition = new Vector3(-0.3f, 1.2f, 0.5f);

            var leftHintObj = FindOrCreateChild(rigObj.transform, "LeftElbowHint");
            leftHintObj.transform.localPosition = new Vector3(-0.4f, 0.9f, -0.2f);

            var leftData = leftIK.data;
            leftData.target = leftTargetObj.transform;
            leftData.hint = leftHintObj.transform;
            if (lHand != null)     leftData.tip  = lHand;
            if (lForearm != null)  leftData.mid  = lForearm;
            if (lUpperArm != null) leftData.root = lUpperArm;
            leftIK.data = leftData;
            leftIK.weight = 0f;

            // ───────────────────────────────────────────────────
            //  WINDPRINT MULTI-PARENT
            // ───────────────────────────────────────────────────
            var windprintVisualObj = FindOrCreateChild(rigObj.transform, "WindprintRigVisual");
            var multiParent = windprintVisualObj.GetComponent<MultiParentConstraint>();
            if (!multiParent)
                multiParent = Undo.AddComponent<MultiParentConstraint>(windprintVisualObj);

            // Create anchor targets
            var shoulderAnchor = FindOrCreateChild(player.transform, "WindprintAnchor_Shoulder");
            shoulderAnchor.transform.localPosition = new Vector3(0.2f, 1.3f, -0.15f);

            var hipAnchor = FindOrCreateChild(player.transform, "WindprintAnchor_Hip");
            hipAnchor.transform.localPosition = new Vector3(0.25f, 0.8f, 0f);

            var handAnchor = FindOrCreateChild(player.transform, "WindprintAnchor_Hand");
            handAnchor.transform.localPosition = new Vector3(0.3f, 1.1f, 0.4f);

            var worldAnchor = FindOrCreateChild(player.transform, "WindprintAnchor_World");
            worldAnchor.transform.localPosition = new Vector3(0, 1.5f, 0.8f);

            var mpData = multiParent.data;
            var mpSources = mpData.sourceObjects;
            if (mpSources.Count == 0)
            {
                mpSources.Add(new WeightedTransform(shoulderAnchor.transform, 1f));
                mpSources.Add(new WeightedTransform(hipAnchor.transform, 0f));
                mpSources.Add(new WeightedTransform(handAnchor.transform, 0f));
                mpSources.Add(new WeightedTransform(worldAnchor.transform, 0f));
            }
            mpData.sourceObjects = mpSources;

            // Set constrained object to the windprint visual itself
            mpData.constrainedObject = windprintVisualObj.transform;
            multiParent.data = mpData;
            multiParent.weight = 1f;

            // ───────────────────────────────────────────────────
            //  CONTROLLER SCRIPTS
            // ───────────────────────────────────────────────────

            // GazeTracker
            var gazeTracker = player.GetComponent<GazeTracker>();
            if (!gazeTracker)
                gazeTracker = Undo.AddComponent<GazeTracker>(player);

            gazeTracker.headAimConstraint = aimConstraint;
            gazeTracker.gazeTarget = gazeTargetObj.transform;
            gazeTracker.eyePoint = headBone != null ? headBone : player.transform;

            // IKVerbRig
            var ikVerbRig = player.GetComponent<IKVerbRig>();
            if (!ikVerbRig)
                ikVerbRig = Undo.AddComponent<IKVerbRig>(player);

            ikVerbRig.rightHandIK = rightIK;
            ikVerbRig.leftHandIK = leftIK;
            ikVerbRig.rightHandTarget = rightTargetObj.transform;
            ikVerbRig.leftHandTarget = leftTargetObj.transform;

            // WindprintParentSwitcher
            var windprintSwitcher = windprintVisualObj.GetComponent<WindprintParentSwitcher>();
            if (!windprintSwitcher)
                windprintSwitcher = Undo.AddComponent<WindprintParentSwitcher>(windprintVisualObj);

            windprintSwitcher.parentConstraint = multiParent;

            EditorUtility.SetDirty(player);

            // ───────────────────────────────────────────────────
            //  DONE
            // ───────────────────────────────────────────────────
            string boneStatus = hasBones
                ? "Bones found and wired automatically!"
                : "No humanoid skeleton detected yet. Bone references are blank — " +
                  "they'll be auto-wired when you import a character model with a Humanoid rig.";

            Debug.Log($"[SFS] Player Rig setup complete!\n{boneStatus}");

            EditorUtility.DisplayDialog(
                "SFS Rigging Setup Complete",
                "Created:\n" +
                "• Gaze tracking (head aim at Readable objects)\n" +
                "• Right hand IK (Read/Rewrite gestures)\n" +
                "• Left hand IK (two-hand Rewrite gestures)\n" +
                "• Windprint visual rig (multi-parent switching)\n\n" +
                boneStatus,
                "Nice!");
        }

        // ═════════════════════════════════════════════════════════
        //  UPDATE EXISTING ANIMATOR CONTROLLER
        // ═════════════════════════════════════════════════════════

        [MenuItem("SFS/Rigging/Add Verb Parameters to Animator")]
        public static void AddVerbParametersToAnimator()
        {
            string path = "Assets/_SFS/Animations/PlayerAnimator.controller";
            var controller = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(path);

            if (controller == null)
            {
                EditorUtility.DisplayDialog(
                    "SFS Rigging",
                    $"No Animator Controller found at {path}.\n" +
                    "Run SFS > Setup > Create Player Animator Controller first.",
                    "OK");
                return;
            }

            // All the verb/rigging parameters from AnimParams.cs that
            // the original AnimatorSetupWizard doesn't create
            var newParams = new (string name, AnimatorControllerParameterType type)[]
            {
                // Translation Verbs
                ("ReadDefault",    AnimatorControllerParameterType.Trigger),
                ("RewriteCushion", AnimatorControllerParameterType.Trigger),
                ("RewriteGuard",   AnimatorControllerParameterType.Trigger),
                // Windprint Costs
                ("EntropyBleed",   AnimatorControllerParameterType.Trigger),
                ("RouteLock",      AnimatorControllerParameterType.Trigger),
                // Symbolic Combat
                ("Pulse",          AnimatorControllerParameterType.Trigger),
                ("ThreadLash",     AnimatorControllerParameterType.Trigger),
                ("RadiantHold",    AnimatorControllerParameterType.Bool),
                ("EdgeClaim",      AnimatorControllerParameterType.Trigger),
                ("Retune",         AnimatorControllerParameterType.Trigger),
                // Movement extensions
                ("Dash",           AnimatorControllerParameterType.Trigger),
                ("Grapple",        AnimatorControllerParameterType.Trigger),
                ("Glide",          AnimatorControllerParameterType.Bool),
                ("WallRun",        AnimatorControllerParameterType.Bool),
                // Sensory
                ("Overload",       AnimatorControllerParameterType.Float),
                ("Calm",           AnimatorControllerParameterType.Float),
                // Rename fix: AnimParams uses VerticalVel, wizard used YVelocity
                ("VerticalVel",    AnimatorControllerParameterType.Float),
            };

            int added = 0;
            foreach (var (name, type) in newParams)
            {
                // Check if already exists
                bool exists = false;
                foreach (var p in controller.parameters)
                {
                    if (p.name == name) { exists = true; break; }
                }

                if (!exists)
                {
                    controller.AddParameter(name, type);
                    added++;
                }
            }

            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();

            Debug.Log($"[SFS] Added {added} new parameters to PlayerAnimator.controller");
            EditorUtility.DisplayDialog(
                "SFS Rigging",
                $"Added {added} verb/combat/sensory parameters to the Animator Controller.\n" +
                "All AnimParams.cs hashes now have matching parameters.",
                "OK");
        }

        // ═════════════════════════════════════════════════════════
        //  HELPERS
        // ═════════════════════════════════════════════════════════

        static GameObject FindOrCreateChild(Transform parent, string name)
        {
            var existing = parent.Find(name);
            if (existing != null)
                return existing.gameObject;

            var obj = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(obj, "SFS Create " + name);
            obj.transform.SetParent(parent, false);
            return obj;
        }
    }
}
#endif
