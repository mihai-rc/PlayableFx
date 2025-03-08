using UnityEngine;

namespace PlayableFx
{
    public abstract class PlayableEffect : MonoBehaviour, IPlayableEffect
    {
        private ITimeSampler m_TimeSampler;
        
        public ITimeSampler TimeSampler
        {
            get
            {
                if (m_TimeSampler is null)
                    m_TimeSampler = CreateSampler();

                return m_TimeSampler;
            }
        }

        protected abstract ITimeSampler CreateSampler();
    }
}