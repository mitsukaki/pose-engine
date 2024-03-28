using System;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace com.mitsukaki.poseengine
{
    public class PEIdlePoseOverride : AGeneratorMenu
    {
        public Pose standingIdle;
        public Pose crouchingIdle;
        public Pose layingIdle;

        public override string GetIdentifier()
        {
            return "IdlePoseOverride";
        }

        public override List<Pose> GetPoseList()
        {
            // create a new list of poses
            List<Pose> poseList = new List<Pose>();

            // add the poses
            poseList.Add(standingIdle);
            poseList.Add(crouchingIdle);
            poseList.Add(layingIdle);

            return poseList;
        }

        public override void CopyTo(GameObject target)
        {
            // add a PEIdlePoseOverride component to the target
            var targetComponent = target.AddComponent<PEIdlePoseOverride>();

            // copy the poses
            targetComponent.standingIdle = standingIdle;
            targetComponent.crouchingIdle = crouchingIdle;
            targetComponent.layingIdle = layingIdle;
        }
    }
}