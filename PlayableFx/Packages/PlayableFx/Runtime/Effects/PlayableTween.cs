using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace PlayableFx
{
    public class PlayableTween : PlayableEffect
    {
        private const string k_TransformNotAssigned = "[PlayableTween] Cannot play tween: {0} because no Transform was assigned!";
        private const string k_DurationNotSet = "[PlayableTween] Cannot play tween: {0} because no Duration was set!";
        
        [field: SerializeField] public Transform Transform { get; set; }
        [field: SerializeField] public float Duration { get; set; }
        [field: SerializeField] public TweenSettings Settings { get; set; }
        
        private TransformTween m_Tween;
        private float m_CurrentTime;

        private void Awake() => m_Tween = new TransformTween();

        private void Start()
        {
            if (Transform == null)
            {
                Debug.LogErrorFormat(k_TransformNotAssigned, name);
                return;
            }
            
            if (Duration <= 0f)
            {
                Debug.LogErrorFormat(k_DurationNotSet, name);
                return;
            }

            m_Tween.SetDuration(Duration)
                .SetStartingValues(Transform.localPosition, Transform.localEulerAngles, Transform.localScale)
                .SetSettings(Settings);
        }

        public override async UniTask PlayAsync(CancellationToken cancellation)
        {
            m_CurrentTime = 0f;
            
            if (Transform == null)
            {
                Debug.LogErrorFormat(k_TransformNotAssigned, name);
                return;
            }
            
            if (Duration <= 0f)
            {
                Debug.LogErrorFormat(k_DurationNotSet, name);
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