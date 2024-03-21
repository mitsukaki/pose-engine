
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace com.mitsukaki.poseengine.editor.anim
{
    public partial class Builder
    {
        /// <summary>
        /// Gets the layer with the specified name.
        /// </summary>
        /// <param name="name">The name of the layer to retrieve.</param>
        /// <returns>The layer with the specified name.</returns>
        public AnimatorControllerLayer GetLayer(string name)
        {
            foreach (var layer in controller.layers)
            {
                if (layer.name == name)
                {
                    return layer;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the layer at the specified index.
        /// </summary>
        /// <param name="index">The index of the layer to retrieve.</param>
        /// <returns>The layer at the specified index.</returns>
        public AnimatorControllerLayer GetLayer(int index)
        {
            return controller.layers[index];
        }

        /// <summary>
        /// Creates a new AnimatorController layer.
        /// </summary>
        /// <param name="name">The name to assign to the added layer.</param>
        /// <returns>The newly created layer.</returns>
        public AnimatorControllerLayer AddLayer(string name)
        {
            var layer = new AnimatorControllerLayer();

            layer.name = name;
            layer.defaultWeight = 1.0f;
            layer.stateMachine = new AnimatorStateMachine();
            controller.AddLayer(layer);
            layer.stateMachine.exitPosition = new Vector3(20, -150, 0);
            layer.stateMachine.anyStatePosition = new Vector3(20, -100, 0);
            layer.stateMachine.entryPosition = new Vector3(20, -50, 0);

            return layer;
        }

        public AnimatorControllerLayer AddLayer(string name, float weight)
        {
            var layer = new AnimatorControllerLayer();

            layer.name = name;
            layer.defaultWeight = weight;
            layer.stateMachine = new AnimatorStateMachine();
            controller.AddLayer(layer);
            layer.stateMachine.exitPosition = new Vector3(20, -150, 0);
            layer.stateMachine.anyStatePosition = new Vector3(20, -100, 0);
            layer.stateMachine.entryPosition = new Vector3(20, -50, 0);

            return layer;
        }

        /// <summary>
        /// Adds a new AnimatorController layer with the specified name and assigns it to the provided layer variable.
        /// </summary>
        /// <param name="name">The name to assign to the added layer.</param>
        /// <param name="layer">The variable to assign the newly created layer to.</param>
        /// <returns>The current instance of the Builder.</returns>
        public Builder AddLayer(string name, out AnimatorControllerLayer layer)
        {
            layer = AddLayer(name);
            return this;
        }

        /// <summary>
        /// Adds a new AnimatorController layer with the specified name and weight and assigns it to the provided layer variable.
        /// </summary>
        /// <param name="name">The name to assign to the added layer.</param>
        /// <param name="weight">The weight to assign to the added layer.</param>
        /// <param name="layer">The variable to assign the newly created layer to.</param>
        /// <returns>The current instance of the Builder.</returns>
        public Builder AddLayer(string name, float weight, out AnimatorControllerLayer layer)
        {
            layer = AddLayer(name, weight);
            return this;
        }

        /// <summary>
        /// Sets the AvatarMask for the specified AnimatorController layer.
        /// </summary>
        /// <param name="mask">The AvatarMask to assign to the layer.</param>
        /// <param name="layer">The AnimatorController layer to set the AvatarMask for.</param>
        /// <returns>The current instance of the Builder.</returns>
        public Builder SetLayerAvatarMask(AvatarMask mask, AnimatorControllerLayer layer)
        {
            layer.avatarMask = mask;
            return this;
        }
    }
}