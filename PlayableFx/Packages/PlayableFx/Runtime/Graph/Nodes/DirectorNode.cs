using UnityEngine.Playables;
using GiftHorse.ScriptableGraphs.Attributes;

namespace PlayableFx
{
    [NodeScript, HeaderColor(0.15f, 0.4f, 0.4f)]
    public class DirectorNode : AsyncProcessNode
    {
        [NodeField] public PlayableDirector Director;
    }
}