
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace com.mitsukaki.poseengine.editor.anim
{
    public partial class Builder
    {
        /// <summary>
        /// Adds a state to the animator controller layer.
        /// </summary>
        /// <param name="stateName">The name of the state.</param>
        /// <param name="layer">The animator controller layer.</param>
        /// <param name="state">The added animator state.</param>
        /// <returns>The builder instance.</returns>
        public Builder AddState(
            string stateName,
            AnimatorControllerLayer layer,
            out AnimatorState state
        )
        {
            state = layer.stateMachine.AddState(stateName);
            return this;
        }

        /// <summary>
        /// Adds a state to the animator controller layer at a specific position.
        /// </summary>
        /// <param name="stateName">The name of the state.</param>
        /// <param name="layer">The animator controller layer.</param>
        /// <param name="position">The position of the state.</param>
        /// <param name="state">The added animator state.</param>
        /// <returns>The builder instance.</returns>
        public Builder AddState(
            string stateName,
            AnimatorControllerLayer layer,
            Vector3 position,
            out AnimatorState state
        )
        {
            state = layer.stateMachine.AddState(stateName, position);
            return this;
        }

        /// <summary>
        /// Adds a state to the animator controller layer at a specific position.
        /// </summary>
        /// <param name="stateName">The name of the state.</param>
        /// <param name="layer">The animator controller layer.</param>
        /// <param name="x">The x-coordinate of the position.</param>
        /// <param name="y">The y-coordinate of the position.</param>
        /// <param name="state">The added animator state.</param>
        /// <returns>The builder instance.</returns>
        public Builder AddState(
            string stateName,
            AnimatorControllerLayer layer,
            int x, int y,
            out AnimatorState state
        )
        {
            state = layer.stateMachine.AddState(stateName, new Vector3(x, y, 0));
            return this;
        }

        /// <summary>
        /// Adds a default state to the animator controller layer.
        /// </summary>
        /// <param name="stateName">The name of the state.</param>
        /// <param name="layer">The animator controller layer.</param>
        /// <param name="state">The added animator state.</param>
        /// <returns>The builder instance.</returns>
        public Builder AddDefaultState(
            string stateName,
            AnimatorControllerLayer layer,
            out AnimatorState state
        )
        {
            state = layer.stateMachine.AddState(stateName);
            layer.stateMachine.defaultState = state;
            return this;
        }

        /// <summary>
        /// Adds a default state to the animator controller layer at a specific position.
        /// </summary>
        /// <param name="stateName">The name of the state.</param>
        /// <param name="layer">The animator controller layer.</param>
        /// <param name="position">The position of the state.</param>
        /// <param name="state">The added animator state.</param>
        /// <returns>The builder instance.</returns>
        public Builder AddDefaultState(
            string stateName,
            AnimatorControllerLayer layer,
            Vector3 position,
            out AnimatorState state
        )
        {
            state = layer.stateMachine.AddState(stateName, position);
            layer.stateMachine.defaultState = state;
            return this;
        }

        /// <summary>
        /// Adds a default state to the animator controller layer at a specific position.
        /// </summary>
        /// <param name="stateName">The name of the state.</param>
        /// <param name="layer">The animator controller layer.</param>
        /// <param name="x">The x-coordinate of the position.</param>
        /// <param name="y">The y-coordinate of the position.</param>
        /// <param name="state">The added animator state.</param>
        /// <returns>The builder instance.</returns>
        public Builder AddDefaultState(
            string stateName,
            AnimatorControllerLayer layer,
            int x, int y,
            out AnimatorState state
        )
        {
            state = layer.stateMachine.AddState(stateName, new Vector3(x, y, 0));
            layer.stateMachine.defaultState = state;
            return this;
        }

        /// <summary>
        /// Adds a state behavior to an animator state.
        /// </summary>
        /// <typeparam name="T">The type of the state behavior.</typeparam>
        /// <param name="state">The animator state.</param>
        /// <param name="behaviour">The added state behavior.</param>
        /// <returns>The builder instance.</returns>
        public Builder AddStateBehaviour<T>(
            AnimatorState state, out T behaviour
        ) where T : UnityEngine.StateMachineBehaviour
        {
            behaviour = state.AddStateMachineBehaviour<T>();
            return this;
        }

        /// <summary>
        /// Sets the motion for an animator state.
        /// </summary>
        /// <param name="state">The animator state.</param>
        /// <param name="motion">The motion to set.</param>
        /// <returns>The builder instance.</returns>
        public Builder SetStateMotion(
            AnimatorState state, Motion motion,
            bool writeDefaultValues = true
        )
        {
            state.motion = motion;
            state.writeDefaultValues = writeDefaultValues;
            return this;
        }
    }
}