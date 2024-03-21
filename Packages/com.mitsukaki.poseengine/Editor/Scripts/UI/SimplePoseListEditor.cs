using UnityEditor;
using UnityEngine;

namespace com.mitsukaki.poseengine.editor.ui
{
    [CustomEditor(typeof(PESimplePoseList))]
    public class SimplePoseListEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}
