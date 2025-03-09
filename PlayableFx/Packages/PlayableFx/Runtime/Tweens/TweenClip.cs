using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PlayableFx
{
    [Serializable]
    public class TweenClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] private TweenSettings m_Position;
        [SerializeField] private TweenSettings m_Rotation;
        [SerializeField] private TweenSettings m_Scale;
        
        public ClipCaps clipCaps => ClipCaps.Extrapolation | ClipCaps.Blending;
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TweenBehavior>.Create(graph);
            playable.GetBehaviour().CreateTween(m_Position, m_Rotation, m_Scale);
            
            return playable;
        }
    }
}
