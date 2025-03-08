using UnityEngine;

namespace PlayableFx
{
    public abstract class EffectTimeSampler<T> : IEffectTimeSampler<T> 
        where T : PlayableEffect
    {
        public abstract float Duration { get; }
        protected abstract void OnSetTime(float time);
        
        private const string k_OutOfBoundsTimeWarning = "[PlayableEffect] Cannot sample Effect: {0}, the time ({1:G17}s) is larger then the duration ({2:G17}s).";
        
        private float m_Time;
        
        public float Time
        {
            get => m_Time;
            set => SetTime(value);
        }
        
        public T Effect { get; }
        
        protected EffectTimeSampler(T effect)
        {
            Effect = effect;
        }

        private void SetTime(float time)
        {
            if (time > Duration)
            {
                Debug.LogError(string.Format(k_OutOfBoundsTimeWarning, Effect.name, time, Duration));
                return;
            }

            m_Time = time;
            OnSetTime(time);
        }
    }
}