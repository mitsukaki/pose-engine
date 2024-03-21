using UnityEngine;
using VRC.SDKBase;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using VRC.Dynamics;

namespace com.mitsukaki.poseengine
{
    [Serializable]
    public struct Pose
    {
        public bool showDebug;
        
        public string name;

        public Texture2D icon;

        public AnimationClip clip;
    }

    public class PESimplePoseList : MonoBehaviour, IEditorOnly
    {
        public bool showDebug;

        public string path;
        
        public Pose[] poses;
    }
}