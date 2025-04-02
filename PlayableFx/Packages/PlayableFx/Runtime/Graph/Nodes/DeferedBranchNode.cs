using System.Collections.Generic;
using System.Threading;
using UnityEngine.Pool;
using Cysharp.Threading.Tasks;
using GiftHorse.ScriptableGraphs.Attributes;

namespace PlayableFx
{
    [NodeScript, HeaderColor(0.15f, 0.4f, 0.4f)]
    public class DeferredBranchNode : SequenceNode, IAsyncPlayable
    {
        [Input] public string In;
        [Output] public string Defer;
        [Output] public string Current;
        
        private List<IAsyncPlayable> m_CurrentNodes;
        private List<IAsyncPlayable> m_DeferredNodes;

        protected override void OnInit()
        {
            m_DeferredNodes = ListPool<IAsyncPlayable>.Get();
            m_CurrentNodes = ListPool<IAsyncPlayable>.Get();
            
            FetchFlowNodesOf(nameof(Defer), m_DeferredNodes);
            FetchFlowNodesOf(nameof(Current), m_CurrentNodes);
        }
        
        protected override void OnProcess()
        {
            Current = Id;
            Defer = Id;
        }

        protected override void OnDispose()
        {
            ListPool<IAsyncPlayable>.Release(m_CurrentNodes);
            ListPool<IAsyncPlayable>.Release(m_DeferredNodes);
        }

        public async UniTask PlayAsync(CancellationToken cancellation)
        {
            var tasks = ListPool<UniTask>.Get();
            
            foreach (var continuation in m_CurrentNodes)
                tasks.Add(continuation.PlayAsync(cancellation));
            
            await UniTask.WhenAll(tasks);
            tasks.Clear();
            
            foreach (var continuation in m_DeferredNodes)
                tasks.Add(continuation.PlayAsync(cancellation));
            
            await UniTask.WhenAll(tasks);
            tasks.Clear();
            
            ListPool<UniTask>.Release(tasks);
        }
    }
}