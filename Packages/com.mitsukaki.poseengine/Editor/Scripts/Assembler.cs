
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
        /// <summary>
        /// Class responsible for assembling the pose engine for a given avatar.
        /// </summary>
        private List<IPoseGenerator> generators;

        /// <summary>
        /// Initializes a new instance of the <see cref="Assembler"/> class.
        /// </summary>
        /// <param name="generators">The list of pose generators.</param>
        public Assembler(List<IPoseGenerator> generators)
        {
            this.generators = generators;
        }

        /// <summary>
        /// Assembles the pose engine for the specified avatar.
        /// </summary>
        /// <param name="avatarRoot">The root GameObject of the avatar.</param>
        /// <param name="factory">The pose engine factory.</param>
        public void Assemble(GameObject avatarRoot, PoseEngineFactory factory)
        {
            // Create the needed directories
            AssetUtility.CreateGeneratedDirectories();

            // Attach the pose engine prefab to the avatar
            var poseEngineInstance = AssetUtility.AttatchPrefabFromGUID(
                avatarRoot.transform,
                Constants.TEMPLATE_ASSET_GUID
            );

            // Create the animator controller
            var animBuilder = anim.Builder.CreateSerialized(
                "Assets/PoseEngine/Generated/"
                    + AssetUtility.RandomAssetName("controller")
            );

            // Build the control layer
            SetupControlLayer(animBuilder);

            // Create the build context
            var poseBuildContext = new PoseBuildContext(
                avatarRoot, poseEngineInstance, animBuilder, factory
            );

            // Run the pose animation generators
            ExecuteGenerators(poseBuildContext);

            // Skin the menu
            ApplyMenuSkin(poseBuildContext);

            // Delete the names if the icon is set
            if (factory.deleteNameIfIconSet)
                DeleteMenuNames(poseBuildContext);

            // Apply the animator to the animator merger
            var animatorMerger = FindAnimatorMerger(
                avatarRoot,
                VRCAvatarDescriptor.AnimLayerType.Action
            );

            if (animatorMerger == null)
            {
                Debug.LogError("[PoseEngine] Failed to find animator merger...");
                return;
            }

            animatorMerger.animator = animBuilder;
        }

        /// <summary>
        /// Sets up the control layer of the animator controller.
        /// </summary>
        /// <param name="animBuilder">The animator builder.</param>
        private void SetupControlLayer(anim.Builder animBuilder)
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

        /// <summary>
        /// Executes the pose animation generators.
        /// </summary>
        /// <param name="poseBuildContext">The pose build context.</param>
        /// <returns>The animator controller.</returns>
        private AnimatorController ExecuteGenerators(
            PoseBuildContext poseBuildContext
        )
        {
            // Run the generator setup pass
            foreach (var generator in generators)
                generator.Setup(poseBuildContext);

            // Run the generators generate layer pass
            foreach (var generator in generators)
                generator.BuildLayers(poseBuildContext);

            // Run the generators generate states pass
            foreach (var generator in generators)
                generator.BuildStates(poseBuildContext);

            // Run the generator cleanup pass
            foreach (var generator in generators)
                generator.CleanUp(poseBuildContext);

            AssetDatabase.SaveAssets();

            return poseBuildContext.poseController;
        }

        /// <summary>
        /// Configures a layer control behaviour for an animation state.
        /// </summary>
        /// <param name="behaviour">The layer control behaviour.</param>
        /// <param name="layer">The layer index.</param>
        /// <param name="weight">The goal weight.</param>
        /// <param name="duration">The blend duration.</param>
        /// <param name="debugString">The debug string.</param>
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

        /// <summary>
        /// Applies the menu skin to the pose engine root menu.
        /// </summary>
        /// <param name="poseBuildContext">The pose build context.</param>
        private void ApplyMenuSkin(
            PoseBuildContext poseBuildContext
        )
        {
            if (poseBuildContext.factory.skinIcons == null) return;

            var menuContainer = poseBuildContext
                .poseEngineInstance.transform.GetChild(0);

            var skinIcons = poseBuildContext.factory.skinIcons;
            foreach (var skinIcon in skinIcons)
            {
                // if a skin icon or name is not set, skip
                if (skinIcon.icon == null || skinIcon.name == null)
                {
                    Debug.Log("[PoseEngine] Skin icon or name is not set...");
                    continue;
                }

                // find the menu item
                var menuItem = menuContainer.Find(skinIcon.name);
                if (menuItem == null)
                {
                    Debug.Log("[PoseEngine] Failed to find menu item: " + skinIcon.name);
                    continue;
                }

                // apply the icon
                menuItem.GetComponent<ModularAvatarMenuItem>().Control.icon = skinIcon.icon;
            }
        }

        /// <summary>
        /// Deletes the names of the menu items if the icon is set.
        /// </summary>
        /// <param name="poseBuildContext">The pose build context.</param>
        private void DeleteMenuNames(
            PoseBuildContext poseBuildContext
        )
        {
            Debug.Log("[PoseEngine] Deleting menu item names...");
            DeleteChildMenuNames(
                poseBuildContext.poseEngineInstance.transform.GetChild(0)
            );
        }

        /// <summary>
        /// Recursively delete the names of the menu items if the icon is set.
        /// </summary>
        /// <param name="parent">The parent transform to search for menu items.</param>
        private void DeleteChildMenuNames(
            Transform parent
        )
        {
            foreach (Transform child in parent)
            {
                var menuItem = child.GetComponent<ModularAvatarMenuItem>();

                if (menuItem == null) continue;
                if (menuItem.Control.icon == null) continue;

                menuItem.name = "";
                menuItem.Control.name = "";

                DeleteChildMenuNames(child);
            }
        }

        /// <summary>
        /// Sets the name of the root menu.
        /// </summary>
        /// <param name="poseEngineInstance">The pose engine instance.</param>
        /// <param name="menuName">The name of the menu.</param>
        private void SetRootMenuName(
            GameObject poseEngineInstance,
            string menuName
        )
        {
            var rootMenu = poseEngineInstance.transform.GetChild(0)
                .GetComponent<ModularAvatarMenuItem>();

            if (rootMenu == null)
            {
                Debug.LogError("[PoseEngine] Failed to find root menu...");
                return;
            }

            rootMenu.name = menuName;
        }

        /// <summary>
        /// Finds the animator merger component for the specified avatar root object and layer type.
        /// </summary>
        /// <param name="avatarRootObject">The root GameObject of the avatar.</param>
        /// <param name="layerType">The layer type.</param>
        /// <returns>The animator merger component.</returns>
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