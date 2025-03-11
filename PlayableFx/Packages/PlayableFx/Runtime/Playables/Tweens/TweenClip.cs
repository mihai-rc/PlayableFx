using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PlayableFx
{
    [Serializable]
    public class TweenClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] private TweenSettings m_Settings;
        
        public ClipCaps clipCaps => ClipCaps.Extrapolation | ClipCaps.Blending;
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TweenBehavior>.Create(graph);
            playable.GetBehaviour().Settings = m_Settings;
            
            return playable;
        }
    }
}
