using VRC.SDKBase;
using UnityEngine;

namespace com.mitsukaki.poseengine
{
    public class PoseEngineFactory : MonoBehaviour, IEditorOnly
    {
        public string version = "0.0.2a";

        public string rootMenuName = "Pose Engine";

        public GameObject avatar;
    }
}
