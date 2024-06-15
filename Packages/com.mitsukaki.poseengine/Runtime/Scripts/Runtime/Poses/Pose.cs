using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using VRC.SDKBase;

namespace com.mitsukaki.poseengine
{
    public enum PoseMenuControlType { None, ToggleEnable, Radial };

    public class Pose
    {
        public int PoseID;

        public string Name;

        public Texture2D Icon;

        public bool lockFeetOnEntry = true;

        public virtual PoseMenuControlType MenuControlType => PoseMenuControlType.None;

        public string DrivingParameterName;
    }
}