using GiftHorse.ScriptableGraphs;
using GiftHorse.ScriptableGraphs.Attributes;

namespace PlayableFx
{
    [NodeScript("Tweening")]
    public class TweenConfigNode : SequenceNode
    {
        [Output] public TweenConfig Value;
        [NodeField] public TweenConfig Config;
        
        protected override void OnProcess(ScriptableGraph graph)
        {
            Value = Config;
        }
    }
}