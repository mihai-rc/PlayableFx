using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GiftHorse.ScriptableGraphs;
using GiftHorse.ScriptableGraphs.Attributes;
using UnityEngine.Pool;

namespace PlayableFx
{
    public class AsyncProcessNode : SequenceNode
    {
        [Output] public string AfterBranch;
        [Output] public string AfterEffect;
        [Input] public string From;

        private List<AsyncProcessNode> m_BranchContinuations; // TODO: Use pool
        private List<AsyncProcessNode> m_EffectContinuations; // TODO: Use pool

        protected override void OnCreate(ScriptableGraph graph)
        {
            m_BranchContinuations = new List<AsyncProcessNode>();
            m_EffectContinuations = new List<AsyncProcessNode>();
            
            foreach (var outPort in OutPorts)
            {
                if (!outPort.Name.Equals(nameof(AfterEffect)) && !outPort.Name.Equals(nameof(AfterBranch))) 
                    continue;

                var continuations = outPort.Name.Equals(nameof(AfterEffect)) 
                    ? m_EffectContinuations 
                    : m_BranchContinuations;
                
                foreach (var connectionId in outPort.ConnectionIds)
                {
                    if (!graph.TryGetConnectionById(connectionId, out var connection))
                    {
                        // TODO: Log Error
                        continue;
                    }
                        
                    if (!graph.TryGetNodeById(connection.ToPort.NodeId, out var node))
                    {
                        // TODO: Log Error
                        continue;
                    }

                    if (node is not AsyncProcessNode asyncNode)
                    {
                        // TODO: Log Error
                        continue;
                    }
                        
                    continuations.Add(asyncNode);
                }
            }
        }
        
        protected override void OnProcess(ScriptableGraph graph)
        {
            AfterEffect = Id;
            AfterBranch = Id;
        }

        protected virtual UniTask OnProcessAsync() => UniTask.CompletedTask;

        public async UniTask ProcessAsync()
        {
            await OnProcessAsync();
            
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