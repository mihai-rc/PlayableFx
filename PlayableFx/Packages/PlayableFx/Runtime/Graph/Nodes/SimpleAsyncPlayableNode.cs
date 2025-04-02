using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using GiftHorse.ScriptableGraphs.Attributes;
using UnityEngine.Pool;

namespace PlayableFx
{
    public class SimpleAsyncPlayableNode : SequenceNode, IAsyncPlayable
    {
        [Output] public string Out;

        private List<IAsyncPlayable> m_OutputNodes;
        
        public async UniTask PlayAsync(CancellationToken cancellation)
        {
            await OnPlayAsync(cancellation);
            
            var outputProcesses = ListPool<UniTask>.Get();

            foreach (var node in m_OutputNodes)
                outputProcesses.Add(node.PlayAsync(cancellation));
            
            await UniTask.WhenAll(outputProcesses);
            
            outputProcesses.Clear();
            ListPool<UniTask>.Release(outputProcesses);
        }

        protected override void OnInit()
        {
            m_OutputNodes = ListPool<IAsyncPlayable>.Get();
            FetchFlowNodesOf(nameof(Out), m_OutputNodes);
        }
        
        protected override void OnProcess() => Out = Id;
        
        protected virtual async UniTask OnPlayAsync(CancellationToken cancellation) { }

        protected override void OnDispose() => ListPool<IAsyncPlayable>.Release(m_OutputNodes);
    }
}