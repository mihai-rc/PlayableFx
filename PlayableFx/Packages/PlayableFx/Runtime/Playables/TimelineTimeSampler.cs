using UnityEngine;

namespace PlayableFx
{
    public class TimelineTimeSampler : EffectTimeSampler<PlayableTimeline>
    {
        private const string k_NullDirectorError = "Can't Create TimelinePlayer: {0} because no PlayableDirector was provided.";
        
        public override float Duration => (float)Effect.Director.duration;
        
        public TimelineTimeSampler(PlayableTimeline effect) 
            : base(effect)
        {
            ReportErrorIfDirectorIsNull();
        }
        
        protected override void OnSetTime(float time)
        {
            Effect.Director.time = time;
            Effect.Director.Evaluate();
        }

        private void ReportErrorIfDirectorIsNull()
        {
            if (Effect.Director is not null)
            {
                return;
            }
            
            Debug.LogError(string.Format(k_NullDirectorError, Effect.name));
        }
    }
}