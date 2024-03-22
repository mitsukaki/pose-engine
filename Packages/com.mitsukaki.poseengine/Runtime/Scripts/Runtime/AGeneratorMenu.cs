using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using VRC.SDKBase;

namespace com.mitsukaki.poseengine
{
    public abstract class AGeneratorMenu : MonoBehaviour, IEditorOnly
    {
        public abstract string GetIdentifier();

        public abstract List<Pose> GetPoseList();

        public abstract void CopyTo(GameObject target);
    }
}