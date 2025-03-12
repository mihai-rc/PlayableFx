using UnityEngine;
using LitMotion;
using UnityEditor;

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

        public TweenSettings Settings { get; set; }

        public Vector3 Position { get; private set; }

        public Vector3 Rotation { get; private set; }
        
        public Vector3 Scale { get; private set; }

        public Vector3 FromPosition
        {
            get => m_FromPosition;
            set
            {
                m_FromPosition = Settings.PositionConfig.OverrideCurrentValues 
                    ? Settings.PositionConfig.From 
                    : value;
                
                Position = m_FromPosition;
            }
        }
        
        public Vector3 FromRotation
        {
            get => m_FromRotation;
            set
            {
                m_FromRotation = Settings.RotationConfig.OverrideCurrentValues 
                    ? Settings.RotationConfig.From 
                    : value;
                
                Rotation = m_FromRotation;
            }
        }
        
        public Vector3 FromScale
        {
            get => m_FromScale;
            set
            {
                m_FromScale = Settings.ScaleConfig.OverrideCurrentValues 
                    ? Settings.ScaleConfig.From 
                    : value;
                
                Scale = m_FromScale;
            }
        }
        
        /// <summary>
        /// The duration of the <see cref="Tween"/>.
        /// </summary>
        public float Duration
        {
            set => m_Duration = value;
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

        private Vector3 m_FromPosition;
        private Vector3 m_FromRotation;
        private Vector3 m_FromScale;
        

        /// <summary>
        /// <see cref="Tween"/>'s constructor.
        /// </summary>
        public Tween() { }
        
        public void Deconstruct(out Vector3 position, out Vector3 rotation, out Vector3 scale)
        {
            position = Position;
            rotation = Rotation;
            scale = Scale;
        }
        
        public void Deconstruct(out Vector3 position, out Vector3 rotation, out Vector3 scale, out TweenSettings settings)
        {
            position = Position;
            rotation = Rotation;
            scale = Scale;
            settings = Settings;
        }
        
        /// <summary>
        /// Sets the <see cref="Transform"/> to its final values.
        /// </summary>
        public void Complete() => SetTime(m_Duration);
        
        /// <summary>
        /// Resets the <see cref="Transform"/> to its original values.
        /// </summary>
        public void Revert() => SetTime(0f);
        
        private void SetTime(float time)
        {
            var sequenceBuilder = LSequence.Create();

            var positionConfig = Settings.PositionConfig;
            if (positionConfig.Enabled)
                CreateMotion(sequenceBuilder, positionConfig, Transformation.Position);

            var rotationConfig = Settings.RotationConfig;
            if (rotationConfig.Enabled)
                CreateMotion(sequenceBuilder, rotationConfig, Transformation.Rotation);

            var scaleConfig = Settings.ScaleConfig;
            if (scaleConfig.Enabled)
                CreateMotion(sequenceBuilder, scaleConfig, Transformation.Scale);

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