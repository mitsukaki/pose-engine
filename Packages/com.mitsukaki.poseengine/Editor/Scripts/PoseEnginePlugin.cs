
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

using nadena.dev.ndmf;
using nadena.dev.modular_avatar.core;

using com.mitsukaki.poseengine.editor;
using com.mitsukaki.poseengine.editor.generators;

using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDK3.Avatars.Components;
using static VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;
using static VRC.SDK3.Avatars.Components.VRCAnimatorLayerControl;

// using ExpressionsMenu = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu;
// using ExpressionsMenuControl = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control;
// using ExpressionParameters = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters;
// using ExpressionParameter = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.Parameter;

[assembly: ExportsPlugin(typeof(PoseEnginePlugin))]

namespace com.mitsukaki.poseengine.editor
{
    public class PoseEnginePlugin : Plugin<PoseEnginePlugin>
    {
        /// <summary>
        /// This name is used to identify the plugin internally, and can be used to declare BeforePlugin/AfterPlugin
        /// dependencies. If not set, the full type name will be used.
        /// </summary>
        public override string QualifiedName => "com.mitsukaki.poseengine.editor";

        /// <summary>
        /// The plugin name shown in debug UIs. If not set, the qualified name will be shown.
        /// </summary>
        public override string DisplayName => "Pose Engine";

        protected override void Configure()
        {
            InPhase(BuildPhase.Generating).Run("Generate/Apply pose engine", ctx =>
            {
                // check if avatar already has a pose engine
                if (ctx.AvatarRootObject.transform.Find("PoseEnginePrefab") != null)
                {
                    Debug.Log("[PoseEngine] Leaving the existing pose engine prefab.\nRemove the existing pose engine prefab to generate an updated one.");
                    return;
                }

                Generate(ctx.AvatarRootObject);
            });
        }

        public void Generate(GameObject avatarRootObject)
        {
            var assembler = new Assembler(
                new List<IPoseGenerator>{
                    new SimplePoseGenerator()
                }
            );

            assembler.Assemble(avatarRootObject);

            AssetDatabase.SaveAssets();
        }

        private Vector3 ComputeStatePosition(int index, int itemCount)
        {
            // arrange in a square grid
            int rows = Mathf.CeilToInt(Mathf.Sqrt(itemCount));

            int x = index % rows;
            int y = Mathf.FloorToInt(index / rows);

            return new Vector3(
                x * 200 - 200,
                y * 50 - 200,
                0
            );
        }
    }
}