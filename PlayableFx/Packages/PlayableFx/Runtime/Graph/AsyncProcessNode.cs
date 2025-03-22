using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GiftHorse.ScriptableGraphs;
using GiftHorse.ScriptableGraphs.Attributes;
using UnityEngine;

namespace PlayableFx
{
    public class AsyncProcessNode : SequenceNode
    {
        [Output] public string To;

        private List<AsyncProcessNode> m_Continuations;

        protected virtual async UniTask OnProcessAsync() { }

        protected override void OnProcess(ScriptableGraph graph) => To = Id;

        protected override void OnCreate(ScriptableGraph graph)
        {
            m_Continuations = new List<AsyncProcessNode>();
            foreach (var outPort in OutPorts)
            {
                if (!outPort.Name.Equals("To")) 
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

        public async UniTask ProcessAsync()
        {
            Debug.Log($"--- Begin ProcessAsync --- Id: {Id}");
            await OnProcessAsync();
            Debug.Log($"--- Begin Children --- Id: {Id}, Count: {m_Continuations.Count}");
            foreach (var continuation in m_Continuations)
            {
                await continuation.ProcessAsync();
            }
            
            Debug.Log($"--- End ProcessAsync --- Id: {Id}");
        }
    }
}