using UnityEngine;
using LitMotion;

namespace PlayableFx
{
    /// <summary>
    /// Encapsulates the tweening process of a transform for transforming a game object
    /// </summary>
    public class TransformTween
    {
        private enum Transformation
        {
            Position,
            Rotation,
            Scale
        }

        private const string k_AutOfBoundTimeError = "[TransformTween] Cannot compute the tween at a time bigger than tween's duration.";
        
        private float m_Duration;
        private TweenSettings m_Settings;
        
        private Vector3 m_Position;
        private Vector3 m_Rotation;
        private Vector3 m_Scale;
        
        private Vector3 m_FromPosition;
        private Vector3 m_FromRotation;
        private Vector3 m_FromScale;
        
        private Vector3 FromPosition
        {
            set
            {
                m_FromPosition = m_Settings.PositionConfig.UseOverride 
                    ? m_Settings.PositionConfig.FromOverride 
                    : value;
                
                m_Position = m_FromPosition;
            }
        }
        
        private Vector3 FromRotation
        {
            set
            {
                m_FromRotation = m_Settings.RotationConfig.UseOverride 
                    ? m_Settings.RotationConfig.FromOverride 
                    : value;
                
                m_Rotation = m_FromRotation;
            }
        }
        
        private Vector3 FromScale
        {
            set
            {
                m_FromScale = m_Settings.ScaleConfig.UseOverride 
                    ? m_Settings.ScaleConfig.FromOverride 
                    : value;
                
                m_Scale = m_FromScale;
            }
        }
        
        /// <summary>
        /// Sets the duration of the tween.
        /// </summary>
        /// <param name="duration"> The duration of the tween. </param>
        /// <returns> Returns the reference to the tween. </returns>
        public TransformTween SetDuration(float duration)
        {
            m_Duration = duration;
            return this;
        }
        
        /// <summary>
        /// Sets the starting values of the tween.
        /// </summary>
        /// <param name="position"> The position to start from. </param>
        /// <param name="rotation"> The rotation to start from. </param>
        /// <param name="scale"> The scale to start from. </param>
        /// <returns> Returns the reference to the tween. </returns>
        public TransformTween SetStartingValues(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            FromPosition = position;
            FromRotation = rotation;
            FromScale = scale;

            return this;
        }
        
        /// <summary>
        /// Sets the settings of the tween.
        /// </summary>
        /// <param name="settings"> The settings of the tween. </param>
        /// <returns> Returns the reference to the tween. </returns>
        public TransformTween SetSettings(TweenSettings settings)
        {
            m_Settings = settings;
            return this;
        }
        
        /// <summary>
        /// Computes the tween at a specific time.
        /// </summary>
        /// <param name="time"> The time to compute the tween values at. </param>
        /// <returns> Returns the reference to the tween. </returns>
        public TransformTween ComputeAt(float time)
        {
            if (time > m_Duration)
            {
                Debug.LogError(k_AutOfBoundTimeError);
                return this;
            }
            
            var sequenceBuilder = LSequence.Create();

            var positionConfig = m_Settings.PositionConfig;
            if (positionConfig.Enabled)
                CreateMotion(sequenceBuilder, positionConfig, Transformation.Position);

            var rotationConfig = m_Settings.RotationConfig;
            if (rotationConfig.Enabled)
                CreateMotion(sequenceBuilder, rotationConfig, Transformation.Rotation);

            var scaleConfig = m_Settings.ScaleConfig;
            if (scaleConfig.Enabled)
                CreateMotion(sequenceBuilder, scaleConfig, Transformation.Scale);

            var sequence = sequenceBuilder.Run();
            sequence.Preserve();
            sequence.PlaybackSpeed = 0f;
            sequence.Time = time;
            sequence.TryCancel();

            return this;
        }

        /// <summary>
        /// Interpolates this tween values with the provided tween values.
        /// </summary>
        /// <param name="tween"> The tween to interpolate with. </param>
        /// <param name="step"> The step of the interpolation. </param>
        /// <returns> Returns the reference to the tween. </returns>
        public TransformTween BlendWith(TransformTween tween, float step)
        {
            m_Position = (m_Settings.PositionConfig.Enabled, tween.m_Settings.PositionConfig.Enabled) switch
            {
                (true , true ) => Vector3.Lerp(m_Position, tween.m_Position, step),
                (true , false) => m_Position,
                (false, true ) => tween.m_Position,
                _ => m_Position
            };
                    
            m_Rotation = (m_Settings.RotationConfig.Enabled, tween.m_Settings.RotationConfig.Enabled) switch
            {
                (true , true ) => Vector3.Lerp(m_Rotation, tween.m_Rotation, step),
                (true , false) => m_Rotation,
                (false, true ) => tween.m_Rotation,
                _ => m_Rotation
            };
                    
            m_Scale = (m_Settings.ScaleConfig.Enabled, tween.m_Settings.ScaleConfig.Enabled) switch
            {
                (true , true ) => Vector3.Lerp(m_Scale, tween.m_Scale, step),
                (true , false) => m_Scale,
                (false, true ) => tween.m_Scale,
                _ => m_Scale
            };
            
            return this;
        }
        
        /// <summary>
        /// Applies the tween values to the <see cref="Transform"/>
        /// </summary>
        /// <param name="transform"> Transform to apply the tween values to.</param>
        public void ApplyTo(Transform transform)
        {
            if (m_Settings.PositionConfig.Enabled)
                transform.localPosition = m_Position;
                    
            if (m_Settings.RotationConfig.Enabled)
                transform.localEulerAngles = m_Rotation;
                    
            if (m_Settings.ScaleConfig.Enabled)
                transform.localScale = m_Scale;
        }
        
        /// <summary>
        /// Sets the tween to its final values.
        /// </summary>
        public void Complete() => ComputeAt(m_Duration);
        
        /// <summary>
        /// Resets the tween to its original values.
        /// </summary>
        public void Revert() => ComputeAt(0f);

        private void CreateMotion(MotionSequenceBuilder sequenceBuilder, TweenConfig config, Transformation transformation)
        {
            var from = transformation switch
            {
                Transformation.Position => m_FromPosition,
                Transformation.Rotation => m_FromRotation,
                Transformation.Scale => m_FromScale,
                _ => default
            };
            
            var motionBuilder = LMotion.Create(from, config.To, m_Duration);
            if (config.Ease is Ease.CustomAnimationCurve)
            {
                motionBuilder.WithEase(config.Curve);
            }
            else
            {
                motionBuilder.WithEase(config.Ease);
            }

            var motion = transformation switch
            {
                Transformation.Position => motionBuilder.Bind(this, (v, t) => t.m_Position = v),
                Transformation.Rotation => motionBuilder.Bind(this, (v, t) => t.m_Rotation = v),
                Transformation.Scale => motionBuilder.Bind(this, (v, t) => t.m_Scale = v),
                _ => default
            };

            sequenceBuilder.Join(motion);
        }
    }
}