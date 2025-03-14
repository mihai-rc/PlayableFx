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

        public TweenSettings Settings
        {
            set => Tween.Settings = value;
        }
        
        public float Duration
        {
            set => Tween.Duration = value;
        }
        
        /// <summary>
        /// Creates the tween with the given settings.
        /// </summary>
        /// <param name="positionSettings"> Position tween settings. </param>
        /// <param name="rotationSettings"> Rotation tween settings. </param>
        /// <param name="scaleSettings"> Scale tween settings. </param>
        // public void CreateTween(TweenSettings positionSettings, TweenSettings rotationSettings, TweenSettings scaleSettings)
        // {
        //     m_Tween = new Tween(positionSettings, rotationSettings, scaleSettings);
        // }

        // public override void OnGraphStart(Playable playable)
        // {
        //     m_Tween.Revert();
        // }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (m_IsFirstFrame)
            {
                if (playerData is not Transform transform)
                {
                    return;
                }
                
                m_IsFirstFrame = false;
                Tween.FromPosition = transform.localPosition;
                Tween.FromRotation = transform.localEulerAngles;
                Tween.FromScale = transform.localScale;
            }
            
            Tween.Time = (float)playable.GetTime();
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            m_IsFirstFrame = true;
            
            // if (info.weight != 0f)
            //     return;
            //
            // m_Tween.Complete();
        }
    }
}