using System;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace com.mitsukaki.poseengine
{
    public enum BlendPoseType { SingleClip, MultiClip };

    [Serializable]
    public class BlendedPose : Pose
    {
        public List<AnimationClip> clips;

        public BlendPoseType Type;
    }
}