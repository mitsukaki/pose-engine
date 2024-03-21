
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

using com.mitsukaki.poseengine.editor.anim;

using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace com.mitsukaki.poseengine.editor
{
    public struct PoseBuildContext
    {
        public GameObject avatarRoot;
        public GameObject poseEngineInstance;

        public Builder poseController;

        public VRCExpressionsMenu poseMenu;

        public PoseEngineFactory factory;

        public PoseBuildContext(
            GameObject avatarRoot,
            GameObject poseEngineInstance,
            Builder poseController,
            VRCExpressionsMenu poseMenu,
            PoseEngineFactory factory
        )
        {
            this.avatarRoot = avatarRoot;
            this.poseEngineInstance = poseEngineInstance;
            this.poseController = poseController;
            this.poseMenu = poseMenu;
            this.factory = factory;
        }
    }
}