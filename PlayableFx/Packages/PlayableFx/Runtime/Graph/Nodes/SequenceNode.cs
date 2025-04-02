using System.Collections.Generic;
using GiftHorse.ScriptableGraphs;

namespace PlayableFx
{
    public abstract class SequenceNode : ScriptableNode
    {
        protected void FetchFlowNodesOf(string portName, List<IAsyncPlayable> nodes)
        {
            if (!TryFindOutPortByName(portName, out var outPort))
                return;
            
            foreach (var connectionId in outPort.ConnectionIds)
            {
                if (!Graph.TryGetOutputNode(connectionId, out ScriptableNode node))
                    continue;
                
                if (node is not IAsyncPlayable asyncNode)
                    continue;
                
                nodes.Add(asyncNode);
            }
        }
    }
}