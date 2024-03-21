using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace com.mitsukaki.poseengine.editor.ui
{
    [CustomEditor(typeof(PESimplePoseList))]
    public class SimplePoseListEditor : UnityEditor.Editor
    {
        private ReorderableList poseList;

        private const float SPACING = 6f;

        private void OnEnable()
        {
            poseList = new ReorderableList(
                serializedObject,
                serializedObject.FindProperty("poses"),
                true, true, true, true
            );
            
            poseList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Simple Pose List");
            };

            var lineHeight = EditorGUIUtility.singleLineHeight;
            var iconWidth = lineHeight * 5;
            poseList.elementHeight = iconWidth + SPACING * 2;

            poseList.drawElementCallback = (
                Rect rect, int index, bool isActive, bool isFocused
            ) => {
                var element = poseList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;

                var lineStart = rect.y;

                // column #1
                // the name field
                EditorGUI.LabelField(new Rect(
                    rect.x, rect.y, rect.width * 0.25f, lineHeight
                ), "Name");
                EditorGUI.PropertyField(new Rect(
                    rect.x + rect.width * 0.25f, rect.y, rect.width * 0.4f, lineHeight
                ), element.FindPropertyRelative("name"), GUIContent.none);
                rect.y += lineHeight + SPACING; // Add spacing between elements

                // the clip field
                EditorGUI.LabelField(new Rect(
                    rect.x, rect.y, rect.width * 0.25f, lineHeight
                ), "Clip");
                EditorGUI.PropertyField(new Rect(
                    rect.x + rect.width * 0.25f, rect.y, rect.width * 0.4f, lineHeight
                ), element.FindPropertyRelative("clip"), GUIContent.none);
                rect.y += lineHeight + SPACING; // Add spacing between elements

                // the icon field
                EditorGUI.LabelField(new Rect(
                    rect.x, rect.y, rect.width * 0.25f, lineHeight
                ), "Icon");
                EditorGUI.PropertyField(new Rect(
                    rect.x + rect.width * 0.25f, rect.y, rect.width * 0.4f, lineHeight
                ), element.FindPropertyRelative("icon"), GUIContent.none);

                // Column #2
                rect.y = lineStart;
                var iconProperty = element.FindPropertyRelative("icon");
                var icon = iconProperty.objectReferenceValue as Texture2D;
                if (icon != null)
                {
                    EditorGUI.DrawPreviewTexture(new Rect(
                        rect.width - iconWidth, rect.y, iconWidth, iconWidth
                    ), icon);
                }
                else
                {
                    EditorGUI.LabelField(new Rect(
                        rect.width - iconWidth, rect.y, iconWidth, iconWidth
                    ), "No Icon");
                }

                rect.y += iconWidth + 2 + SPACING;

                // Draw a divider
                EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 1), Color.grey);
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            poseList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}