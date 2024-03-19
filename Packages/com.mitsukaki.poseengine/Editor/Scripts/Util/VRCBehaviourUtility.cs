
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDK3.Avatars.Components;
using static VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;
using static VRC.SDK3.Avatars.Components.VRCAnimatorLayerControl;

namespace com.mitsukaki.poseengine.editor
{
    public static class VRCBehaviourUtility
    {
        public static void SetParamFlag(AnimatorState state, string paramName)
        {
            var poseDriverBehaviour = state.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
            poseDriverBehaviour.parameters = new List<VRCAvatarParameterDriver.Parameter>();
            poseDriverBehaviour.parameters.Add(new VRCAvatarParameterDriver.Parameter()
            {
                type = VRCAvatarParameterDriver.ChangeType.Set,
                name = paramName,
                value = 1
            });
        }
    }
}