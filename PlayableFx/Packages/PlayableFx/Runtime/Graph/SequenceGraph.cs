using System.Linq;
using System.Threading;
using UnityEngine;
using GiftHorse.ScriptableGraphs;

namespace PlayableFx
{
    public class SequenceGraph : ScriptableGraphOf<SequenceNode>
    {
        private const string k_LostRootError = "[SequenceGraph] The graph of {0} is misconfugured, the SequenceRootNode was lost";
        private const string k_InvalidRootError = "[SequenceGraph] The graph of {0} is misconfugured, the SequenceRootNode could not be found";

        [SerializeField] private string m_RootNodeId;

        protected override void OnStart()
        {
            var cancellation = new CancellationTokenSource();

            foreach (var node in Nodes)
                node.Process();
            
            if (TryGetRoot(out var root))
                root.PlayAsync(cancellation.Token);
        }

        private void Reset()
        {
            TryGetRoot(out _);
        }

        private bool TryGetRoot(out SequenceRootNode root)
        {
            if (string.IsNullOrEmpty(m_RootNodeId) && Nodes.Any())
            {
                Debug.LogErrorFormat(k_LostRootError, name);
                    
                root = null;
                return false;
            }
            
            if (!string.IsNullOrEmpty(m_RootNodeId) && !Nodes.Any())
            {
                Debug.LogErrorFormat(k_InvalidRootError, name);
                    
                root = null;
                return false;
            }

            if (string.IsNullOrEmpty(m_RootNodeId) && !Nodes.Any())
            {
                var createdRoot = new SequenceRootNode();
                m_RootNodeId = createdRoot.Id;
                AddNode(createdRoot);
            }
            
            return TryGetNodeById(m_RootNodeId, out root);
        }
    }
}
