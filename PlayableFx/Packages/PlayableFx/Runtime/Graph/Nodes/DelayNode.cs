using System.Threading;
using Cysharp.Threading.Tasks;
using GiftHorse.ScriptableGraphs.Attributes;
using UnityEngine;

namespace PlayableFx
{
    [NodeScript]
    public class DelayNode : AsyncProcessNode
    {
        private const string k_InvalidDuration = "[DelayNode] Duration is zero or negative. Graph name: {0}";
        
        [Input] public string In;
        [NodeField] public float Duration;
        
        protected override UniTask OnProcessAsync(CancellationToken cancellation)
        {
            if (Duration <= 0)
            {
                Debug.LogWarningFormat(k_InvalidDuration, Graph.name);
                return UniTask.CompletedTask;
            }

            return UniTask.Delay((int)(Duration * 1000), cancellationToken: cancellation);
        }
    }
}