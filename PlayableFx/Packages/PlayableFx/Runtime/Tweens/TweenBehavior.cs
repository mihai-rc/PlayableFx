using UnityEngine.Playables;

namespace PlayableFx
{
    /// <summary>
    /// The behavior that will be attached to the <see cref="TweenClip"/>
    /// </summary>
    public class TweenBehavior : PlayableBehaviour
    {
        private Tween m_Tween;
        private bool m_IsFirstFrame = true;

        /// <summary>
        /// Reference to the tween that will be played.
        /// </summary>
        public ref Tween Tween => ref m_Tween;

        public override void OnGraphStart(Playable playable)
        {
            m_Tween.ResetToOriginalValues();
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (m_IsFirstFrame)
            {
                m_IsFirstFrame = false;
                m_Tween.PrepareStartValues();
            }
            
            m_Tween.Time = (float)playable.GetTime();
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            m_IsFirstFrame = true;
        }

        /// <summary>
        /// Creates the tween with the given settings.
        /// </summary>
        /// <param name="positionSettings"> Position tween settings. </param>
        /// <param name="rotationSettings"> Rotation tween settings. </param>
        /// <param name="scaleSettings"> Scale tween settings. </param>
        public void CreateTween(TweenSettings positionSettings, TweenSettings rotationSettings, TweenSettings scaleSettings)
        {
            m_Tween = new Tween(positionSettings, rotationSettings, scaleSettings);
        }
    }
}