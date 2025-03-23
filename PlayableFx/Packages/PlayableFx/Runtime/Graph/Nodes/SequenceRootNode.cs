using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GiftHorse.ScriptableGraphs;
using GiftHorse.ScriptableGraphs.Attributes;

namespace PlayableFx
{
    [Serializable] 
    [NodeScript(ExcludeFromSearch = true), HeaderColor(0.5f, 0.2f, 0.2f)]
    public class SequenceRootNode : SequenceNode
    {
        [Output] public string Start;
        
        private List<AsyncProcessNode> m_Continuations; // TODO: Use pool
        
        protected override void OnCreate(ScriptableGraph graph)
        {
            m_Continuations = new List<AsyncProcessNode>();
            foreach (var outPort in OutPorts)
            {
                if (!outPort.Name.Equals(nameof(Start))) 
                    continue;
                
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
                        
                    m_Continuations.Add(asyncNode);
                }
            }
        }
        
        protected override void OnProcess(ScriptableGraph graph) => Start = Id;
        
        public async UniTask ProcessAsync()
        {
            foreach (var continuation in m_Continuations)
            {
                await continuation.ProcessAsync();
            }
        }
    }
}