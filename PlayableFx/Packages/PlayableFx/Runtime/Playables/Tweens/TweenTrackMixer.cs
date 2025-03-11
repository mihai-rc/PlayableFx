using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Pool;

namespace PlayableFx
{
    public class TweenTrackMixer : PlayableBehaviour
    {
        private const string k_NullTransformError = "[TweenTrackMixer] Can't play tween because no Transform was provided.";

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (playerData is not Transform transform)
            {
                Debug.LogError(k_NullTransformError);
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
                
                    Debug.Log(playingInput.behavior.Tween.Position);
                    transform.localPosition = playingInput.behavior.Tween.Position;
                    transform.localEulerAngles = playingInput.behavior.Tween.Rotation;
                    transform.localScale = playingInput.behavior.Tween.Scale;
                
                    break;
                }
                case 2:
                {
                    var firstInput = playingInputs[0];
                    var secondInput = playingInputs[1];
                    
                    transform.localPosition = Vector3.Lerp(firstInput.behavior.Tween.Position, secondInput.behavior.Tween.Position, firstInput.weight);
                    transform.localEulerAngles = Vector3.Lerp(firstInput.behavior.Tween.Rotation, secondInput.behavior.Tween.Rotation, firstInput.weight);
                    transform.localScale = Vector3.Lerp(firstInput.behavior.Tween.Scale, secondInput.behavior.Tween.Scale, firstInput.weight);
                    
                    break;
                }
            }
            
            ListPool<(TweenBehavior behavior, float weight)>.Release(playingInputs);
        }
    }
}