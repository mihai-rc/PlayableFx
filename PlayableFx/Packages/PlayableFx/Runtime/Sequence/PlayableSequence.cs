using UnityEngine;

namespace PlayableFx
{
    public class PlayableSequence : MonoBehaviour, IPlayableSequence
    {
        [SerializeField] private PlayableEffect m_Effect;

        private ISequencePlayer m_SequencePlayer;

        public IPlayableEffect Effect => m_Effect;

        public ISequencePlayer Player
        {
            get
            {
                if (m_Effect is null)
                    return null;
                
                if (m_SequencePlayer is null)
                    m_SequencePlayer = new SequencePlayer(name, m_Effect.TimeSampler);

                return m_SequencePlayer;
            }
        }
    }
}