using UnityEditor;
using UnityEngine;

namespace com.mitsukaki.poseengine.editor.ui
{
    [CustomEditor(typeof(PEBlendedPose))]
    public class BlendedPoseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}
