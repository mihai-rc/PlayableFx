using UnityEngine;

namespace PlayableFx
{
    public abstract class TimeSampler : ITimeSampler
    {
        private const string k_OutOfBoundsTimeWarning = "[PlayableEffect] Cannot sample Effect: {0}, the time ({1:G17}s) is larger then the duration ({2:G17}s).";

        private readonly string m_Name;
        private float m_Time;
        
        public abstract float Duration { get; }
        
        public float Time
        {
            get => m_Time;
            set => SetTime(value);
        }
        
        protected abstract void OnSetTime(float time);

        private void SetTime(float time)
        {
            if (time > Duration)
            {
                Debug.LogError(string.Format(k_OutOfBoundsTimeWarning, m_Name, time, Duration));
                return;
            }

            m_Time = time;
            OnSetTime(time);
        }
    }
}