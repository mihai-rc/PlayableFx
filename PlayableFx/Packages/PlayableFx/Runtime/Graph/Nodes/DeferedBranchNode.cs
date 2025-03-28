using System.Collections.Generic;
using System.Threading;
using UnityEngine.Pool;
using Cysharp.Threading.Tasks;
using GiftHorse.ScriptableGraphs.Attributes;

namespace PlayableFx
{
    [NodeScript, HeaderColor(0.15f, 0.4f, 0.4f)]
    public class DeferredBranchNode : SequenceNode, IAsyncProcessNode
    {
        [Input] public string In;
        [Output] public string Defer;
        [Output] public string Current;
        
        private List<IAsyncProcessNode> m_DeferredContinuations;
        private List<IAsyncProcessNode> m_CurrentContinuations;

        protected override void OnInit()
        {
            m_DeferredContinuations = ListPool<IAsyncProcessNode>.Get();
            m_CurrentContinuations = ListPool<IAsyncProcessNode>.Get();
            
            FetchFlowNodesOf(nameof(Defer), m_DeferredContinuations);
            FetchFlowNodesOf(nameof(Current), m_CurrentContinuations);
        }
        
        protected override void OnProcess()
        {
            Current = Id;
            Defer = Id;
        }

        protected override void OnDispose()
        {
            ListPool<IAsyncProcessNode>.Release(m_DeferredContinuations);
            ListPool<IAsyncProcessNode>.Release(m_CurrentContinuations);
        }

        public async UniTask ProcessAsync(CancellationToken cancellation)
        {
            var tasks = ListPool<UniTask>.Get();
            
            foreach (var continuation in m_CurrentContinuations)
                tasks.Add(continuation.ProcessAsync(cancellation));
            
            await UniTask.WhenAll(tasks);
            tasks.Clear();
            
            foreach (var continuation in m_DeferredContinuations)
                tasks.Add(continuation.ProcessAsync(cancellation));
            
            await UniTask.WhenAll(tasks);
            tasks.Clear();
            
            ListPool<UniTask>.Release(tasks);
        }
    }
}