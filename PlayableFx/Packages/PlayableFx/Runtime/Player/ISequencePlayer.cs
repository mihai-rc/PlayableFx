using System;
using Cysharp.Threading.Tasks;

namespace PlayableFx
{
    public interface ISequencePlayer
    {
        event Action<SequenceState> OnStateChanged;
        
        SequenceState State { get; }
        
        bool CanPlay { get; }
        bool IsPlaying { get; }
        bool IsPaused { get; }
        
        //ISequenceNode SequenceRoot { get; }

        float Progress { get; set; }

        UniTask PlayAsync();

        void Pause();
        void Resume();
        void Stop();
        void Revert();
        void Complete();
    }
}