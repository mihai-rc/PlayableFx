using System;
using UnityEngine;
using LitMotion;

namespace PlayableFx
{
    [Serializable]
    public struct TweenSettings
    {
        public bool Enabled;
        public Vector3 From;
        public Vector3 To;
        public Ease Ease;
        public AnimationCurve Curve;
    }
}
