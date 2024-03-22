using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using VRC.SDKBase;

namespace com.mitsukaki.poseengine
{
    [System.Serializable]
    public class SimplePose : Pose
    {
        public AnimationClip clip;
    }
}