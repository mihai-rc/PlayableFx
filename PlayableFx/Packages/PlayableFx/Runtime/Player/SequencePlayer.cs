using System;
using UnityEngine;
using LitMotion;
using Cysharp.Threading.Tasks;

namespace PlayableFx
{
    public class SequencePlayer : ISequencePlayer
    {
        public event Action<SequenceState> OnStateChanged;
        
        public SequenceState State
        {
            get => m_State;
            private set
            {
                m_State = value;
                OnStateChanged?.Invoke(value);
            }
        }

        public float Progress
        {
            get => m_Progress;
            set
            {
                m_Progress = value;
                var time = ProgressToTime(value, m_Duration);
                m_TimeSampler.Time = time;
            }
        }
        
        public bool CanPlay => State is not SequenceState.Playing;
        public bool IsPlaying => State is SequenceState.Playing;
        public bool IsPaused => State is SequenceState.Paused;
        
        private const string k_TimeSamplerCannotBrNull = "[PlayableEffect] Cannot instanciate SequencePlayer: {0} with a null ITimeSampler";
        private const string k_CannotPlayWhilePlayingError = "[PlayableEffect] Cannot play Effect: {0}, it is not idle or paused.";
        private const string k_CannotPauseWhileNotPlayingError = "[PlayableEffect] Cannot pause Effect: {0}, it is not playing.";
        private const string k_InvalidProgressError = "[PlayableEffect] Progress must be a value between 0 and 1.";

        private readonly string m_Name;
        private SequenceState m_State;
        private float m_Duration;
        private float m_Progress;
        private ITimeSampler m_TimeSampler;
        private MotionHandle m_Tween;

        public SequencePlayer(string name, ITimeSampler timeSampler)
        {
            m_Name = name;
            m_State = SequenceState.Idle;
            m_TimeSampler = timeSampler;

            if (timeSampler is not null)
            {
                m_Duration = timeSampler.Duration;
            }
            else
            {
                ReportError(k_TimeSamplerCannotBrNull);
            }
        }

        public async UniTask PlayAsync()
        {
            if (!TrySetState(SequenceState.Playing))
            {
                return;
            }

            m_Tween = LMotion
                .Create(0f, 1f, m_Duration)
                .WithEase(Ease.Linear)
                .Bind(this, (progress, player) => player.Progress = progress);
            
            await m_Tween;

            TrySetState(SequenceState.Idle);
        }

        public void Pause()
        {
            if (!TrySetState(SequenceState.Paused))
            {
                return;
            }

            m_Tween.PlaybackSpeed = 0f;
        }

        public void Resume()
        {
            if (!TrySetState(SequenceState.Playing))
            {
                return;
            }

            m_Tween.PlaybackSpeed = 1f;
        }

        public void Stop()
        {
            if (!TrySetState(SequenceState.Idle))
            {
                return;
            }

            m_Tween.Cancel();
        }

        public void Revert()
        {
            Stop();
            Progress = 0f;
        }

        public void Complete()
        {
            Stop();
            Progress = 1f;
        }
        
        private bool TrySetState(SequenceState state)
        {
            return state switch
            {
                SequenceState.Playing => TrySetPlayState(),
                SequenceState.Paused => TrySetPauseState(),
                SequenceState.Idle => TrySetIdleState(),
                
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };
        }
        
        private bool TrySetPlayState()
        {
            if (!CanPlay)
            {
                ReportError(k_CannotPlayWhilePlayingError);
                return false;
            }

            State = SequenceState.Playing;
            return true;
        }
        
        private bool TrySetPauseState()
        {
            if (State is not SequenceState.Playing)
            {
                ReportError(k_CannotPauseWhileNotPlayingError);
                return false;
            }

            State = SequenceState.Paused;
            return true;
        }

        private bool TrySetIdleState()
        {
            if (State is SequenceState.Idle)
            {
                return false;
            }
            
            State = SequenceState.Idle;
            return true;
        }
        
        private static float TimeToProgress(float time, float duration)
        {
            if (time == 0f)
            {
                return 0f;
            }

            return time / duration;
        }
        
        private float ProgressToTime(float progress, float duration)
        {
            if (progress is < 0 or > 1)
            {
                ReportError(k_InvalidProgressError);
                return 0f;
            }

            return duration * progress;
        }
        
        private void ReportError(string message)
        {
            Debug.LogError(string.Format(message, m_Name));
        }
    }
}
