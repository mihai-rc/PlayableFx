using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PlayableFx
{
    /// <summary>
    /// The track that holds the tween clips.
    /// </summary>
    [TrackColor(148/255f, 222/255f, 89/255f)]
    [TrackBindingType(typeof(Transform))]
    [TrackClipType(typeof(TweenClip))]
    public class TweenTrack : TrackAsset
    {
        protected override Playable CreatePlayable(PlayableGraph graph, GameObject gameObject, TimelineClip clip)
        {
            // Playable.GetDuration() returns some super strange results when ClipCaps.Extrapolation is set.
            // In order to make extrapolation work intuitively with LitTween we need clip.duration, so we override CreatePlayable and pass the current clip to our behavior.
            // Thank my dude here https://forum.unity.com/threads/trying-to-get-percentage-of-the-way-through-playable.503672/#post-3281262
            // additional info here https://forum.unity.com/threads/timeline-adds-1-million-to-playable-getduration-when-extrapolation-is-set-to-anything-but-none.1324440/
            var playable = (ScriptPlayable<TweenBehavior>)base.CreatePlayable(graph, gameObject, clip);
        
            // Grab the reference exposed on the track so we can initialize our TweenBehavior with its values.
            // var transformBinding = gameObject
            //     .GetComponent<PlayableDirector>()
            //     .GetGenericBinding(this) as Transform;
            //
            // if (transformBinding is null)
            //     return playable;
        
            // ref var tween = ref playable.GetBehaviour().Tween;
            // tween.Transform = transformBinding;
            playable.GetBehaviour().Duration = (float)clip.duration;
            
            return playable;
        }

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject gameObject, int inputCount)
        {
            return ScriptPlayable<TweenTrackMixer>.Create(graph, inputCount);
        }
    }
}
