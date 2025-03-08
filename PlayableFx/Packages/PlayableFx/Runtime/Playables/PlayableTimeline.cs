using UnityEngine;
using UnityEngine.Playables;

namespace PlayableFx
{
    public class PlayableTimeline : PlayableEffect
    {
        [field: SerializeField] 
        public PlayableDirector Director { get; private set; }

        protected override ITimeSampler CreateSampler()
        {
            return new TimelineTimeSampler(this);
        }
    }
}