using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GiftHorse.ScriptableGraphs.Attributes;
using UnityEngine.Pool;

namespace PlayableFx
{
    public class AsyncProcessNode : SequenceNode, IAsyncProcessNode
    {
        [Output] public string Out;

        private List<IAsyncProcessNode> m_OutputNodes;
        
        public async UniTask ProcessAsync()
        {
            await OnProcessAsync();
            
            var outputProcesses = ListPool<UniTask>.Get();
            outputProcesses.AddRange(Enumerable.Select(m_OutputNodes, node => node.ProcessAsync()));
            await UniTask.WhenAll(outputProcesses);
            
            outputProcesses.Clear();
            ListPool<UniTask>.Release(outputProcesses);
        }

        protected override void OnInit()
        {
            m_OutputNodes = ListPool<IAsyncProcessNode>.Get();

            if (!TryFindOutPortByName(nameof(Out), out var startPort))
                return;
            
            foreach (var connectionId in startPort.ConnectionIds)
            {
                if (!Graph.TryGetOutputNode(connectionId, out AsyncProcessNode asyncNode))
                    continue;
                        
                m_OutputNodes.Add(asyncNode);
            }
        }
        
        protected override void OnProcess() => Out = Id;
        
        protected virtual async UniTask OnProcessAsync() { }

        protected override void OnDispose() => ListPool<IAsyncProcessNode>.Release(m_OutputNodes);
    }
}