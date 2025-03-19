using GiftHorse.ScriptableGraphs;

namespace PlayableFx
{
    public class SequenceGraph : ScriptableGraphOf<SequenceNode>
    {
        protected override void OnConnect(SequenceNode fromNode, OutPort fromPort, SequenceNode toNode, InPort toPort)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnDisconnect(SequenceNode fromNode, OutPort fromPort, SequenceNode toNode, InPort toPort)
        {
            throw new System.NotImplementedException();
        }
    }
}
