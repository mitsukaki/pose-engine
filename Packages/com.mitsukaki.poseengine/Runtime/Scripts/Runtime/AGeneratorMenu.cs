using UnityEngine;
using System;
using System.Collections;
using VRC.SDKBase;

namespace com.mitsukaki.poseengine
{
    public abstract class AGeneratorMenu : MonoBehaviour, IEditorOnly
    {
        public abstract string GetIdentifier();

    }
}