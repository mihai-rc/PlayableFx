using UnityEngine;
using Cysharp.Threading.Tasks;
using GiftHorse.ScriptableGraphs;
using GiftHorse.ScriptableGraphs.Attributes;

namespace PlayableFx
{
    [NodeScript("Tweening"), HeaderColor(0.29f, 0.43f, 0.17f)]
    public class TweenNode : AsyncFlowNode
    {
        // [Input] public TweenConfig PositionConfig;
        // [Input] public TweenConfig RotationConfig;
        // [Input] public TweenConfig ScaleConfig;
        
        private const string k_TransformNotAssigned = "[SequenceGraph] [TweenNode] Cannot process tween with Id: {0} because no Transform was assigned! Graph name: {1}";
        private const string k_DurationNotSet = "[SequenceGraph] [TweenNode] Cannot process tween with Id: {0} because no Duration was set! Graph name: {1}";
        
        [NodeField] public Transform Transform;
        [NodeField] public float Duration;
        [NodeField] public TweenSettings Settings;
        
        private TransformTween m_Tween;
        private float m_CurrentTime;

        protected override void OnCreate(ScriptableGraph graph)
        {
            m_Tween = new TransformTween();
            base.OnCreate(graph);
        }

        protected override void OnProcess(ScriptableGraph graph)
        {
            base.OnProcess(graph);

            if (Transform == null)
            {
                Debug.LogErrorFormat(k_TransformNotAssigned, Id, graph.name);
                return;
            }
            
            if (Duration <= 0f)
            {
                Debug.LogErrorFormat(k_DurationNotSet, Id, graph.name);
                return;
            }

            m_Tween.SetDuration(Duration)
                .SetStartingValues(Transform.localPosition, Transform.localEulerAngles, Transform.localScale)
                .SetSettings(Settings);
        }

        protected override async UniTask OnProcessAsync()
        {
            m_CurrentTime = 0f;
            
            if (Transform == null)
            {
                // Debug.LogErrorFormat(k_TransformNotAssigned, Id, graph.name);
                return;
            }
            
            if (Duration <= 0f)
            {
                // Debug.LogErrorFormat(k_DurationNotSet, Id, graph.name);
                return;
            }
            
            while (m_CurrentTime < Duration)
            {
                m_CurrentTime += Time.deltaTime;
                
                if (m_CurrentTime > Duration)
                    m_CurrentTime = Duration;
                
                m_Tween.ComputeAt(m_CurrentTime).ApplyTo(Transform);
                
                await UniTask.Yield();
            }
        }
    }
}