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

        /// <summary>
        /// Enumerate the poses in the menu, giving each an ID
        /// </summary>
        /// <param name="startIndex">The ID to start at</param>
        /// <returns>The ID to start at for the next menu</returns>
        public abstract int EnumeratePoses(int startIndex);

        public abstract void CopyTo(GameObject target);
    }
}