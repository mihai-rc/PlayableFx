using System;
using LitMotion;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PlayableFx
{
    [Serializable]
    public class TweenClip : PlayableAsset, ITimelineClipAsset
    {
        [Serializable]
        public struct Settings
        {
            public bool Enabled;
            public Vector3 From;
            public Vector3 To;
            public Ease Ease;
            public AnimationCurve Curve;
        }
        
        [Space] public Settings Position;
        [Space] public Settings Rotation;
        [Space] public Settings Scale;
        
        public ClipCaps clipCaps => ClipCaps.Extrapolation;
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TweenBehavior>.Create(graph);
            var tweenBehavior = playable.GetBehaviour();
            
            tweenBehavior.PositionSettings = Position;
            tweenBehavior.RotationSettings = Rotation;
            tweenBehavior.ScaleSettings = Scale;
            
            return playable;
        }
    }
}
