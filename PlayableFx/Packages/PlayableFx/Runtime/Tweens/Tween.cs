using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

namespace PlayableFx
{
    public class Tween
    {
        private enum Transformation
        {
            Position,
            Rotation,
            Scale
        }
        
        /// <summary>
        /// The duration of the <see cref="Tween"/>.
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// The <see cref="UnityEngine.Transform"/> to animate. 
        /// </summary>
        public Transform Transform
        {
            set
            {
                m_Transform = value;
                m_OriginalPosition = value.localPosition;
                m_OriginalRotation = value.localEulerAngles;
                m_OriginalScale = value.localScale;
            }
        }

        /// <summary>
        /// Sets the time of the <see cref="Tween"/>.
        /// </summary>
        public float Time
        {
            set => SetTime(value);
        }
        
        private const string k_NullTransformError = "[PlayableTween] Can't play tween because no Transform was provided.";
        
        private readonly TweenSettings m_PositionSettings;
        private readonly TweenSettings m_RotationSettings;
        private readonly TweenSettings m_ScaleSettings;
        
        private Vector3 m_OriginalPosition;
        private Vector3 m_OriginalRotation;
        private Vector3 m_OriginalScale;
        
        private Vector3 m_FromPosition;
        private Vector3 m_FromRotation;
        private Vector3 m_FromScale;
        
        private Transform m_Transform;

        /// <summary>
        /// <see cref="Tween"/>'s constructor.
        /// </summary>
        /// <param name="positionSettings"> Position tween settings. </param>
        /// <param name="rotationSettings"> Rotation tween settings. </param>
        /// <param name="scaleSettings"> Scale tween settings. </param>
        public Tween(TweenSettings positionSettings, TweenSettings rotationSettings, TweenSettings scaleSettings)
        {
            m_PositionSettings = positionSettings;
            m_RotationSettings = rotationSettings;
            m_ScaleSettings = scaleSettings;
            
            m_OriginalPosition = default;
            m_OriginalRotation = default;
            m_OriginalScale = default;
            
            m_FromPosition = default;
            m_FromRotation = default;
            m_FromScale = default;
        }
        
        /// <summary>
        /// Sets the starting values of the <see cref="Tween"/> internally.
        /// </summary>
        public void PrepareStartValues()
        {
            if (!TryGetTransform(out var transform))
                return;
            
            m_FromPosition = m_PositionSettings.FromCurrentValue 
                ? transform.localPosition
                : m_PositionSettings.From;
            
            m_FromRotation = m_RotationSettings.FromCurrentValue
                ? transform.localEulerAngles
                : m_RotationSettings.From;
            
            m_FromScale = m_ScaleSettings.FromCurrentValue
                ? transform.localScale
                : m_ScaleSettings.From;
        }
        
        /// <summary>
        /// Resets the <see cref="Transform"/> to its original values.
        /// </summary>
        public void ResetToOriginalValues()
        {
            if (!TryGetTransform(out var transform))
                return;
            
            if (m_PositionSettings.Enabled)
                transform.position = m_OriginalPosition;
            
            if (m_RotationSettings.Enabled)
                transform.eulerAngles = m_OriginalRotation;
            
            if (m_ScaleSettings.Enabled)
                transform.localScale = m_OriginalScale;
        }
        
        /// <summary>
        /// Tries to get the <see cref="Transform"/> bount to this <see cref="Tween"/>.
        /// </summary>
        /// <param name="transform"> Reference to the <see cref="Transform"/> bound to this <see cref="Tween"/>. </param>
        /// <returns> Returns whether the <see cref="Transform"/> is null or not. </returns>
        private bool TryGetTransform(out Transform transform)
        {
            if (m_Transform is null)
            {
                Debug.LogError(k_NullTransformError);
                
                transform = null;
                return false;
            }
            
            transform = m_Transform;
            return true;
        }
        
        private void SetTime(float time)
        {
            if (!TryGetTransform(out var transform))
                return;

            var sequenceBuilder = LSequence.Create();

            if (m_PositionSettings.Enabled)
                CreateMotion(sequenceBuilder, m_PositionSettings, transform, Transformation.Position);

            if (m_RotationSettings.Enabled)
                CreateMotion(sequenceBuilder, m_RotationSettings, transform, Transformation.Rotation);

            if (m_ScaleSettings.Enabled)
                CreateMotion(sequenceBuilder, m_ScaleSettings, transform, Transformation.Scale);

            var sequence = sequenceBuilder.Run();
            sequence.Preserve();
            sequence.PlaybackSpeed = 0f;
            sequence.Time = time;
            sequence.TryCancel();
        }

        private void CreateMotion(MotionSequenceBuilder sequenceBuilder, 
            TweenSettings settings,
            Transform transform, 
            Transformation transformation)
        {
            var from = transformation switch
            {
                Transformation.Position => m_FromPosition,
                Transformation.Rotation => m_FromRotation,
                Transformation.Scale => m_FromScale,
                _ => default
            };
            
            var motionBuilder = LMotion.Create(from, settings.To, Duration);
            if (settings.Ease is Ease.CustomAnimationCurve)
            {
                motionBuilder.WithEase(settings.Curve);
            }
            else
            {
                motionBuilder.WithEase(settings.Ease);
            }

            var motion = transformation switch
            {
                Transformation.Position => motionBuilder.BindToLocalPosition(transform),
                Transformation.Rotation => motionBuilder.BindToLocalEulerAngles(transform),
                Transformation.Scale => motionBuilder.BindToLocalScale(transform),
                _ => default
            };

            sequenceBuilder.Join(motion);
        }
    }
}