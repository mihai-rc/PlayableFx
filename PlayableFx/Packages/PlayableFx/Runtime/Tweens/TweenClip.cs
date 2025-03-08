using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PlayableFx
{
    [Serializable]
    public class TweenClip : PlayableAsset, ITimelineClipAsset
    {
        public TweenSettings Position;
        public TweenSettings Rotation;
        public TweenSettings Scale;
        
        public ClipCaps clipCaps => ClipCaps.Extrapolation;
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TweenBehavior>.Create(graph);
            playable.GetBehaviour().CreateTween(Position, Rotation, Scale);
            
            return playable;
        }
    }
}
