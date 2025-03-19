using UnityEngine;
using UnityEngine.Playables;

namespace PlayableFx
{
    /// <summary>
    /// The behavior that will be attached to the <see cref="TweenClip"/>
    /// </summary>
    public class TweenBehavior : PlayableBehaviour
    {
        private TransformTween m_Tween;
        private bool m_IsFirstFrame = true;

        /// <summary>
        /// Reference to the tween that will be played.
        /// </summary>
        public TransformTween Tween
        {
            get
            {
                if (m_Tween is null)
                    m_Tween = new TransformTween();
                
                return m_Tween;
            }
        }

        /// <summary>
        /// Sets the settings of the tween.
        /// </summary>
        public TweenSettings Settings
        {
            set => Tween.SetSettings(value);
        }
        
        /// <summary>
        /// Sets the duration of the tween.
        /// </summary>
        public float Duration
        {
            set => Tween.SetDuration(value);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (m_IsFirstFrame)
            {
                if (playerData is not Transform transform)
                {
                    return;
                }
                
                m_IsFirstFrame = false;
                Tween.SetStartingValues(transform.localPosition, transform.localEulerAngles, transform.localScale);
            }
            
            Tween.ComputeAt((float)playable.GetTime());
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            m_IsFirstFrame = true;
        }
    }
}