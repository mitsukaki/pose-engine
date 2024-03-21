
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace com.mitsukaki.poseengine.editor.ui
{
    [CustomEditor(typeof(PoseEngineFactory))]
    public class FactoryEditor : UnityEditor.Editor
    {
        private bool experimentalFoldout = false;

        public override void OnInspectorGUI()
        {
            DrawWarnings();

            DrawDefaultInspector();

            DrawExperimentals();
        }

        private void DrawWarnings()
        {
            DrawPrebuiltWarning();

            DrawNoComponentsWarning();
        }

        private void DrawExperimentals()
        {
            experimentalFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(
                experimentalFoldout,
                "Experimental"
            );

            if (experimentalFoldout)
            {
                DrawPrebuiltGenerator();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawPrebuiltWarning()
        {
            // if an avatar is set
            if ((target as PoseEngineFactory).avatar != null)
            {
                // if the avatar has a pose engine prefab
                if ((target as PoseEngineFactory).avatar.transform.Find("PoseEnginePrefab") != null)
                {
                    // Warn the user that the pre-built system is currently broken and should not be used
                    EditorGUILayout.HelpBox(
                        "A prebuilt pose engine prefab was detected on this avatar. This feature is currently broken! Please delete the PoseEnginePrefab from your avatar.",
                        MessageType.Warning
                    );

                    // Draw a button to delete the prefab
                    if (GUILayout.Button("Delete Pose Engine Prefab"))
                    {
                        var pePrefab = (target as PoseEngineFactory).avatar.transform.Find("PoseEnginePrefab");
                        if (pePrefab != null)
                        {
                            Debug.Log("[PoseEngine] Deleting existing PoseEnginePrefab");
                            DestroyImmediate(pePrefab.gameObject);
                        }
                    }
                }
            }
        }

        private void DrawNoComponentsWarning()
        {
            // if an avatar is set
            if ((target as PoseEngineFactory).avatar != null)
            {
                int componentCount = 0;

                // count the number of PE_SimplePoseList components
                componentCount += (target as PoseEngineFactory).avatar.GetComponentsInChildren<PESimplePoseList>().Length;

                // if there are no components
                if (componentCount == 0)
                {
                    // Inform the user that no components were detected
                    EditorGUILayout.HelpBox(
                        "No Pose Engine components were detected on this avatar. Add one such as \"PE_SimplePostList\" to being using Pose Engine.",
                        MessageType.Warning
                    );
                }
            }
        }

        private void DrawPrebuiltGenerator()
        {
            // Label warning about the prebuilt function being broken
            EditorGUILayout.HelpBox(
                "The prebuilt function is currently mostly broken! It exists for testing purposes only.",
                MessageType.Warning
            );

            // Button to generate the prebuilt
            if (GUILayout.Button("Generate Prebuilt"))
            {
                var factory = (target as PoseEngineFactory);

                // get the avatar
                var avatar = factory.avatar;

                // delete the existing pose engine if it exists
                var pePrefab = avatar.transform.Find("PoseEnginePrefab");
                if (pePrefab != null)
                {
                    Debug.Log("[PoseEngine] Deleting existing PoseEnginePrefab");
                    DestroyImmediate(pePrefab.gameObject);
                }

                // Run the assembler
                var assembler = new Assembler(
                    new List<generators.IPoseGenerator>{
                            new generators.SimplePoseGenerator()
                    }
                );

                assembler.Assemble(avatar, factory);
            }
        }
    }
}
