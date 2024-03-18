
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace com.mitsukaki.poseengine.editor.ui
{
    [CustomEditor(typeof(PoseEngineFactory))]
    public class FactoryEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Generate Prebuilt"))
            {
                // get the avatar
                var avatar = (target as PoseEngineFactory).avatar;

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

                assembler.Assemble(avatar);
            }
        }
    }
}
