#region 

using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

#endregion

namespace com.mitsukaki.poseengine.Editor.anim
{
    /// <summary>
    /// A class for building AnimatorControllers.
    /// </summary>
    public partial class Builder
    {
        /// <summary>
        /// The parameter type for a trigger parameter.
        /// </summary>
        public const AnimatorControllerParameterType TriggerParam
            = AnimatorControllerParameterType.Trigger;

        /// <summary>
        /// The parameter type for a boolean parameter.
        /// </summary>
        public const AnimatorControllerParameterType BoolParam
            = AnimatorControllerParameterType.Bool;

        /// <summary>
        /// The parameter type for an integer parameter.
        /// </summary>
        public const AnimatorControllerParameterType IntParam
            = AnimatorControllerParameterType.Int;

        /// <summary>
        /// The parameter type for a float parameter.
        /// </summary>
        public const AnimatorControllerParameterType FloatParam
            = AnimatorControllerParameterType.Float;

        /// <summary>
        /// The AnimatorController being built.
        /// </summary>
        private AnimatorController controller;

        /// <summary>
        /// Creates a new Builder instance from an existing AnimatorController.
        /// </summary>
        /// <param name="controller">The animation controller to build from.</param>
        public Builder(AnimatorController controller)
        {
            this.controller = controller;
        }

        /// <summary>
        /// Creates a new Builder instance with a serialized AnimatorController at the specified path.
        /// </summary>
        /// <param name="path">The path to the serialized AnimatorController.</param>
        /// <returns>The new Builder instance.</returns>
        public static Builder CreateSerialized(string path)
        {
            var controller = AnimatorController.CreateAnimatorControllerAtPath(path);
            return new Builder(controller);
        }

        /// <summary>
        /// Adds a parameter to the AnimatorController being built.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="type">The type of the parameter.</param>
        /// <returns>The Builder instance.</returns>
        public Builder AddParameter(string name, AnimatorControllerParameterType type)
        {
            controller.AddParameter(name, type);
            return this;
        }

        /// <summary>
        /// Gets the AnimatorController that has been built.
        /// </summary>
        /// <returns>The built AnimatorController.</returns>
        public AnimatorController GetController()
        {
            return controller;
        }

        /// <summary>
        /// Implicitly converts a Builder instance to an AnimatorController.
        /// </summary>
        /// <param name="builder">The Builder instance to convert.</param>
        /// <returns>The converted AnimatorController.</returns>
        public static implicit operator AnimatorController(Builder builder)
        {
            return builder.GetController();
        }
    }
}