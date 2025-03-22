using System;
using LitMotion;
using UnityEngine;

namespace PlayableFx
{
    /// <summary>
    /// The configuration of a tween value.
    /// </summary>
    [Serializable]
    public struct TweenConfig
    {
        /// <summary>
        /// Whether this configuration takes effect or not.
        /// </summary>
        [field: SerializeField, Tooltip("Whether this configuration takes effect or not.")]
        public bool Enabled { get; private set; }
        
        /// <summary>
        /// Sets whether the tween should start from the current value or from the overrides.
        /// </summary>
        [field: SerializeField, Tooltip("Sets whether the tween should start from the current value or from the overrides.")]
        public bool UseOverride { get; private set; }
        
        /// <summary>
        /// Starting value to tween from.
        /// </summary>
        [field: SerializeField, Tooltip("Starting value to tween from.")]
        public Vector3 FromOverride { get; private set; }
        
        /// <summary>
        /// Destination value of tween to.
        /// </summary>
        [field: SerializeField, Tooltip("Destination value of tween to.")]
        public Vector3 To { get; private set; }
        
        /// <summary>
        /// Ease function to apply to the tween.
        /// </summary>
        [field: SerializeField, Tooltip("Ease function to apply to the tween.")]
        public Ease Ease { get; private set; }
        
        /// <summary>
        /// Animation curve to apply to the tween.
        /// Takes effect if <see cref="Ease"/> is set to <see cref="Ease.CustomAnimationCurve"/>.
        /// </summary>
        [field: SerializeField, Tooltip("Animation curve to apply to the tween.")]
        public AnimationCurve Curve { get; private set; }
    }
}