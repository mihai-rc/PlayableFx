using GiftHorse.ScriptableGraphs.Attributes;

namespace PlayableFx
{
    public abstract class AsyncFlowNode : AsyncProcessNode
    {
        [Input] public string From;
    }
}