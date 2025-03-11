using UnityEngine;
using LitMotion;

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

        public Vector3 Position { get; private set; }

        public Vector3 Rotation { get; private set; }
        
        public Vector3 Scale { get; private set; }

        public Vector3 FromPosition
        {
            get => m_FromPosition;
            set
            {
                m_FromPosition = value;
                Position = value;
            }
        }
        
        public Vector3 FromRotation
        {
            get => m_FromRotation;
            set
            {
                m_FromRotation = value;
                Rotation = value;
            }
        }
        
        public Vector3 FromScale
        {
            get => m_FromScale;
            set
            {
                m_FromScale = value;
                Scale = value;
            }
        }
        
        /// <summary>
        /// The duration of the <see cref="Tween"/>.
        /// </summary>
        public float Duration
        {
            set => m_Duration = value;
        }
        
        public TweenSettings Settings
        {
            set
            {
                m_PositionConfig = value.PositionConfig;
                m_RotationConfig = value.RotationConfig;
                m_ScaleConfig = value.ScaleConfig;
            }
        }

        /// <summary>
        /// Sets the time of the <see cref="Tween"/>.
        /// </summary>
        public float Time
        {
            set => SetTime(value);
        }
        
        private const string k_NullTransformError = "[Tween] Can't play tween because no Transform was provided.";
        
        private float m_Duration;
        
        private TweenConfig m_PositionConfig;
        private TweenConfig m_RotationConfig;
        private TweenConfig m_ScaleConfig;
        
        private Vector3 m_FromPosition;
        private Vector3 m_FromRotation;
        private Vector3 m_FromScale;
        

        /// <summary>
        /// <see cref="Tween"/>'s constructor.
        /// </summary>
        public Tween() { }
        
        /// <summary>
        /// Sets the <see cref="Transform"/> to its final values.
        /// </summary>
        public void Complete()
        {
            if (m_PositionConfig.Enabled)
                Position = m_PositionConfig.To;
            
            if (m_RotationConfig.Enabled)
                Rotation = m_RotationConfig.To;
            
            if (m_ScaleConfig.Enabled)
                Scale = m_ScaleConfig.To;
        }
        
        /// <summary>
        /// Resets the <see cref="Transform"/> to its original values.
        /// </summary>
        public void Revert()
        {
            if (m_PositionConfig.Enabled)
                Position = m_FromPosition;
            
            if (m_RotationConfig.Enabled)
                Rotation = m_FromRotation;
            
            if (m_ScaleConfig.Enabled)
                Scale = m_FromScale;
        }
        
        private void SetTime(float time)
        {
            var sequenceBuilder = LSequence.Create();

            if (m_PositionConfig.Enabled)
                CreateMotion(sequenceBuilder, m_PositionConfig, Transformation.Position);

            if (m_RotationConfig.Enabled)
                CreateMotion(sequenceBuilder, m_RotationConfig, Transformation.Rotation);

            if (m_ScaleConfig.Enabled)
                CreateMotion(sequenceBuilder, m_ScaleConfig, Transformation.Scale);

            var sequence = sequenceBuilder.Run();
            sequence.Preserve();
            sequence.PlaybackSpeed = 0f;
            sequence.Time = time;
            sequence.TryCancel();
        }

        private void CreateMotion(MotionSequenceBuilder sequenceBuilder, 
            TweenConfig config,
            Transformation transformation)
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
                Transformation.Position => motionBuilder.Bind(this, (v, t) => t.Position = v),
                Transformation.Rotation => motionBuilder.Bind(this, (v, t) => t.Rotation = v),
                Transformation.Scale => motionBuilder.Bind(this, (v, t) => t.Scale = v),
                _ => default
            };

            sequenceBuilder.Join(motion);
        }
    }
}