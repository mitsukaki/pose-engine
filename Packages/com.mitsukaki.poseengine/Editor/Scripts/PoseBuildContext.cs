
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

        public PoseEngineFactory factory;

        public PoseBuildContext(
            GameObject avatarRoot,
            GameObject poseEngineInstance,
            Builder poseController,
            PoseEngineFactory factory
        )
        {
            this.avatarRoot = avatarRoot;
            this.poseEngineInstance = poseEngineInstance;
            this.poseController = poseController;
            this.factory = factory;
        }
    }
}