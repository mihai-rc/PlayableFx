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
        
        /// <summary>
        /// The duration of the <see cref="Tween"/>.
        /// </summary>
        public float Duration { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public TweenSettings Settings { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Vector3 Position { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Vector3 Rotation { get; private set; }
        
        /// <summary>
        /// 
        /// </summary>
        public Vector3 Scale { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Vector3 FromPosition
        {
            get => m_FromPosition;
            private set
            {
                m_FromPosition = Settings.PositionConfig.OverrideCurrentValues 
                    ? Settings.PositionConfig.From 
                    : value;
                
                Position = m_FromPosition;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public Vector3 FromRotation
        {
            get => m_FromRotation;
            private set
            {
                m_FromRotation = Settings.RotationConfig.OverrideCurrentValues 
                    ? Settings.RotationConfig.From 
                    : value;
                
                Rotation = m_FromRotation;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public Vector3 FromScale
        {
            get => m_FromScale;
            private set
            {
                m_FromScale = Settings.ScaleConfig.OverrideCurrentValues 
                    ? Settings.ScaleConfig.From 
                    : value;
                
                Scale = m_FromScale;
            }
        }

        private Vector3 m_FromPosition;
        private Vector3 m_FromRotation;
        private Vector3 m_FromScale;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public Tween ForSeconds(float duration)
        {
            Duration = duration;
            return this;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public Tween FromValues(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            FromPosition = position;
            FromRotation = rotation;
            FromScale = scale;

            return this;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public Tween ToSettings(TweenSettings settings)
        {
            Settings = settings;
            return this;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        public void At(float time)
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        public void Deconstruct(out Vector3 position, out Vector3 rotation, out Vector3 scale)
        {
            position = Position;
            rotation = Rotation;
            scale = Scale;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="settings"></param>
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
        public void Complete() => At(Duration);
        
        /// <summary>
        /// Resets the <see cref="Transform"/> to its original values.
        /// </summary>
        public void Revert() => At(0f);

        private void CreateMotion(MotionSequenceBuilder sequenceBuilder, TweenConfig config, Transformation transformation)
        {
            var from = transformation switch
            {
                Transformation.Position => m_FromPosition,
                Transformation.Rotation => m_FromRotation,
                Transformation.Scale => m_FromScale,
                _ => default
            };
            
            var motionBuilder = LMotion.Create(from, config.To, Duration);
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