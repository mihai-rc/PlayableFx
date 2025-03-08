using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PlayableFx
{
    /// <summary>
    /// The track that holds the tween clips
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
            var transformBinding = gameObject
                .GetComponent<PlayableDirector>()
                .GetGenericBinding(this) as Transform;
            
            if (transformBinding == null)
                return playable;

            var tweenBehaviour = playable.GetBehaviour();
            tweenBehaviour.Transform = transformBinding;
            tweenBehaviour.Duration = (float)clip.duration;

            // var firstClip = GetClips().FirstOrDefault();
            // if (firstClip is null) 
            //     return playable;
            //
            // if (firstClip.asset is not TweenClip tweenClip)
            //     return playable;
            
            // tweenBehaviour.CacheOriginalValues(tweenClip.Position.From, tweenClip.Rotation.From, tweenClip.Scale.From);
            
            return playable;
        }
    }
}
