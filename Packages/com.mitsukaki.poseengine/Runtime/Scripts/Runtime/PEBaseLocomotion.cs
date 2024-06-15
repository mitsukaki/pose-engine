using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using VRC.SDKBase;

namespace com.mitsukaki.poseengine
{
    public class PEBaseLocomotion : MonoBehaviour, IEditorOnly
    {
        public RuntimeAnimatorController BaseLayerAnimator;

        public AnimationClip crouchClip;
        public AnimationClip proneClip;
        public AnimationClip afkClip;

        public bool keepCrouchCrawling = false;
        public bool keepProneCrawling = false;
    }
}