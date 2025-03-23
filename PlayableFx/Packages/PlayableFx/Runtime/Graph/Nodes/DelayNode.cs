using GiftHorse.ScriptableGraphs.Attributes;

namespace PlayableFx
{
    [NodeScript]
    public class DelayNode : AsyncProcessNode
    {
        [NodeField] public float Duration;
    }
}