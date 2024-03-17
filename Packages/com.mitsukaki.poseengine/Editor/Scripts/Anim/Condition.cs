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
    /// Provides static methods and properties for defining animator conditions.
    /// </summary>
    public static class Condition
    {
        /// <summary>
        /// Represents the "Greater Than" condition for animator parameters.
        /// </summary>
        public static AnimatorConditionMode IsGreaterThan
            = AnimatorConditionMode.Greater;

        /// <summary>
        /// Represents the "Less Than" condition for animator parameters.
        /// </summary>
        public static AnimatorConditionMode IsLessThan
            = AnimatorConditionMode.Less;

        /// <summary>
        /// Represents the "Equal To" condition for animator parameters.
        /// </summary>
        public static AnimatorConditionMode IsEqualTo
            = AnimatorConditionMode.Equals;

        /// <summary>
        /// Represents the "Not Equal To" condition for animator parameters.
        /// </summary>
        public static AnimatorConditionMode IsNotEqualTo
            = AnimatorConditionMode.NotEqual;

        /// <summary>
        /// Represents the "True" condition for animator parameters.
        /// </summary>
        public static AnimatorConditionMode IsTrue
            = AnimatorConditionMode.If;

        /// <summary>
        /// Represents the "False" condition for animator parameters.
        /// </summary>
        public static AnimatorConditionMode IsFalse
            = AnimatorConditionMode.IfNot;
    }
}