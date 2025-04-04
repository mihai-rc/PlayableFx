using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Pool;

namespace PlayableFx
{
    /// <summary>
    /// The tween track mixer.
    /// </summary>
    public class TweenTrackMixer : PlayableBehaviour
    {
        private const string k_NullTransformOnInitError = "[TweenTrackMixer] Can't initialize because no Transform was provided.";
        private const string k_NullTransformOnStartError = "[TweenTrackMixer] Can't initialize because no Transform was provided.";
        private const string k_NullTransformOnTickError = "[TweenTrackMixer] Can't play tween because no Transform was provided.";
        
        private Transform m_Transform;
        private Vector3 m_DefaultPosition;
        private Vector3 m_DefaultRotation;
        private Vector3 m_DefaultScale;
        
        /// <summary>
        /// Initializes the mixer.
        /// </summary>
        /// <param name="transform"> The <see cref="Transform"/> the tweens are applied to. </param>
        public void Init(Transform transform)
        {
            if (transform is null)
            {
                Debug.LogError(k_NullTransformOnInitError);
                return;
            }
            
            m_Transform = transform;
            m_DefaultPosition = transform.localPosition;
            m_DefaultRotation = transform.localEulerAngles;
            m_DefaultScale = transform.localScale;
        }

        public override void OnGraphStart(Playable playable)
        {
            if (m_Transform is null)
            {
                Debug.LogError(k_NullTransformOnStartError);
                return;
            }
            
            m_Transform.localPosition = m_DefaultPosition;
            m_Transform.localEulerAngles = m_DefaultRotation;
            m_Transform.localScale = m_DefaultScale;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (m_Transform is null)
            {
                Debug.LogError(k_NullTransformOnTickError);
                return;
            }
            
            var playingInputs = ListPool<(TweenBehavior behavior, float weight)>.Get();
            var inputCount = playable.GetInputCount();
            for (var i = 0; i < inputCount; i++)
            {
                var inputPlayable = (ScriptPlayable<TweenBehavior>)playable.GetInput(i);
                var inputWeight = playable.GetInputWeight(i);
                
                if (inputWeight == 0f)
                    continue;
                
                var inputBehavior = inputPlayable.GetBehaviour();
                playingInputs.Add((inputBehavior, inputWeight));
            }
            
            var playingInputsCount = playingInputs.Count;
            switch (playingInputsCount)
            {
                case 1:
                {
                    var playingInput = playingInputs.First();
                    playingInput.behavior.Tween.ApplyTo(m_Transform);
                    
                    break;
                }
                
                case 2:
                {
                    var firstInput = playingInputs[0];
                    var secondInput = playingInputs[1];
                    
                    firstInput.behavior.Tween
                        .BlendWith(secondInput.behavior.Tween, secondInput.weight)
                        .ApplyTo(m_Transform);
                    
                    break;
                }
            }
            
            ListPool<(TweenBehavior behavior, float weight)>.Release(playingInputs);
        }
    }
}