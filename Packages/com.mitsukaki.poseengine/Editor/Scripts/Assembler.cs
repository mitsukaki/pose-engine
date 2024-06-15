
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
                Constants.SIMPLE_POSE_PREFAB_GUID
            );

            // Get/initialize the core animator
            var coreAnimator = GetCoreAnimator(avatarRoot);
            var animBuilder = new anim.Builder(coreAnimator);

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
            var generatedLocoObject = FindChildWithNameRecursive(
                "Generated Loco", avatarRoot.transform
            );

            if (generatedLocoObject == null)
            {
                Debug.LogError("[PoseEngine] Failed to find generated loco object...");
                return;
            }
            
            var animatorMerger = FindAnimatorMerger(
                generatedLocoObject, VRCAvatarDescriptor.AnimLayerType.Base
            );

            if (animatorMerger == null)
            {
                Debug.LogError("[PoseEngine] Failed to find animator merger...");
                return;
            }

            animatorMerger.animator = animBuilder;
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

            return poseBuildContext.coreAnimator;
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
                .poseEngineInstance.transform.GetChild(1);

            var skinIcons = poseBuildContext.factory.skinIcons;
            foreach (var skinIcon in skinIcons)
            {
                // if a skin icon or name is not set, skip
                if (skinIcon.icon == null || skinIcon.name == null)
                   continue;

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
                poseBuildContext.poseEngineInstance.transform.GetChild(1)
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
            GameObject searchRoot,
            VRCAvatarDescriptor.AnimLayerType layerType
        )
        {
            var mergerComponents = searchRoot.GetComponentsInChildren<ModularAvatarMergeAnimator>();
            if (mergerComponents.Length == 0) return null;

            foreach (var comp in mergerComponents)
            {
                if (comp.layerType == layerType)
                    if (comp.animator == null)
                        return comp;
            }

            return null;
        }

        /// <summary>
        /// Finds a child GameObject with the specified name.
        /// </summary>
        /// <param name="name">The name of the child GameObject.</param>
        /// <param name="parent">The parent transform to search for the child GameObject.</param>
        /// <returns>The child GameObject with the specified name.</returns>
        private GameObject FindChildWithNameRecursive(
            string name,
            Transform parent
        )
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                    return child.gameObject;

                var result = FindChildWithNameRecursive(name, child);
                if (result != null)
                    return result;
            }

            return null;
        }

        private AnimatorController GetCoreAnimator(GameObject avatarRoot)
        {
            var baseLocos = avatarRoot.GetComponentsInChildren<PEBaseLocomotion>();
            string assetPath;

            // if no loco, we copy the default
            if (baseLocos.Length == 0)
                assetPath = AssetDatabase.GUIDToAssetPath(Constants.DEFAULT_BASE_ANIM_GUID);

            // else we copy the provided one
            else
            {
                var animator = baseLocos[0].BaseLayerAnimator;
                assetPath = AssetDatabase.GetAssetPath(animator);
            }

            // duplicate the animator
            var controllerName = AssetUtility.RandomAssetName("controller");
            var newPath = "Assets/PoseEngine/Generated/" + controllerName;
            if (!AssetDatabase.CopyAsset(assetPath, newPath )) return null;

            return AssetDatabase.LoadAssetAtPath<AnimatorController>(newPath);
        }
    }
}