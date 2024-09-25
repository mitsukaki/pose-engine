#region

using UnityEngine;
using UnityEditor.Animations;

using nadena.dev.modular_avatar.core;

using VRC.SDK3.Avatars.ScriptableObjects;
using static com.mitsukaki.poseengine.editor.anim.Condition;

#endregion

namespace com.mitsukaki.poseengine.editor
{
    public static class PersistentPoseUtility
    {
        public static void EnablePersistentPosing(PoseBuildContext context)
        {
            var pePrefab = context.poseEngineInstance;
            var maParams = pePrefab.GetComponent<ModularAvatarParameters>();
            if (maParams == null)
            {
                Debug.LogError("[PoseEngine] Failed to find modular avatar parameters...");
                return;
            }

            // Create the pose restore & mirror flag parameters
            ParameterUtility.AddNewParameter(
                context, "PoseEngine/PoseRestore/PoseID", true,
                ParameterSyncType.Int
            );

            ParameterUtility.AddNewParameter(
                context, "PoseEngine/PoseRestore/Mirrored", true,
                ParameterSyncType.Bool
            );

            ParameterUtility.AddNewParameter(
                context, "PoseEngine/PoseRestore/SaveEnabled", true,
                ParameterSyncType.Bool
            );

            // Replace the elevator parameter
            ParameterUtility.ReplaceParameterByName(
                context, new ParameterConfig()
                {
                    nameOrPrefix = "PoseEngine/Elevation",
                    syncType = ParameterSyncType.Float,
                    localOnly = false,
                    defaultValue = 0.5f,
                    saved = true,
                    hasExplicitDefaultValue = true,
                }
            );

            // Create the animator params
            var animBuilder = context.coreAnimator;
            animBuilder.AddParameter("PoseEngine/PoseRestore/PoseID", anim.Builder.IntParam);
            animBuilder.AddParameter("PoseEngine/PoseRestore/Mirrored", anim.Builder.BoolParam);
            animBuilder.AddParameter("PoseEngine/PoseRestore/SaveEnabled", anim.Builder.BoolParam);

            // Create the new loading state, and transitions
            var layer = animBuilder.GetLayer(Constants.LOCO_LAYER);
            AnimatorState loadingState;
            animBuilder.AddState("PoseRestore/Loading", layer, out loadingState);
            loadingState.writeDefaultValues = false;

            // Add the transitions to the restore state and also the current default state
            // skip/bypass transitions
            animBuilder.StartTransition()
                .From(loadingState).To(layer.stateMachine.defaultState)
                .SetNoExitTime().SetFixedDuration(0.01f)
                .When("PoseEngine/PoseRestore/PoseID", IsEqualTo, 0)
                .Build();

            animBuilder.StartTransition()
                .From(loadingState).To(layer.stateMachine.defaultState)
                .SetNoExitTime().SetFixedDuration(0.01f)
                .When("PoseEngine/PoseRestore/SaveEnabled", false)
                .Build();

            // Create the pose restore saving toggle
            var poseObject = new GameObject("Persist Poses");
            var settingsObj = context.poseEngineInstance.transform
                .GetChild(1).GetChild(2);
            poseObject.transform.parent = settingsObj.transform;

            // attatch the menu item to the object
            var poseMenuItem = poseObject.AddComponent<ModularAvatarMenuItem>();
            poseMenuItem.Control = new VRCExpressionsMenu.Control();

            // create the menu item for the object
            poseMenuItem.Control.name = "Persist Poses";
            poseMenuItem.Control.value = 0;
            poseMenuItem.Control.type = VRCExpressionsMenu.Control.ControlType.Toggle;
            poseMenuItem.Control.parameter = new VRCExpressionsMenu.Control.Parameter();
            poseMenuItem.Control.parameter.name = "PoseEngine/PoseRestore/SaveEnabled";

            Debug.Log("[PoseEngine] Enabled persistent posing.");
        }

        public static void FinalizePersistentPosing(PoseBuildContext context)
        {
            var animBuilder = context.coreAnimator;
            var layer = animBuilder.GetLayer(Constants.LOCO_LAYER);
            var loadingState = animBuilder.FindStateByName(
                "PoseRestore/Loading", layer
            );

            if (loadingState == null)
            {
                Debug.LogError("[PoseEngine] Failed to find loading state...");
                return;
            }

            // make the loading state the default
            layer.stateMachine.defaultState = loadingState;

            Debug.Log("[PoseEngine] Finalized persistent posing.");
        }

        public static void CreateRestoringTransition(
            PoseBuildContext context,
            AnimatorControllerLayer layer,
            AnimatorState poseState,
            Pose pose,
            bool applyMirroring,
            bool isMirrored
        )
        {
            var loadingState = context.coreAnimator.FindStateByName(
                "PoseRestore/Loading", layer
            );

            if (loadingState == null)
            {
                Debug.LogError(
                    "[PoseEngine] Persistant posing enabled but no loading state found"
                );

                return;
            }

            var transit = context.coreAnimator.StartTransition()
                .From(loadingState).To(poseState)
                .SetNoExitTime().SetFixedDuration(0.25f)
                .When("PoseEngine/PoseRestore/PoseID", IsEqualTo, pose.PoseID)
                .When("PoseEngine/PoseRestore/SaveEnabled", true);
                
            if (applyMirroring)
                transit.When("PoseEngine/PoseRestore/Mirrored", isMirrored);

            transit.Build();
        }

        public static void ApplyPersistenceParamDrivers(
            AnimatorState state, Pose pose, bool isMirrored
        )
        {
            VRCBehaviourUtility.SetParam(
                state, "PoseEngine/PoseRestore/PoseID", pose.PoseID
            );

            VRCBehaviourUtility.SetParam(
                state, "PoseEngine/PoseRestore/Activating", false
            );

            if (isMirrored)
                VRCBehaviourUtility.SetParamFlag(
                    state, "PoseEngine/PoseRestore/Mirrored"
                );
        }
    }
}