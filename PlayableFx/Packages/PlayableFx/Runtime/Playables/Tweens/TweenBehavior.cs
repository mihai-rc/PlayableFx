using UnityEngine;
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
        public Tween Tween
        {
            get
            {
                if (m_Tween is null)
                    m_Tween = new Tween();
                
                return m_Tween;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TweenSettings Settings
        {
            set => Tween.ToSettings(value);
        }
        
        /// <summary>
        /// 
        /// </summary>
        public float Duration
        {
            set => Tween.ForSeconds(value);
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
                Tween.FromValues(transform.localPosition, transform.localEulerAngles, transform.localScale);
            }
            
            Tween.At((float)playable.GetTime());
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            m_IsFirstFrame = true;
        }
    }
}