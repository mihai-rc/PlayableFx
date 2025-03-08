using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.Playables;

namespace PlayableFx
{
    /// <summary>
    /// The behavior that will be attached to the <see cref="TweenClip"/>
    /// </summary>
    public class TweenBehavior : PlayableBehaviour
    {
        private enum Transformation
        {
            Position,
            Rotation,
            Scale
        }
        
        private Transform m_Transform;
        private Vector3 m_OriginalPosition;
        private Vector3 m_OriginalRotation;
        private Vector3 m_OriginalScale;

        /// <summary>
        /// The <see cref="Transform"/> to animate.
        /// </summary>
        public Transform Transform 
        {
            get => m_Transform;
            set
            {
                m_Transform = value;
                m_OriginalPosition = value.position;
                m_OriginalRotation = value.eulerAngles;
                m_OriginalScale = value.localScale;
            }
        }
        
        /// <summary>
        /// The duration of the clip.
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// Position tween settings.
        /// </summary>
        public TweenClip.Settings PositionSettings { get; set; }

        /// <summary>
        /// Rotation tween settings.
        /// </summary>
        public TweenClip.Settings RotationSettings { get; set; }

        /// <summary>
        /// Scale tween settings.
        /// </summary>
        public TweenClip.Settings ScaleSettings { get; set; }

        public override void OnGraphStart(Playable playable)
        {
            if (Transform is null)
                return;
            
            if (PositionSettings.Enabled)
                Transform.position = m_OriginalPosition;
            
            if (RotationSettings.Enabled)
                Transform.eulerAngles = m_OriginalRotation;
            
            if (ScaleSettings.Enabled)
                Transform.localScale = m_OriginalScale;
        }

        // public override void OnBehaviourPause(Playable playable, FrameData info)
        // {
        //     if (m_Transform is null)
        //         return;
        //
        //     var currentTime = playable
        //         .GetGraph()
        //         .GetRootPlayable(0)
        //         .GetTime();
        //
        //     if (currentTime <= m_Clip.start)
        //     {
        //         if (PositionSettings.Enabled)
        //             m_Transform.position = PositionSettings.From;
        //
        //         if (RotationSettings.Enabled)
        //             m_Transform.eulerAngles = RotationSettings.From;
        //
        //         if (ScaleSettings.Enabled)
        //             m_Transform.localScale = ScaleSettings.From;
        //     }
        //
        //     if (currentTime >= m_Clip.end)
        //     {
        //         if (PositionSettings.Enabled)
        //             m_Transform.position = PositionSettings.To;
        //
        //         if (RotationSettings.Enabled)
        //             m_Transform.eulerAngles = RotationSettings.To;
        //
        //         if (ScaleSettings.Enabled)
        //             m_Transform.localScale = ScaleSettings.To;
        //     }
        // }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (Transform is null)
                return;

            var sequenceBuilder = LSequence.Create();

            if (PositionSettings.Enabled)
                CreateMotion(sequenceBuilder, PositionSettings, Transform, Transformation.Position);

            if (RotationSettings.Enabled)
                CreateMotion(sequenceBuilder, RotationSettings, Transform, Transformation.Rotation);

            if (ScaleSettings.Enabled)
                CreateMotion(sequenceBuilder, ScaleSettings, Transform, Transformation.Scale);

            var sequence = sequenceBuilder.Run();
            sequence.Preserve();
            sequence.PlaybackSpeed = 0f;
            sequence.Time = playable.GetTime();
            sequence.TryCancel();
        }
        
        // public void CacheOriginalValues(Vector3 position, Vector3 rotation, Vector3 scale)
        // {
        //     m_OriginalPosition = position;
        //     m_OriginalRotation = rotation;
        //     m_OriginalScale = scale;
        // }

        private void CreateMotion(MotionSequenceBuilder sequenceBuilder, 
            TweenClip.Settings settings,
            Transform transform, 
            Transformation transformation)
        {
            var motionBuilder = LMotion.Create(settings.From, settings.To, Duration);
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