
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

using nadena.dev.modular_avatar.core;

using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

using com.mitsukaki.poseengine.editor.generators;
using static com.mitsukaki.poseengine.editor.anim.Condition;

namespace com.mitsukaki.poseengine.editor
{
    public class Assembler
    {
        private List<IPoseGenerator> generators;

        public Assembler(List<IPoseGenerator> generators)
        {
            this.generators = generators;
        }

        public void Assemble(GameObject avatarRoot, PoseEngineFactory factory)
        {
            // Create the needed directories
            AssetUtility.CreateGeneratedDirectories();

            // Attach the pose engine prefab to the avatar
            var poseEngineInstance = AssetUtility.AttatchPrefabFromGUID(
                avatarRoot.transform,
                Constants.TEMPLATE_ASSET_GUID
            );

            // Create the pose menu
            var poseMenu = CreatePoseMenu(poseEngineInstance, factory);
            if (poseMenu == null) {
                Debug.LogError("[PoseEngine] Failed to create pose menu.");
                return;
            }

            // Create the animator controller
            var animBuilder = anim.Builder.CreateSerialized(
                "Assets/PoseEngine/Generated/"
                    + AssetUtility.RandomAssetName("controller")
            );

            // Build the control layer
            BuildControlLayer(animBuilder);

            // Run the pose animation generators
            ExecuteGenerators(animBuilder, avatarRoot, poseMenu, factory);

            // Apply the animator to the animator merger
            var animatorMerger = FindAnimatorMerger(
                avatarRoot,
                VRCAvatarDescriptor.AnimLayerType.Action
            );

            if (animatorMerger == null) {
                Debug.LogError("[PoseEngine] Failed to find animator merger...");
                return;
            }

            animatorMerger.animator = animBuilder;
        }

        private void BuildControlLayer(anim.Builder animBuilder)
        {
            AnimatorControllerLayer controlLayer;
            AnimatorState inactiveState, activeState;
            VRCAnimatorLayerControl activeBehaviour, inactiveBehaviour;

            // Add the parameters
            animBuilder.AddParameter("PoseEngine/Pose", anim.Builder.IntParam);
            animBuilder.AddParameter("PoseEngine/PoseState/Enter", anim.Builder.BoolParam);
            animBuilder.AddParameter("PoseEngine/PoseState/Exit", anim.Builder.BoolParam);

            // Build the layer
            animBuilder.AddLayer("PoseEngine/Poser/Control", out controlLayer);

            // Add states
            animBuilder.AddDefaultState("Inactive", controlLayer, 0, 0, out inactiveState);
            animBuilder.AddState("Active", controlLayer, 0, 50, out activeState);

            // Add transitions
            animBuilder.StartTransition()
                .From(inactiveState).To(activeState).SetDuration(0.25f)
                .When("PoseEngine/Pose", IsGreaterThan, 0)
                .When("PoseEngine/Pose", IsNotEqualTo, 255)
                .Build();

            animBuilder.StartTransition()
                .From(activeState).To(inactiveState).SetDuration(0.25f)
                .When("PoseEngine/Pose", IsEqualTo, 255)
                .Build();

            // Add behaviours
            animBuilder
                .AddStateBehaviour<VRCAnimatorLayerControl>(
                    inactiveState, out inactiveBehaviour
                )
                .AddStateBehaviour<VRCAnimatorLayerControl>(
                    activeState, out activeBehaviour
                );

            // Configure the behaviours
            ConfigureLayerControl(
                inactiveBehaviour, Constants.POSE_LAYER, 0.0f, 0.25f,
                "PoseEngine/Poser/Control/Inactive"
            );

            ConfigureLayerControl(
                activeBehaviour, Constants.POSE_LAYER, 1.0f, 0.25f,
                "PoseEngine/Poser/Control/Active"
            );

            AssetDatabase.SaveAssets();
        }

        private void BuildPoseLayer(anim.Builder animBuilder)
        {
            AnimatorControllerLayer poseLayer;
            AnimatorState poseState;

            animBuilder.AddParameter("PoseEngine/Elevation", anim.Builder.FloatParam);
            animBuilder.AddParameter("PoseEngine/PoseState/DelayedEnter", anim.Builder.BoolParam);
            animBuilder.AddLayer("PoseEngine/Poser/Pose", 0.0f, out poseLayer);
            animBuilder.SetLayerAvatarMask(AssetDatabase.LoadAssetAtPath<AvatarMask>(
                AssetDatabase.GUIDToAssetPath(Constants.POSE_AVATAR_MASK_GUID)
            ), poseLayer);
            
            animBuilder.AddDefaultState("PoseEngine_Inactive", poseLayer, out poseState);
            VRCBehaviourUtility.SetParamFlag(poseState, "PoseEngine/PoseState/Exit");
        }

        private AnimatorController ExecuteGenerators(
            anim.Builder animBuilder,
            GameObject avatarRoot,
            VRCExpressionsMenu poseMenu,
            PoseEngineFactory factory
        )
        {
            // Build the pose layer
            BuildPoseLayer(animBuilder);

            // Create the build context
            var poseBuildContext = new PoseBuildContext(
                avatarRoot, animBuilder, poseMenu, factory
            );

            // Run the generators
            foreach (var generator in generators)
            {
                generator.Generate(poseBuildContext);
                AssetDatabase.SaveAssets();
            }

            return poseBuildContext.poseController;
        }

        private void ConfigureLayerControl(
            VRCAnimatorLayerControl behaviour, int layer, float weight,
            float duration, string debugString
        )
        {
            behaviour.playable = VRCAnimatorLayerControl.BlendableLayer.Action;
            behaviour.layer = layer;
            behaviour.goalWeight = weight;
            behaviour.blendDuration = duration;
            behaviour.debugString = debugString;
        }

        private VRCExpressionsMenu CreatePoseMenu(
            GameObject poseEngineInstance,
            PoseEngineFactory factory
        )
        {
            // Set the root menu name
            SetRootMenuName(poseEngineInstance, factory.rootMenuName);

            // Get the pose menu installer
            var poseMenuInstaller = CreatePoseMenuInstaller(
                poseEngineInstance, "Poses"
            );

            if (poseMenuInstaller == null) {
                Debug.LogError("[PoseEngine] Failed to create pose menu installer...");
                return null;
            }

            // Create the pose menu
            var poseMenu = AssetUtility.CreateSerializedClone(
                ScriptableObject.CreateInstance<VRCExpressionsMenu>()
            ) as VRCExpressionsMenu;

            // Apply the pose menu to the pose menu installer
            poseMenuInstaller.Control.subMenu = poseMenu;

            return poseMenu;
        }

        private void SetRootMenuName(
            GameObject poseEngineInstance,
            string menuName
        )
        {
            var rootMenu = poseEngineInstance.transform.GetChild(0)
                .GetComponent<ModularAvatarMenuItem>();
            
            if (rootMenu == null) {
                Debug.LogError("[PoseEngine] Failed to find root menu...");
                return;
            }

            rootMenu.name = menuName;
        }

        private ModularAvatarMenuItem CreatePoseMenuInstaller(
            GameObject poseEngineInstance,
            string menuName
        )
        {
            // find the pose menu container
            var poseObj = poseEngineInstance.transform.GetChild(0).Find("Poses");
            if (poseObj == null) {
                Debug.LogError("[PoseEngine] Failed to find pose menu container...");
                return null;
            }

            // get the pose menu installer
            var poseMenuInstaller = poseObj.GetComponent<ModularAvatarMenuItem>();
            poseMenuInstaller.name = menuName;
            if (poseMenuInstaller == null) {
                Debug.LogError("[PoseEngine] Failed to find pose menu installer...");
                return null;
            }

            // apply the pose menu
            poseMenuInstaller.Control = new VRCExpressionsMenu.Control();
            poseMenuInstaller.Control.type = VRCExpressionsMenu.Control.ControlType.SubMenu;
            poseMenuInstaller.Control.name = "Poses";

            return poseMenuInstaller;
        }

        private ModularAvatarMergeAnimator FindAnimatorMerger(
            GameObject avatarRootObject,
            VRCAvatarDescriptor.AnimLayerType layerType
        )
        {
            var mergerComponents = avatarRootObject.GetComponentsInChildren<ModularAvatarMergeAnimator>();
            if (mergerComponents.Length == 0) return null;

            foreach (var comp in mergerComponents)
            {
                if (comp.layerType == layerType)
                    if (comp.animator == null)
                        return comp;
            }

            return null;
        }
    }
}