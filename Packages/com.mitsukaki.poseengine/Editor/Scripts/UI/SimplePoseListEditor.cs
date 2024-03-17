using UnityEditor;
using UnityEngine;

namespace com.mitsukaki.poseengine.Editor.ui
{
    [CustomEditor(typeof(PE_SimplePoseList))]
    public class SimplePoseListEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            // make a collapsible debug information section
            var simplePoseList = (target as PE_SimplePoseList);
            simplePoseList.showDebug = EditorGUILayout.Foldout(
                simplePoseList.showDebug,
                "Debug Information"
            );

            if (simplePoseList.showDebug)
            {
                EditorGUI.indentLevel++;
                // Write every poses name, and whether it has root motion
                for (int i = 0; i < simplePoseList.poses.Length; i++)
                {
                    simplePoseList.poses[i].showDebug = EditorGUILayout.Foldout(
                        simplePoseList.poses[i].showDebug,
                        simplePoseList.poses[i].name
                    );

                    if (!simplePoseList.poses[i].showDebug) continue;

                    var pose = simplePoseList.poses[i];

                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("Clip Count: " + pose.clips.Length);
                    EditorGUILayout.LabelField("Pose Type: " + (pose.clips.Length > 1 ? "Blended Pose" : "Mirrorable Single"));
                    var indent = (pose.clips.Length > 1);
                    foreach (var clip in pose.clips)
                    {
                        if (indent) EditorGUI.indentLevel++;
                        
                        if (clip == null)
                        {
                            EditorGUILayout.LabelField("Clip: NULL");
                            continue;
                        }

                        EditorGUILayout.LabelField("Clip: " + clip.name);
                        EditorGUILayout.LabelField("Is Humanoid?: " + clip.isHumanMotion);
                        EditorGUILayout.LabelField("Has Motion Curves?: " + clip.hasMotionCurves);
                        EditorGUILayout.LabelField("Has Root Motion?: " + clip.hasRootCurves);

                        if (indent) EditorGUI.indentLevel--;
                    }
                }

                EditorGUI.indentLevel--;
            }
        }
    }
}
