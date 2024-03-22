using System;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace com.mitsukaki.poseengine
{
    public class PEBlendedPose : AGeneratorMenu
    {
        public List<BlendedPose> poses;

        public override string GetIdentifier()
        {
            return "BlendedPose";
        }

        public override List<Pose> GetPoseList()
        {
            // create a new list of poses
            List<Pose> poseList = new List<Pose>();

            // iterate through the poses
            foreach (BlendedPose pose in poses)
                poseList.Add(pose);

            return poseList;
        }

        public override void CopyTo(GameObject target)
        {
            // add a PEBlendedPose component to the target
            var targetComponent = target.AddComponent<PEBlendedPose>();

            // copy the poses
            targetComponent.poses = new List<BlendedPose>(poses);
        }
    }
}