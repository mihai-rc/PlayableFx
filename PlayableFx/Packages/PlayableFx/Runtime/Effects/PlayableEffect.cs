using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PlayableFx
{
    public abstract class PlayableEffect : MonoBehaviour, IPlayableEffect
    {
        public abstract UniTask PlayAsync(CancellationToken cancellation);
    }
}