using UnityEngine.Playables;
using GiftHorse.ScriptableGraphs.Attributes;

namespace PlayableFx
{
    [NodeScript("Directing"), HeaderColor(0.15f, 0.4f, 0.4f)]
    public class DirectorNode : AsyncFlowNode
    {
        [NodeField] public PlayableDirector Director;
    }
}