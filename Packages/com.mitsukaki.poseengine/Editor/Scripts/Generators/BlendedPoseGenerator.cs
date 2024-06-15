
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;

using static com.mitsukaki.poseengine.editor.anim.Condition;

using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace com.mitsukaki.poseengine.editor.generators
{
    public class BlendedPoseGenerator : IPoseGenerator
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
            // ...
        }

        /// <summary>
        /// Build the states for the simple pose generator.
        /// </summary>
        /// <param name="context">The pose build context.</param>
        /// <returns></returns>
        public void BuildStates(PoseBuildContext context)
        {
            var compList = context.poseEngineInstance
                .GetComponentsInChildren<PEBlendedPose>();

            Debug.Log("[PoseEngine] Processing " + compList.Length + " Blended Pose components");

            foreach (var comp in compList)
                foreach (var pose in comp.poses)
                    CreateBlendedPose(pose, context);
        }

        private void CreateBlendedPose(
            BlendedPose blendedPose,
            PoseBuildContext context
        )
        {
            var animBuilder = context.coreAnimator;
            var layer = animBuilder.GetLayer(Constants.LOCO_LAYER);
            var rootState = layer.stateMachine.defaultState;
            var poseIndex = blendedPose.PoseID;

            // create the parameter if needed
            if (
                blendedPose.DrivingParameterName.Length > 0 &&
                !animBuilder.HasParameter(blendedPose.DrivingParameterName)
            )
                animBuilder.AddParameter(
                    blendedPose.DrivingParameterName, anim.Builder.FloatParam
                );

            // create the blendpose state
            AnimatorState poseState;
            switch (blendedPose.blendType)
            {
                case BlendPoseType.SingleClip:
                    poseState = CreateSingleClipState(blendedPose, context);
                    break;

                case BlendPoseType.MultiClip:
                    poseState = CreateMultiClipState(blendedPose, context);
                    break;

                default:
                    Debug.LogError(
                        "[PoseEngine] Unsupported blended pose type: " + blendedPose.blendType
                    );

                    return;
            }
            
            // add the parameter to the avatar parameters
            ParameterUtility.AddNewParameter(
                context, blendedPose.DrivingParameterName
            );

            // set the parameter drivers
            VRCBehaviourUtility.SetParam(poseState, "PoseEngine/Pose", 0);
            VRCBehaviourUtility.SetParamFlag(poseState, "PoseEngine/PoseState/DelayedEnter");

            // lock feet on entry if needed
            // TODO: disable hard lock on this being true
            if (blendedPose.lockFeetOnEntry || true)
                VRCBehaviourUtility.SetParamFlag(poseState, "PoseEngine/Lock/Feet");

            // entry transition
            animBuilder.StartTransition()
                .FromAny(layer.stateMachine).To(poseState)
                .SetNoExitTime().SetFixedDuration(0.25f)
                .When("PoseEngine/Pose", IsEqualTo, poseIndex)
                .Build();

            // exiting transition
            animBuilder.StartTransition()
                .From(poseState).To(rootState)
                .SetNoExitTime().SetFixedDuration(0.25f)
                .When("PoseEngine/Pose", IsEqualTo, 255)
                .Build();
        }


        private AnimatorState CreateSingleClipState(
            BlendedPose blendedPose,
            PoseBuildContext context
        )
        {
            var clip = blendedPose.clips[0];
            var elevatorMotion = ElevatorUtility.CreateElevatorBlendTree(
                ElevatorUtility.TranslateMotion(clip, -2.0f),
                ElevatorUtility.TranslateMotion(clip, 2.0f),
                context, blendedPose.Name + "_Elevator"
            );

            // create a state for the pose
            AnimatorState state;
            var animBuilder = context.coreAnimator;
            animBuilder.AddState(
                blendedPose.Name,
                context.coreAnimator.GetLayer(Constants.LOCO_LAYER),
                Vector3.zero, out state
            );

            state.writeDefaultValues = false;
            state.motion = elevatorMotion;
            state.speed = 0.0f;
            state.timeParameter = blendedPose.DrivingParameterName;
            state.timeParameterActive = true;

            return state;
        }

        private AnimatorState CreateMultiClipState(
            BlendedPose blendedPose,
            PoseBuildContext context
        )
        {
            var downMotions = new List<AnimationClip>();
            var upMotions = new List<AnimationClip>();
            foreach (var clip in blendedPose.clips)
            {
                downMotions.Add(ElevatorUtility.TranslateMotion(clip, -2.0f));
                upMotions.Add(ElevatorUtility.TranslateMotion(clip, 2.0f));
            }

            // create the blend tree for the motions
            var blendTree = new BlendTree
            {
                name = blendedPose.Name,
                blendType = BlendTreeType.SimpleDirectional2D,
                blendParameter = blendedPose.DrivingParameterName,
                blendParameterY = "PoseEngine/Elevation"
            };

            float step = 1.0f / (float)(blendedPose.clips.Count - 1);
            for (int i = 0; i < blendedPose.clips.Count; i++)
            {
                blendTree.AddChild(downMotions[i], new Vector2(i * step, 0.0f));
                blendTree.AddChild(upMotions[i], new Vector2(i * step, 1.0f));
            }

            // create a state for the pose
            AnimatorState state;
            var animBuilder = context.coreAnimator;
            animBuilder.AddState(
                blendedPose.Name,
                context.coreAnimator.GetLayer(Constants.LOCO_LAYER),
                Vector3.zero, out state
            );

            state.writeDefaultValues = false;
            state.motion = blendTree;

            return state;
        }
    }
}