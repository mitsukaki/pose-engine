using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using VRC.SDKBase;

namespace com.mitsukaki.poseengine
{
    public class PESimplePoseList : AGeneratorMenu
    {
        public List<SimplePose> poses;

        public override string GetIdentifier()
        {
            return "SimplePoseList";
        }

        public override List<Pose> GetPoseList()
        {
            // create a new list of poses
            List<Pose> poseList = new List<Pose>();

            // iterate through the poses
            foreach (SimplePose pose in poses)
                poseList.Add(pose);

            return poseList;
        }

        public override void CopyTo(GameObject target)
        {
            var targetMenu = target.AddComponent<PESimplePoseList>();

            targetMenu.poses = new List<SimplePose>(poses);
        }
    }
}