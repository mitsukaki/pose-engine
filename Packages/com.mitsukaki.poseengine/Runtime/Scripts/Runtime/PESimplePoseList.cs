using UnityEngine;
using VRC.SDKBase;
using System;
using System.Collections;
using System.Collections.Generic;
using VRC.Dynamics;

namespace com.mitsukaki.poseengine
{
    [Serializable]
    public struct Pose
    {
        public string name;

        public Texture2D icon;

        public AnimationClip clip;
    }

    public class PESimplePoseList : AGeneratorMenu
    {
        public Pose[] poses;

        public override string GetIdentifier()
        {
            return "SimplePoseList";
        }
    }
}