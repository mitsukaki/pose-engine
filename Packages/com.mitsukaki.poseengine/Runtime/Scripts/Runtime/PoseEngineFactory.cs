using VRC.SDKBase;
using UnityEngine;

namespace com.mitsukaki.poseengine
{
    [System.Serializable]
    public struct SkinIcon
    {
        public string name;
        public Texture2D icon;
    }

    public class PoseEngineFactory : MonoBehaviour, IEditorOnly
    {
        public string rootMenuName = "Pose Engine";

        public bool deleteNameIfIconSet = false;

        public GameObject avatar;

        public SkinIcon[] skinIcons;

        // function to run when inspected
        public void OnInspect()
        {
            Debug.Log("YOUR MOM");
        }
    }
}
