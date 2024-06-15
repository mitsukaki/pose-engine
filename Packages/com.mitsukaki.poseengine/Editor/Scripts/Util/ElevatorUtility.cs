using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace com.mitsukaki.poseengine.editor
{
    public static class ElevatorUtility
    {
        /// <summary>
        /// Create a blend tree that blends between two motions based on the elevation of the avatar.
        /// </summary>
        /// <param name="downMotion">The motion to play when the avatar is at the lowest elevation</param>
        /// <param name="upMotion">The motion to play when the avatar is at the highest elevation</param>
        /// <param name="buildContext">The pose build context</param>
        /// <param name="name">The name of the blend tree</param>
        /// <returns>The blend tree</returns>
        public static Motion CreateElevatorBlendTree(
            Motion downMotion, Motion upMotion,
            PoseBuildContext buildContext, string name
        )
        {
            // create the blend tree
            var blendTree = new BlendTree
            {
                name = name,
                blendType = BlendTreeType.Simple1D,
                blendParameter = "PoseEngine/Elevation"
            };

            blendTree.AddChild(downMotion, 0.0f);
            blendTree.AddChild(upMotion, 1.0f);

            return blendTree;
        }

        /// <summary>
        /// Translate the motion of a humanoid animation clip by a given amount.
        /// </summary>
        /// <param name="clip">The clip to translate</param>
        /// <param name="translation">The translation to apply</param>
        /// <returns>The translated clip</returns>
        public static AnimationClip TranslateMotion(AnimationClip clip, float translation = 1.0f)
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
        public static void TransposeHumanoidClipKeys(
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
    }
}