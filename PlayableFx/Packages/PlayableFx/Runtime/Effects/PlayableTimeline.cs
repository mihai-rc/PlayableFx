using System.Threading;
using UnityEngine;
using UnityEngine.Playables;
using Cysharp.Threading.Tasks;

namespace PlayableFx
{
    public class PlayableTimeline : PlayableEffect
    {
        [SerializeField] public PlayableDirector m_Director;
        
        public override async UniTask PlayAsync(CancellationToken cancellation)
        {
            if (m_Director == null)
            {
                Debug.LogErrorFormat("[PlayableTimeline] Cannot play timeline: {0} because no PlayableDirector was assigned!", name);
                return;
            }

            m_Director.Play();
            while (m_Director.state == PlayState.Playing)
            {
                if (cancellation.IsCancellationRequested)
                {
                    m_Director.Stop();
                    return;
                }
                
                await UniTask.Yield();
            }
        }
    }
}