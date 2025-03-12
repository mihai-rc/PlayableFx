using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Pool;

namespace PlayableFx
{
    public class TweenTrackMixer : PlayableBehaviour
    {
        private const string k_NullTransformOnInitError = "[TweenTrackMixer] Can't initialize because no Transform was provided.";
        private const string k_NullTransformOnStartError = "[TweenTrackMixer] Can't initialize because no Transform was provided.";
        private const string k_NullTransformOnTickError = "[TweenTrackMixer] Can't play tween because no Transform was provided.";
        
        private Transform m_Transform;
        private Vector3 m_DefaultPosition;
        private Vector3 m_DefaultRotation;
        private Vector3 m_DefaultScale;
        
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
                var inoutWeight = playable.GetInputWeight(i);
                
                if (inoutWeight == 0f)
                    continue;
                
                var inputBehavior = inputPlayable.GetBehaviour();
                playingInputs.Add((inputBehavior, inoutWeight));
            }
            
            Debug.Log(playingInputs.Count);
            var playingInputsCount = playingInputs.Count;
            switch (playingInputsCount)
            {
                case 1:
                {
                    var playingInput = playingInputs.First();
                    var tween = playingInput.behavior.Tween;
                    
                    if (tween.Settings.PositionConfig.Enabled)
                        m_Transform.localPosition = playingInput.behavior.Tween.Position;
                    
                    if (tween.Settings.RotationConfig.Enabled)
                        m_Transform.localEulerAngles = playingInput.behavior.Tween.Rotation;
                    
                    if (tween.Settings.ScaleConfig.Enabled)
                        m_Transform.localScale = playingInput.behavior.Tween.Scale;
                    
                    break;
                }
                case 2:
                {
                    var firstInput = playingInputs[0];
                    var firstWeight = firstInput.weight;
                    var (firstPosition, firstRotation, firstScale, firstSettings) = firstInput.behavior.Tween;
                    
                    var secondInput = playingInputs[1];
                    var secondWeight = secondInput.weight;
                    var (secondPosition, secondRotation, secondScale, secondSettings) = secondInput.behavior.Tween;
                    
                    m_Transform.localPosition = (firstSettings.PositionConfig.Enabled, secondSettings.PositionConfig.Enabled) switch
                    {
                        (true , true ) => Vector3.Lerp(firstPosition, secondPosition, secondWeight),
                        (true , false) => firstPosition,
                        (false, true ) => secondPosition,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    
                    m_Transform.localEulerAngles = (firstSettings.RotationConfig.Enabled, secondSettings.RotationConfig.Enabled) switch
                    {
                        (true , true ) => Vector3.Lerp(firstRotation, secondRotation, secondWeight),
                        (true , false) => firstRotation,
                        (false, true ) => secondRotation,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    
                    m_Transform.localScale = (firstSettings.ScaleConfig.Enabled, secondSettings.ScaleConfig.Enabled) switch
                    {
                        (true , true ) => Vector3.Lerp(firstScale, secondScale, secondWeight),
                        (true , false) => firstScale,
                        (false, true ) => secondScale,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    
                    break;
                }
            }
            
            ListPool<(TweenBehavior behavior, float weight)>.Release(playingInputs);
        }
    }
}