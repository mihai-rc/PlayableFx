using System.Collections.Generic;
using UnityEngine.Pool;
using Cysharp.Threading.Tasks;
using GiftHorse.ScriptableGraphs.Attributes;

namespace PlayableFx
{
    public class DeferBranchNode : SequenceNode, IAsyncProcessNode
    {
        [Input] public string In;
        [Output] public string Defer;
        [Output] public string Current;
        
        private List<IAsyncProcessNode> m_BranchContinuations;
        private List<IAsyncProcessNode> m_EffectContinuations;

        protected override void OnInit()
        {
            m_BranchContinuations = ListPool<IAsyncProcessNode>.Get();
            m_EffectContinuations = ListPool<IAsyncProcessNode>.Get();
            
            if (!TryFindOutPortByName(nameof(Defer), out var afterBranchPort))
                return;
            
            foreach (var connectionId in afterBranchPort.ConnectionIds)
            {
                if (!Graph.TryGetOutputNode(connectionId, out AsyncProcessNode asyncNode))
                    continue;
                        
                m_BranchContinuations.Add(asyncNode);
            }
            
            if (!TryFindOutPortByName(nameof(Current), out var afterEffectPort))
                return;
            
            foreach (var connectionId in afterEffectPort.ConnectionIds)
            {
                if (!Graph.TryGetOutputNode(connectionId, out AsyncProcessNode asyncNode))
                    continue;
                        
                m_EffectContinuations.Add(asyncNode);
            }
        }
        
        protected override void OnProcess()
        {
            Current = Id;
            Defer = Id;
        }

        protected override void OnDispose()
        {
            ListPool<IAsyncProcessNode>.Release(m_BranchContinuations);
            ListPool<IAsyncProcessNode>.Release(m_EffectContinuations);
        }

        public async UniTask ProcessAsync()
        {
            var tasks = ListPool<UniTask>.Get();
            
            foreach (var continuation in m_EffectContinuations)
                tasks.Add(continuation.ProcessAsync());
            
            await UniTask.WhenAll(tasks);
            tasks.Clear();
            
            foreach (var continuation in m_BranchContinuations)
                tasks.Add(continuation.ProcessAsync());
            
            await UniTask.WhenAll(tasks);
            tasks.Clear();
            
            ListPool<UniTask>.Release(tasks);
        }
    }
}