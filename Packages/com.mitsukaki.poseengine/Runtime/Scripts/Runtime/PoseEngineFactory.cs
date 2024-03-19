using VRC.SDKBase;
using UnityEngine;

namespace com.mitsukaki.poseengine
{
    public class PoseEngineFactory : MonoBehaviour, IEditorOnly
    {
        public string version = "0.2.0";

        public string rootMenuName = "Pose Engine";

        public bool deleteNameIfIconSet = false;

        public GameObject avatar;
    }
}
