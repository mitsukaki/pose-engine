#region 

using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

#endregion

namespace com.mitsukaki.poseengine.Editor
{
    public struct PoseBuildContext
    {
        public GameObject avatarRoot;

        public AnimatorController poseController;

        public VRCExpressionsMenu poseMenu;

        public PoseBuildContext(
            GameObject avatarRoot,
            AnimatorController poseController,
            VRCExpressionsMenu poseMenu
        )
        {
            this.avatarRoot = avatarRoot;
            this.poseController = poseController;
            this.poseMenu = poseMenu;
        }
    }
}