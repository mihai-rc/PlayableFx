using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using GiftHorse.ScriptableGraphs.Attributes;

namespace PlayableFx
{
    [NodeScript, HeaderColor(0.29f, 0.43f, 0.17f)]
    public class PlayableEffectNode : AsyncProcessNode
    {
        private const string k_NullPlayableEffectHolder = "[SequenceGraph] [PlayableEffectNode] Cannot play node with Id: {0} because no PlayableEffectHolder was assigned! Graph name: {1}";
        private const string k_NullPlayableEffect = "[SequenceGraph] [PlayableEffectNode] Cannot play node with Id: {0} because no PlayableEffect was assigned! Graph name: {1}";
        
        [Input] public string In;
        [NodeField] public PlayableEffectHolder EffectHolder;

        private PlayableEffect m_PlayableEffect;
        
        protected override void OnProcess()
        {
            base.OnProcess();
            if (EffectHolder is null)
            {
                Debug.LogErrorFormat(k_NullPlayableEffectHolder, Id, Graph.name);
                return;
            }
            
            m_PlayableEffect = EffectHolder.Effect;
            if (m_PlayableEffect is null)
            {
                Debug.LogErrorFormat(k_NullPlayableEffect, Id, Graph.name);
                return;
            }
        }

        protected override async UniTask OnProcessAsync(CancellationToken cancellation)
        {
            if (m_PlayableEffect is null)
            {
                Debug.LogErrorFormat(k_NullPlayableEffect, Id, Graph.name);
                return;
            }
            
            await m_PlayableEffect.PlayAsync(cancellation);
        }
    }
}