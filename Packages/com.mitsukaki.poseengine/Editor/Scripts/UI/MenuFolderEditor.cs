using UnityEditor;
using UnityEngine;

namespace com.mitsukaki.poseengine.editor.ui
{
    [CustomEditor(typeof(PEMenuFolder))]
    public class MenuFolderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}
