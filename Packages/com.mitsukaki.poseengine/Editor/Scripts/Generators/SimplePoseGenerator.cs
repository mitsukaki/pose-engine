
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace com.mitsukaki.poseengine.editor.generators
{
    public class SimplePoseGenerator : IPoseGenerator
    {
        public void Generate(PoseBuildContext context)
        {
            ProcessSimplePoseComponents(
                context.avatarRoot,
                context.poseController,
                context.poseMenu,
                context.factory
            );
        }

        private AnimationClip TranslateMotion(AnimationClip clip, float translation = 1.0f)
        {
            var translatedClip = Object.Instantiate(clip);
            translatedClip.name = clip.name + " (Translated + " + translation + ")";

            var binding = EditorCurveBinding.FloatCurve("", typeof(UnityEngine.Animator), "RootT.y");
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

            AnimationUtility.SetEditorCurve(translatedClip, binding, curve);

            return translatedClip;
        }

        private Motion CreateElevatorBlendTree(AnimationClip clip, string name)
        {
            var blendTree = new BlendTree();
            blendTree.name = name;

            blendTree.blendType = BlendTreeType.Simple1D;
            blendTree.blendParameter = "PoseEngine/Elevation";

            blendTree.AddChild(TranslateMotion(clip, -2.0f), 0.0f);
            blendTree.AddChild(TranslateMotion(clip, 2.0f), 1.0f);

            return blendTree;
        }

        private void ProcessSimplePoseComponents(
            GameObject avatarRootObject,
            AnimatorController actionAnim,
            VRCExpressionsMenu poseMenu,
            PoseEngineFactory factory
        )
        {
            var compList = avatarRootObject.GetComponentsInChildren<PE_SimplePoseList>();
            Debug.Log("[PoseEngine] Processing " + compList.Length + " Simple Pose List components");

            var poseLayer = actionAnim.layers[Constants.POSE_LAYER];
            var poseStateMachine = poseLayer.stateMachine;

            var rootState = poseStateMachine.defaultState;

            int stateIndex = 1;
            foreach (var comp in compList)
            {
                foreach (var pose in comp.poses)
                {
                    if (pose.clips.Length == 0)
                    {
                        Debug.LogError("[PoseEngine] Pose " + pose.name + " has no clips...");
                        continue;
                    }

                    // create the pose menu item
                    var poseMenuItem = new VRCExpressionsMenu.Control();

                    if (factory.deleteNameIfIconSet && pose.icon != null)
                        poseMenuItem.name = "";
                    else poseMenuItem.name = pose.name;

                    poseMenuItem.icon = pose.icon;
                    poseMenuItem.type = VRCExpressionsMenu.Control.ControlType.Button;
                    poseMenuItem.value = stateIndex;
                    poseMenuItem.parameter = new VRCExpressionsMenu.Control.Parameter();
                    poseMenuItem.parameter.name = "PoseEngine/Pose";

                    poseMenu.controls.Add(poseMenuItem);

                    // create the pose state
                    var state = poseStateMachine.AddState(
                        pose.name,
                        ComputeStatePosition(stateIndex - 1, compList.Length * 2)
                    );

                    state.motion = CreateElevatorBlendTree(pose.clips[0], pose.name);
                    state.writeDefaultValues = true; // we let ModularAvatar force it back off as needed

                    VRCBehaviourUtility.SetParamFlag(state, "PoseEngine/PoseState/Enter");

                    var enablingTransition = rootState.AddTransition(state);
                    var disablingTransition = state.AddTransition(rootState);

                    enablingTransition.hasExitTime = false;
                    enablingTransition.duration = 0.25f;
                    enablingTransition.hasFixedDuration = true;
                    enablingTransition.AddCondition(
                        AnimatorConditionMode.Equals,
                        stateIndex,
                        "PoseEngine/Pose"
                    );

                    disablingTransition.hasExitTime = false;
                    disablingTransition.duration = 0.25f;
                    disablingTransition.hasFixedDuration = true;
                    disablingTransition.AddCondition(
                        AnimatorConditionMode.Equals,
                        255,
                        "PoseEngine/Pose"
                    );

                    // we only generate mirrors for single-clip poses
                    if (pose.clips.Length == 1)
                    {
                        var mirroredState = poseStateMachine.AddState(
                            pose.name + "_Mirror",
                            ComputeStatePosition(stateIndex, compList.Length * 2)
                            + new Vector3(0, 50, 0)
                        );

                        mirroredState.motion = CreateElevatorBlendTree(pose.clips[0], pose.name + " (Mirrored)");
                        mirroredState.mirror = true;
                        mirroredState.writeDefaultValues = true; // we let ModularAvatar force it back off as needed

                        VRCBehaviourUtility.SetParamFlag(mirroredState, "PoseEngine/PoseState/Enter");

                        var mirrorTransition = state.AddTransition(mirroredState);
                        var unMirrorTransition = mirroredState.AddTransition(state);
                        var mirrorDisablingTransition = mirroredState.AddTransition(rootState);

                        mirrorTransition.hasExitTime = true;
                        mirrorTransition.hasFixedDuration = true;
                        mirrorTransition.exitTime = 0.25f;
                        mirrorTransition.duration = 0.25f;
                        mirrorTransition.AddCondition(
                            AnimatorConditionMode.Equals,
                            stateIndex,
                            "PoseEngine/Pose"
                        );

                        unMirrorTransition.hasExitTime = true;
                        unMirrorTransition.hasFixedDuration = true;
                        unMirrorTransition.exitTime = 0.25f;
                        unMirrorTransition.duration = 0.25f;
                        unMirrorTransition.AddCondition(
                            AnimatorConditionMode.Equals,
                            stateIndex,
                            "PoseEngine/Pose"
                        );

                        mirrorDisablingTransition.hasExitTime = false;
                        mirrorDisablingTransition.duration = 0.25f;
                        mirrorDisablingTransition.AddCondition(
                            AnimatorConditionMode.Equals,
                            255,
                            "PoseEngine/Pose"
                        );
                    }

                    stateIndex += 2;
                }
            }
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