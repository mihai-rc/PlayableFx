using GiftHorse.ScriptableGraphs.Attributes;

namespace PlayableFx
{
    [NodeScript("Directing")]
    public class DelayNode : AsyncFlowNode
    {
        [NodeField] public float Duration;
    }
}