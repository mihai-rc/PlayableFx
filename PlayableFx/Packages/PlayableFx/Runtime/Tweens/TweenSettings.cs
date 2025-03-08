using System;
using UnityEngine;
using LitMotion;

namespace PlayableFx
{
    [Serializable]
    public struct TweenSettings
    {
        /// <summary>
        /// Whether this configuration takes effect or not.
        /// </summary>
        public bool Enabled;
        
        /// <summary>
        /// Sets whether the tween should start from the current value or from the overrides.
        /// </summary>
        public bool FromCurrentValue;
        
        /// <summary>
        /// Starting value to tween from.
        /// </summary>
        public Vector3 From;
        
        /// <summary>
        /// Destination value of tween to.
        /// </summary>
        public Vector3 To;
        
        /// <summary>
        /// Ease function to apply to the tween.
        /// </summary>
        public Ease Ease;
        
        /// <summary>
        /// Animation curve to apply to the tween.
        /// Takes effect if <see cref="Ease"/> is set to <see cref="Ease.CustomAnimationCurve"/>.
        /// </summary>
        public AnimationCurve Curve;
    }
}