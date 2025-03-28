using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace PlayableFx
{
    public abstract class PlayableEffect : MonoBehaviour, IPlayableEffect
    {
        public abstract UniTask PlayAsync(CancellationToken cancellation);
    }
}