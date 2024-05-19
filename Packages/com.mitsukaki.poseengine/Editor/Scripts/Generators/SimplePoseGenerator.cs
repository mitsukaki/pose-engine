
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

using static com.mitsukaki.poseengine.editor.anim.Condition;

using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace com.mitsukaki.poseengine.editor.generators
{
    public class SimplePoseGenerator : IPoseGenerator
    {
        public void Setup(PoseBuildContext context)
        {
            AnimatorState poseState;

            var animBuilder = context.coreAnimator;

            animBuilder.AddParameter("PoseEngine/Pose", anim.Builder.IntParam);
            animBuilder.AddParameter("PoseEngine/Mirrored", anim.Builder.BoolParam);
            animBuilder.AddParameter("PoseEngine/Elevation", anim.Builder.FloatParam);
            animBuilder.AddParameter("PoseEngine/Lock/Feet", anim.Builder.BoolParam);
            animBuilder.AddParameter("PoseEngine/PoseState/DelayedEnter", anim.Builder.BoolParam);
        }

        /// <summary>
        /// Clean up the layers for the simple pose generator.
        /// </summary>
        /// <param name="context">The pose build context.</param>
        /// <returns></returns>
        public void CleanUp(PoseBuildContext context)
        {
            // ...
        }

        /// <summary>
        /// Build the layers for the simple pose generator.
        /// </summary>
        /// <param name="context">The pose build context.</param>
        /// <returns></returns>
        public void BuildLayers(PoseBuildContext context)
        {
            var animBuilder = context.coreAnimator;
            var layer = animBuilder.GetLayer(Constants.LOCO_LAYER);
            layer.name = "PoseEngine/Locomotion";
        }

        /// <summary>
        /// Build the states for the simple pose generator.
        /// </summary>
        /// <param name="context">The pose build context.</param>
        /// <returns></returns>
        public void BuildStates(PoseBuildContext context)
        {
            var compList = context.poseEngineInstance
                .GetComponentsInChildren<PESimplePoseList>();

            Debug.Log("[PoseEngine] Processing " + compList.Length + " Simple Pose List components");

            foreach (var comp in compList)
                foreach (var pose in comp.poses)
                {
                    CreateSimplePoseState(compList.Length, context, pose, false);
                    CreateSimplePoseState(compList.Length, context, pose, true);
                }
                
        }


        private void CreateSimplePoseState(
            int componentCount,
            PoseBuildContext context,
            SimplePose pose,
            bool isMirrored
        )
        {
            int stateIndex = pose.PoseID;
            var animBuilder = context.coreAnimator;

            // create the pose state
            var layer = animBuilder.GetLayer(Constants.LOCO_LAYER);
            var rootState = layer.stateMachine.defaultState;
            var statePos = ComputeStatePosition(
                stateIndex * 2 + (isMirrored ? 1 : 0), componentCount * 2
            );
            
            Debug.Log("[PoseEngine] Creating pose state " + pose.Name + " at " + statePos.ToString() + " (mirrored: " + isMirrored + ")");
            AnimatorState poseState = MakePoseState(
                context, layer, pose, statePos, isMirrored
            );

            // entry transition
            animBuilder.StartTransition()
                .FromAny(layer.stateMachine).To(poseState)
                .SetNoExitTime().SetFixedDuration(0.25f)
                .When("PoseEngine/Pose", IsEqualTo, stateIndex)
                .When("PoseEngine/Mirrored", isMirrored)
                .Build();

            // exiting transition
            animBuilder.StartTransition()
                .From(poseState).To(rootState)
                .SetNoExitTime().SetFixedDuration(0.25f)
                .When("PoseEngine/Pose", IsEqualTo, 255)
                .Build();
        }

        private AnimatorState MakePoseState(
            PoseBuildContext context, AnimatorControllerLayer layer,
            SimplePose pose, Vector3 position, bool isMirrored
        )
        {
            AnimatorState state;
            string suffix = isMirrored ? "_M" : "";

            var animBuilder = context.coreAnimator;
            animBuilder.AddState(
                pose.Name + suffix, layer, position, out state
            );

            state.writeDefaultValues = false;
            state.motion = CreateElevatorBlendTree(
                pose.clip, context, pose.Name + suffix
            );

            // set the parameter drivers
            VRCBehaviourUtility.SetParam(state, "PoseEngine/Pose", 0);
            VRCBehaviourUtility.SetParamFlag(state, "PoseEngine/PoseState/DelayedEnter");

            // lock feet on entry if needed
            // TODO: disable hard lock on this being true
            if (!isMirrored && pose.lockFeetOnEntry || true)
                VRCBehaviourUtility.SetParamFlag(state, "PoseEngine/Lock/Feet");

            // add behaviour to set the mirrored flag (inverted to current value)
            VRCBehaviourUtility.SetParam(state, "PoseEngine/Mirrored", !isMirrored);

            state.mirror = isMirrored;

            return state;
        }

        private Motion CreateElevatorBlendTree(
            AnimationClip clip, PoseBuildContext buildContext, string name
        ) 
        {
            // create the blend tree
            var blendTree = new BlendTree();
            blendTree.name = name;

            blendTree.blendType = BlendTreeType.Simple1D;
            blendTree.blendParameter = "PoseEngine/Elevation";

            blendTree.AddChild(TranslateMotion(clip, -2.0f), 0.0f);
            blendTree.AddChild(TranslateMotion(clip, 2.0f), 1.0f);

            return blendTree;
        }

        /// <summary>
        /// Translate the motion of a humanoid animation clip by a given amount.
        /// </summary>
        /// <param name="clip">The clip to translate</param>
        /// <param name="translation">The translation to apply</param>
        /// <returns>The translated clip</returns>
        private AnimationClip TranslateMotion(AnimationClip clip, float translation = 1.0f)
        {
            var translatedClip = Object.Instantiate(clip);
            translatedClip.name = clip.name + "_T" + translation;

            var binding = EditorCurveBinding.FloatCurve("", typeof(UnityEngine.Animator), "RootT.y");
            TransposeHumanoidClipKeys(binding, translatedClip, translation);

            return translatedClip;
        }

        /// <summary>
        /// Transpose the keys of a humanoid animation clip by a given translation.
        /// </summary>
        /// <param name="binding">The binding to transpose</param>
        /// <param name="clip">The clip to transpose</param>
        /// <param name="translation">The translation to apply</param>
        /// <returns></returns>
        private void TransposeHumanoidClipKeys(
            EditorCurveBinding binding, AnimationClip clip, float translation
        )
        {
            AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);

            // iterate over all keys and add the translation
            if (curve != null)
            {
                for (int i = 0; i < curve.keys.Length; i++)
                {
                    Keyframe key = curve.keys[i];
                    key.value += translation;

                    curve.MoveKey(i, key);
                }
            }
            else
            {
                curve = new AnimationCurve();
                curve.AddKey(0, translation);
                curve.AddKey(clip.length, translation);
            }

            AnimationUtility.SetEditorCurve(clip, binding, curve);
        }

        private Vector3 ComputeStatePosition(int index, int itemCount)
        {
            // arrange in a square grid
            int columns = Mathf.CeilToInt(Mathf.Sqrt(itemCount));

            int x = index % columns;
            int y = index / columns;

            return new Vector3(x * 200, y * 50 + 50, 0);
        }
    }
}