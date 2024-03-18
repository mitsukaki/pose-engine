
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace com.mitsukaki.poseengine.editor
{
    public struct PoseBuildContext
    {
        public GameObject avatarRoot;

        public AnimatorController poseController;

        public VRCExpressionsMenu poseMenu;

        public PoseEngineFactory factory;

        public PoseBuildContext(
            GameObject avatarRoot,
            AnimatorController poseController,
            VRCExpressionsMenu poseMenu,
            PoseEngineFactory factory
        )
        {
            this.avatarRoot = avatarRoot;
            this.poseController = poseController;
            this.poseMenu = poseMenu;
            this.factory = factory;
        }
    }
}