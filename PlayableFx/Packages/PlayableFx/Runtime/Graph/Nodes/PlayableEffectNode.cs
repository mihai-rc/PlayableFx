using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using GiftHorse.ScriptableGraphs.Attributes;

namespace PlayableFx
{
    // [NodeScript, HeaderColor(0.15f, 0.4f, 0.4f)]
    [NodeScript, HeaderColor(0.29f, 0.43f, 0.17f)]
    public class PlayableEffectNode : AsyncProcessNode
    {
        private const string k_PlayableEffectNotAssigned = "[SequenceGraph] [PlayableEffectNode] Cannot play node with Id: {0} because no PlayableEffect was assigned! Graph name: {1}";

        [Input] public string In;
        [NodeField] public PlayableEffect PlayableEffect;

        protected override void OnProcess()
        {
            base.OnProcess();
            
            if (PlayableEffect != null) 
                return;
            
            Debug.LogErrorFormat(k_PlayableEffectNotAssigned, Id, Graph.name);
        }

        protected override async UniTask OnProcessAsync() => await PlayableEffect.PlayAsync(CancellationToken.None);
    }
}