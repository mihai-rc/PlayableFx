using System;
using UnityEngine;

namespace PlayableFx
{
    /// <summary>
    /// The settings of a tween.
    /// </summary>
    [Serializable]
    public struct TweenSettings
    {
        /// <summary>
        /// The config of the position tween.
        /// </summary>
        [field: SerializeField, Tooltip("The config of the position tween.")]
        public TweenConfig PositionConfig { get; /*private*/ set; }
        
        /// <summary>
        /// The config of the rotation tween.
        /// </summary>
        [field: SerializeField, Tooltip("The config of the rotation tween.")]
        public TweenConfig RotationConfig { get; /*private*/ set; }
        
        /// <summary>
        /// The config of the scale tween.
        /// </summary>
        [field: SerializeField, Tooltip("The config of the scale tween.")]
        public TweenConfig ScaleConfig { get; /*private*/ set; }
    }
}