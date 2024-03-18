
using System.Collections.Generic;

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
        /// Creates an instance of the fluid transition builder to build a
        /// transition between two animator states with chained methods.
        /// </summary>
        public FluidTransitionBuilder StartTransition()
        {
            return new FluidTransitionBuilder(this);
        }
    }


    /// <summary>
    /// Builder class for creating transitions between animator states with a momre fluid UI.
    /// </summary>
    public class FluidTransitionBuilder
    {
        
        private Builder builder;

        public AnimatorState fromState;
        public AnimatorState toState;

        public bool hasFixedDuration = true;
        public bool hasExitTime = false;

        public float duration = 0.0f;
        public float offset = 0.0f;
        public float exitTime = 0.0f;

        private List<AnimatorCondition> conditions;

        private TransitionInterruptionSource interruptionSource;

        private AnimatorStateTransition transition;

        /// <summary>
        /// Constructor for the FluidTransitionBuilder class.
        /// </summary>
        /// <param name="builder">The parent builder object.</param>
        public FluidTransitionBuilder(Builder builder)
        {
            this.builder = builder;
            this.conditions = new List<AnimatorCondition>();
        }

        /// <summary>
        /// Builds and returns the parent builder object.
        /// </summary>
        /// <returns>The parent builder object.</returns>
        public Builder Build()
        {
            if (fromState == null || toState == null)
            {
                throw new System.Exception(
                    "FluidTransitionBuilder: Cannot create transition without both from and to states."
                );
            }

            transition = fromState.AddTransition(toState);
            transition.hasFixedDuration = hasFixedDuration;
            transition.hasExitTime = hasExitTime;
            transition.duration = duration;
            transition.offset = offset;
            transition.exitTime = exitTime;
            transition.interruptionSource = interruptionSource;

            for (int i = 0; i < conditions.Count; i++)
            {
                transition.AddCondition(
                    conditions[i].mode,
                    conditions[i].threshold,
                    conditions[i].parameter
                );
            }

            return builder;
        }

        /// <summary>
        /// Sets the "from" state of the transition.
        /// </summary>
        /// <param name="fromState">The "from" state of the transition.</param>
        /// <returns>The FluidTransitionBuilder object.</returns>
        public FluidTransitionBuilder From(AnimatorState fromState)
        {
            this.fromState = fromState;
            return this;
        }

        /// <summary>
        /// Sets the "to" state of the transition.
        /// </summary>
        /// <param name="toState">The "to" state of the transition.</param>
        /// <returns>The FluidTransitionBuilder object.</returns>
        public FluidTransitionBuilder To(AnimatorState toState)
        {
            this.toState = toState;
            return this;
        }

        /// <summary>
        /// Adds conditions to the transition.
        /// </summary>
        /// <param name="conditions">The conditions to be added.</param>
        /// <returns>The FluidTransitionBuilder object.</returns>
        public FluidTransitionBuilder When(AnimatorCondition[] conditions)
        {
            for (int i = 0; i < conditions.Length; i++)
                this.conditions.Add(conditions[i]);

            return this;
        }

        /// <summary>
        /// Adds a single condition to the transition.
        /// </summary>
        /// <param name="parameter">The parameter of the condition.</param>
        /// <param name="mode">The mode of the condition.</param>
        /// <param name="threshold">The threshold of the condition.</param>
        /// <returns>The FluidTransitionBuilder object.</returns>
        public FluidTransitionBuilder When(
            string parameter, AnimatorConditionMode mode, float threshold
        )
        {
            this.conditions.Add(new AnimatorCondition
            {
                mode = mode,
                threshold = threshold,
                parameter = parameter
            });

            return this;
        }

        /// <summary>
        /// Adds a single condition to the transition.
        /// </summary>
        /// <param name="parameter">The parameter of the condition.</param>
        /// <param name="mode">The mode of the condition.</param>
        /// <param name="threshold">The threshold of the condition.</param>
        /// <returns>The FluidTransitionBuilder object.</returns>
        public FluidTransitionBuilder When(
            string parameter, AnimatorConditionMode mode, bool threshold
        )
        {
            When(parameter, mode, threshold ? 1 : 0);

            return this;
        }

        /// <summary>
        /// Adds a single condition to the transition.
        /// </summary>
        /// <param name="parameter">The parameter of the condition.</param>
        /// <param name="mode">The mode of the condition.</param>
        /// <param name="threshold">The threshold of the condition.</param>
        /// <returns>The FluidTransitionBuilder object.</returns>
        public FluidTransitionBuilder When(
           string parameter, AnimatorConditionMode mode, int threshold
        )
        {
            this.conditions.Add(new AnimatorCondition
            {
                mode = mode,
                threshold = threshold,
                parameter = parameter
            });

            return this;
        }

        

        /// <summary>
        /// Sets the duration of the transition.
        /// </summary>
        /// <param name="duration">The duration of the transition.</param>
        /// <returns>The FluidTransitionBuilder object.</returns>
        public FluidTransitionBuilder SetDuration(float duration)
        {
            this.duration = duration;
            return this;
        }

        /// <summary>
        /// Sets the offset of the transition.
        /// </summary>
        /// <param name="offset">The offset of the transition.</param>
        /// <returns>The FluidTransitionBuilder object.</returns>
        public FluidTransitionBuilder SetOffset(float offset)
        {
            this.offset = offset;
            return this;
        }

        /// <summary>
        /// Sets the exit time of the transition.
        /// </summary>
        /// <param name="exitTime">The exit time of the transition.</param>
        /// <returns>The FluidTransitionBuilder object.</returns>
        public FluidTransitionBuilder SetExitTime(float exitTime)
        {
            this.hasExitTime = exitTime != 0;
            this.exitTime = exitTime;
            return this;
        }

        /// <summary>
        /// Sets the transition to have no exit time.
        /// </summary>
        /// <returns>The FluidTransitionBuilder object.</returns>
        public FluidTransitionBuilder SetNoExitTime()
        {
            this.hasExitTime = false;
            this.exitTime = 0.0f;
            return this;
        }

        /// <summary>
        /// Sets the transition to have a fixed duration.
        /// </summary>
        /// <param name="duration">The fixed duration of the transition.</param>
        /// <returns>The FluidTransitionBuilder object.</returns>
        public FluidTransitionBuilder SetFixedDuration(float duration)
        {
            this.hasFixedDuration = true;
            this.duration = duration;
            return this;
        }

        /// <summary>
        /// Sets the transition to have a percentage duration.
        /// </summary>
        /// <param name="duration">The percentage duration of the transition.</param>
        /// <returns>The FluidTransitionBuilder object.</returns>
        public FluidTransitionBuilder SetPercentageDuration(float duration)
        {
            this.hasFixedDuration = false;
            this.duration = duration;
            return this;
        }

        /// <summary>
        /// Sets the interruption source of the transition.
        /// </summary>
        /// <param name="source">The interruption source of the transition.</param>
        /// <returns>The FluidTransitionBuilder object.</returns>
        public FluidTransitionBuilder SetInterruptionSource(
            TransitionInterruptionSource source
        )
        {
            this.interruptionSource = source;
            return this;
        }
    }
}