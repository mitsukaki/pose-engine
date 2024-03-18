
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
        /// Builder for creating transitions between states. Allows specifying
        /// transition duration, exit time, and offset.
        /// </summary>
        /// <param name="fromState">The state to transition from.</param>
        /// <param name="toState">The state to transition to.</param>
        /// <param name="isFixedDuration">
        /// Determines whether the duration of the transition is reported in a
        /// fixed duration in seconds or as a normalized time.
        /// </param>
        /// <param name="hasExitTime">Whether the transition should be delayed
        /// in seconds before unity begins checking it.</param>
        /// <param name="duration">How long the transition should take.</param>
        /// <param name="exitTime">
        /// The normalized time (percentage of the overall transition length)
        /// at which unity should test the transition conditions.
        /// </param>
        /// <param name="offset">
        /// The time at which the destination state will start.
        /// </param>
        /// <returns>A unity animator state transition object.</returns>
        public AnimatorStateTransition Transition(
            AnimatorState fromState, AnimatorState toState, bool isFixedDuration,
            bool hasExitTime, float duration, float exitTime, float offset
        )
        {
            var transition = fromState.AddTransition(toState);

            transition.hasFixedDuration = isFixedDuration;
            transition.duration = 0.0f;
            transition.offset = 0.0f;
            transition.hasExitTime = hasExitTime;
            transition.exitTime = (hasExitTime) ? exitTime : 0.0f;

            return transition;
        }

        /// <summary>
        /// Builder for creating transitions between states. Allows specifying
        /// transition duration, exit time, and offset. Uses default values
        /// for offset and exit time.
        /// </summary>
        /// <param name="fromState">The state to transition from.</param>
        /// <param name="toState">The state to transition to.</param>
        /// <param name="isFixedDuration">
        /// Determines whether the duration of the transition is reported in a
        /// fixed duration in seconds or as a normalized time.
        /// </param>
        /// <param name="hasExitTime">Whether the transition should be delayed
        /// in seconds before unity begins checking it.</param>
        /// <param name="duration">How long the transition should take.</param>
        /// <param name="exitTime">
        /// The normalized time (percentage of the overall transition length)
        /// at which unity should test the transition conditions.
        /// </param>
        /// <returns>A unity animator state transition object.</returns>
        public AnimatorStateTransition Transition(
            AnimatorState fromState, AnimatorState toState,
            bool isFixedDuration, bool hasExitTime, float duration,
            float exitTime
        )
        {
            return Transition(
                fromState, toState, isFixedDuration,
                hasExitTime, duration, exitTime, 0.0f
            );
        }

        /// <summary>
        /// Builder for creating transitions between states. Uses default
        /// values for duration, exit time, and offset.
        /// </summary>
        /// <param name="fromState">The state to transition from.</param>
        /// <param name="toState">The state to transition to.</param>
        /// <returns>A unity animator state transition object.</returns>
        public AnimatorStateTransition Transition(
            AnimatorState fromState, AnimatorState toState
        )
        {
            return Transition(
                fromState, toState, false, false, 0.0f, 0.0f, 0.0f
            );
        }

        /// <summary>
        /// Builder for creating transitions between states with a fixed
        /// duration.
        /// </summary>
        /// <param name="fromState">The state to transition from.</param>
        /// <param name="toState">The state to transition to.</param>
        /// <param name="duration">How long the transition should take.</param>
        /// <returns>A unity animator state transition object.</returns>
        public AnimatorStateTransition TransitionFixed(
            AnimatorState fromState,
            AnimatorState toState,
            float duration
        )
        {
            return Transition(
                fromState, toState, true, false, duration, 0.0f, 0.0f
            );
        }

        /// <summary>
        /// Builder for creating transitions between states with a duration
        /// specified as a percentage of the overall transition length.
        /// </summary>
        /// <param name="fromState">The state to transition from.</param>
        /// <param name="toState">The state to transition to.</param>
        /// <param name="duration">The duration of the transition as a
        /// percentage of the overall transition length.</param>
        /// <returns>A unity animator state transition object.</returns>
        public AnimatorStateTransition TransitionPercentage(
            AnimatorState fromState,
            AnimatorState toState,
            float duration
        )
        {
            return Transition(
                fromState, toState, false, false, duration, 0.0f, 0.0f
            );
        }

        /// <summary>
        /// Builder for creating transitions between states and assigns the
        /// created transition to an output parameter.
        /// </summary>
        /// <param name="fromState">The state to transition from.</param>
        /// <param name="toState">The state to transition to.</param>
        /// <param name="transition">
        /// The created animator state transition object.
        /// </param>
        /// <returns>The builder instance.</returns>
        public Builder Transition(
            AnimatorState fromState,
            AnimatorState toState,
            out AnimatorStateTransition transition
        )
        {
            transition = Transition(fromState, toState);
            return this;
        }

        /// <summary>
        /// Builder for creating transitions between states with a fixed
        /// duration and assigns the created transition to an output parameter.
        /// </summary>
        /// <param name="fromState">The state to transition from.</param>
        /// <param name="toState">The state to transition to.</param>
        /// <param name="duration">How long the transition should take.</param>
        /// <param name="transition">
        /// The created animator state transition object.
        /// </param>
        /// <returns>The builder instance.</returns>
        public Builder TransitionFixed(
            AnimatorState fromState,
            AnimatorState toState,
            float duration,
            out AnimatorStateTransition transition
        )
        {
            transition = TransitionFixed(fromState, toState, duration);
            return this;
        }

        /// <summary>
        /// Builder for creating transitions between states with a duration
        /// specified as a percentage of the overall transition length and
        /// assigns the created transition to an output parameter.
        /// </summary>
        /// <param name="fromState">The state to transition from.</param>
        /// <param name="toState">The state to transition to.</param>
        /// <param name="duration">The duration of the transition as a
        /// percentage of the overall transition length.</param>
        /// <param name="transition">
        /// The created animator state transition object.
        /// </param>
        /// <returns>The builder instance.</returns>
        public Builder TransitionPercentage(
            AnimatorState fromState,
            AnimatorState toState,
            float duration,
            out AnimatorStateTransition transition
        )
        {
            transition = TransitionPercentage(fromState, toState, duration);
            return this;
        }

        /// <summary>
        /// Sets the duration of a transition to a fixed value.
        /// </summary>
        /// <param name="duration">
        /// The fixed duration of the transition in seconds.
        /// </param>
        /// <param name="transition">The animator state transition object.</param>
        /// <returns>The builder instance.</returns>
        public Builder SetTransitionFixedDuration(
            float duration, AnimatorStateTransition transition
        )
        {
            transition.hasFixedDuration = true;
            transition.duration = duration;
            return this;
        }

        /// <summary>
        /// Sets the duration of a transition as a percentage of the overall
        /// transition length.
        /// </summary>
        /// <param name="duration">
        /// The duration of the transition as a percentage of the overall transition length.
        /// </param>
        /// <param name="transition">The animator state transition object.</param>
        /// <returns>The builder instance.</returns>
        public Builder SetTransitionPercentageDuration(
            float duration, AnimatorStateTransition transition
        )
        {
            transition.hasFixedDuration = false;
            transition.duration = duration;
            return this;
        }

        /// <summary>
        /// Sets the duration of a transition to zero, effectively making it
        /// instant.
        /// </summary>
        /// <param name="transition">The animator state transition object.</param>
        /// <returns>The builder instance.</returns>
        public Builder SetTransitionNoDuration(
            AnimatorStateTransition transition
        )
        {
            transition.hasFixedDuration = true;
            transition.duration = 0.0f;
            return this;
        }

        /// <summary>
        /// Sets the offset of a transition, which determines the time at which
        /// the destination state will start.
        /// </summary>
        /// <param name="offset">
        /// The time at which the destination state will start.
        /// </param>
        /// <param name="transition">The animator state transition object.</param>
        /// <returns>The builder instance.</returns>
        public Builder SetTransitionOffset(
            float offset, AnimatorStateTransition transition
        )
        {
            transition.offset = offset;
            return this;
        }

        /// <summary>
        /// Sets the exit time of a transition, which determines the normalized
        /// time at which unity should test the transition conditions.
        /// </summary>
        /// <param name="exitTime">
        /// The normalized time (percentage of the overall transition length) at
        /// which unity should test the transition conditions.
        /// </param>
        /// <param name="transition">The animator state transition object.</param>
        /// <returns>The builder instance.</returns>
        public Builder SetTransitionExitTime(
            float exitTime, AnimatorStateTransition transition
        )
        {
            transition.exitTime = exitTime;
            transition.hasExitTime = true;
            return this;
        }

        /// <summary>
        /// Disables the exit time of a transition, causing it to be checked immediately.
        /// </summary>
        /// <param name="transition">The animator state transition object.</param>
        /// <returns>The builder instance.</returns>
        public Builder SetTransitionNoExitTime(
            AnimatorStateTransition transition)
        {
            transition.hasExitTime = false;
            transition.exitTime = 0.0f;
            return this;
        }

        /// <summary>Sets the interruption source of a transition.</summary>
        /// <param name="source">The interruption source of the transition.</param>
        /// <param name="transition">The animator state transition object.</param>
        /// <returns>The builder instance.</returns>
        public Builder SetTransitionInterruptionSource(
            TransitionInterruptionSource source,
            AnimatorStateTransition transition
        )
        {
            transition.interruptionSource = source;
            return this;
        }
    }
}