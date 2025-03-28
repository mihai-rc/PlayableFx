using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using GiftHorse.ScriptableGraphs.Attributes;
using UnityEngine.Pool;

namespace PlayableFx
{
    public class AsyncProcessNode : SequenceNode, IAsyncProcessNode
    {
        [Output] public string Out;

        private List<IAsyncProcessNode> m_OutputNodes;
        
        public async UniTask ProcessAsync(CancellationToken cancellation)
        {
            await OnProcessAsync(cancellation);
            
            var outputProcesses = ListPool<UniTask>.Get();
            outputProcesses.AddRange(Enumerable
                .Select(m_OutputNodes, node => node
                .ProcessAsync(cancellation)));
            
            await UniTask.WhenAll(outputProcesses);
            
            outputProcesses.Clear();
            ListPool<UniTask>.Release(outputProcesses);
        }

        protected override void OnInit()
        {
            m_OutputNodes = ListPool<IAsyncProcessNode>.Get();
            FetchFlowNodesOf(nameof(Out), m_OutputNodes);
        }
        
        protected override void OnProcess() => Out = Id;
        
        protected virtual async UniTask OnProcessAsync(CancellationToken cancellation) { }

        protected override void OnDispose() => ListPool<IAsyncProcessNode>.Release(m_OutputNodes);
    }
}