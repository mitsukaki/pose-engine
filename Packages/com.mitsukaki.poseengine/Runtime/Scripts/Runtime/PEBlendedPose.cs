using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.mitsukaki.poseengine
{
    public enum BlendPoseType { SingleClip, MultiClip };

    [System.Serializable]
    public struct BlendedPose
    {
        public string name;

        public Texture2D icon;

        public BlendPoseType type;

        public List<AnimationClip> clips;
    }

    public class PEBlendedPose : AGeneratorMenu
    {
        public List<BlendedPose> poses;

        public override string GetIdentifier()
        {
            return "BlendedPose";
        }
    }
}