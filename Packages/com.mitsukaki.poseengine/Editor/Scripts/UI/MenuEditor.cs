using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace com.mitsukaki.poseengine.editor.ui
{
    [CustomEditor(typeof(PEMenu))]
    public class MenuEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Get the target object
            PEMenu menu = (PEMenu)target;

            // Get it's game object
            GameObject go = menu.gameObject;

            // Draw a label for "Menu"
            EditorGUILayout.LabelField("Menu");

            DrawMenuItems(go);
        }

        private void DrawMenuItems(GameObject go)
        {
            List<AGeneratorMenu> children = new List<AGeneratorMenu>();

            // Get the children of the game object
            foreach (Transform child in go.transform)
            {
                AGeneratorMenu menu = child.GetComponent<AGeneratorMenu>();

                if (menu != null) children.Add(menu);
            }

            // if there are no children, return
            if (children.Count == 0) return;

            // Indent the editor
            EditorGUI.indentLevel++;

            // Loop through the children
            foreach (AGeneratorMenu child in children)
            {
                // Draw a label for the child
                EditorGUILayout.LabelField(child.gameObject.name);
            }

            // Unindent the editor
            EditorGUI.indentLevel--;
        }
    }
}
