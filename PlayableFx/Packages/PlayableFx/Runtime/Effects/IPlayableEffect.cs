using System.Threading;
using Cysharp.Threading.Tasks;

namespace PlayableFx
{
    public interface IPlayableEffect
    {
        UniTask PlayAsync(CancellationToken cancellation);
    }
}