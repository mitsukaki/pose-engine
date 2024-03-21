using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.mitsukaki.poseengine
{
    public class PEMenu : AGeneratorMenu
    {
        public string menuName;

        public override string GetIdentifier()
        {
            return "Menu";
        }
    }
}