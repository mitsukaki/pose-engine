using UnityEngine;
using UnityEditor.Animations;

using System;
using System.Collections;
using System.Collections.Generic;

using VRC.SDKBase;

namespace com.mitsukaki.poseengine
{
    public class PEBaseLocomotion : MonoBehaviour, IEditorOnly
    {
        public AnimatorController BaseLayerAnimator;
    }
}