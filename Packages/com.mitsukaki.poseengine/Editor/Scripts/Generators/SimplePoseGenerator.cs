
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
            // ...
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
            AnimatorControllerLayer poseLayer;
            AnimatorState poseState;

            var animBuilder = context.poseController;

            animBuilder.AddParameter("PoseEngine/Elevation", anim.Builder.FloatParam);
            animBuilder.AddParameter("PoseEngine/PoseState/DelayedEnter", anim.Builder.BoolParam);
            animBuilder.AddLayer("PoseEngine/Poser/Pose", 0.0f, out poseLayer);
            animBuilder.SetLayerAvatarMask(AssetDatabase.LoadAssetAtPath<AvatarMask>(
                AssetDatabase.GUIDToAssetPath(Constants.POSE_AVATAR_MASK_GUID)
            ), poseLayer);

            animBuilder.AddDefaultState("PoseEngine_Inactive", poseLayer, out poseState);
            VRCBehaviourUtility.SetParamFlag(poseState, "PoseEngine/PoseState/Exit");
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
                    PopulateSimplePoseLayer(compList.Length, context, pose);
        }

        /// <summary>
        /// Create a new animation clip that moves the root transform so that the head is centered
        /// on the X-Y plane.
        /// </summary>
        /// <param name="clip">The clip to center</param>
        /// <returns>The centered clip</returns>
        private AnimationClip HeadCenterAnimation(
            AnimationClip clip,
            Animator animator,
            GameObject avatarRootObject
        )
        {
            // if not human, skip the processing
            if (!animator.isHuman) return clip;

            // sample the animation on the proxy at frame 0
            var controller = animator.runtimeAnimatorController;
            var animAvatar = animator.avatar;
            clip.SampleAnimation(avatarRootObject, 0);

            // get the distance between the head and the root
            var headBone = animator.GetBoneTransform(HumanBodyBones.Head);
            var rootBone = animator.GetBoneTransform(HumanBodyBones.Hips);

            var distance = headBone.position - rootBone.position;

            // create a new animation clip
            var centeredClip = Object.Instantiate(clip);
            centeredClip.name = clip.name + "_HC";

            // translate the root transform on all axis
            var xBinding = EditorCurveBinding.FloatCurve("", typeof(UnityEngine.Animator), "RootT.x");
            var yBinding = EditorCurveBinding.FloatCurve("", typeof(UnityEngine.Animator), "RootT.y");
            var zBinding = EditorCurveBinding.FloatCurve("", typeof(UnityEngine.Animator), "RootT.z");

            TransposeHumanoidClipKeys(xBinding, centeredClip, distance.x);
            TransposeHumanoidClipKeys(yBinding, centeredClip, distance.y);
            TransposeHumanoidClipKeys(zBinding, centeredClip, distance.z);

            return centeredClip;
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

        private AnimatorState MakePoseState(
            PoseBuildContext context, AnimatorControllerLayer layer,
            SimplePose pose, Vector3 position, bool isMirrored
        )
        {
            AnimatorState state;
            string suffix = isMirrored ? "_M" : "";

            var animBuilder = context.poseController;
            animBuilder.AddState(
                pose.Name + suffix, layer, position, out state
            );

            state.motion = CreateElevatorBlendTree(
                pose.clip, context, pose.Name + suffix
            );

            VRCBehaviourUtility.SetParam(state, "PoseEngine/Pose", 0);
            VRCBehaviourUtility.SetParamFlag(state, "PoseEngine/PoseState/DelayedEnter");

            state.mirror = isMirrored;

            return state;
        }

        private void CreateMirroringTransitions(
            anim.Builder animBuilder, AnimatorState mainState,
            AnimatorState mirrorState, AnimatorState rootState,
            int stateIndex
        )
        {
            // swap to mirror transition
            animBuilder.StartTransition()
                .From(mainState).To(mirrorState)
                .SetExitTime(0.25f).SetFixedDuration(0.25f)
                .When("PoseEngine/Pose", IsEqualTo, stateIndex)
                .Build();

            // unswap from mirror transition
            animBuilder.StartTransition()
                .From(mirrorState).To(mainState)
                .SetExitTime(0.25f).SetFixedDuration(0.25f)
                .When("PoseEngine/Pose", IsEqualTo, stateIndex)
                .Build();
        }

        private void CreateSwappingExitTransitions(
            anim.Builder animBuilder, AnimatorState mainState,
            AnimatorState mirrorState, AnimatorState rootState,
            int stateIndex
        )
        {
            // swapping pose exit transition
            animBuilder.StartTransition()
                .From(mainState).To(rootState)
                .SetNoExitTime().SetFixedDuration(0.05f)
                .When("PoseEngine/Pose", IsNotEqualTo, stateIndex)
                .When("PoseEngine/Pose", IsNotEqualTo, 0)
                .Build();

            // mirror swapping pose exit transition
            animBuilder.StartTransition()
                .From(mirrorState).To(rootState)
                .SetNoExitTime().SetFixedDuration(0.05f)
                .When("PoseEngine/Pose", IsNotEqualTo, stateIndex)
                .When("PoseEngine/Pose", IsNotEqualTo, 0)
                .Build();
        }

        private void CreateExitingTransitions(
            anim.Builder animBuilder, AnimatorState mainState,
            AnimatorState mirrorState, AnimatorState rootState,
            int stateIndex
        )
        {
            // main exiting transition
            animBuilder.StartTransition()
                .From(mainState).To(rootState)
                .SetNoExitTime().SetFixedDuration(0.25f)
                .When("PoseEngine/Pose", IsEqualTo, 255)
                .Build();

            // mirror exiting transition
            animBuilder.StartTransition()
                .From(mirrorState).To(rootState)
                .SetNoExitTime().SetFixedDuration(0.25f)
                .When("PoseEngine/Pose", IsEqualTo, 255)
                .Build();
        }

        private void PopulateSimplePoseLayer(
            int componentCount, PoseBuildContext context, SimplePose pose
        )
        {
            int stateIndex = pose.PoseID;
            var animBuilder = context.poseController;

            // create the pose states
            var layer = animBuilder.GetLayer(Constants.POSE_LAYER);
            AnimatorState mainState = MakePoseState(
                context, layer, pose,
                ComputeStatePosition(stateIndex - 1, componentCount * 2), false
            );

            AnimatorState mirrorState = MakePoseState(
                context, layer, pose,
                ComputeStatePosition(stateIndex, componentCount * 2), true
            );

            // set up transitions
            var rootState = layer.stateMachine.defaultState;

            // entry transition
            animBuilder.StartTransition()
                .From(rootState).To(mainState)
                .SetNoExitTime().SetFixedDuration(0.25f)
                .When("PoseEngine/Pose", IsEqualTo, stateIndex)
                .Build();

            // Create exiting transitions
            CreateExitingTransitions(
                animBuilder, mainState, mirrorState, rootState, stateIndex
            );

            // Create mirroring transitions
            CreateMirroringTransitions(
                animBuilder, mainState, mirrorState, rootState, stateIndex
            );

            // Create swapping exit transitions
            CreateSwappingExitTransitions(
                animBuilder, mainState, mirrorState, rootState, stateIndex
            );
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