using System;
using GiftHorse.ScriptableGraphs.Attributes;

namespace PlayableFx
{
    [Serializable] 
    [NodeScript(ExcludeFromSearch = true), HeaderColor(0.5f, 0.2f, 0.2f)]
    public class SequenceRootNode : AsyncProcessNode
    {
    }
}