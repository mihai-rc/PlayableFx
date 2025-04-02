using System.Threading;
using Cysharp.Threading.Tasks;

namespace PlayableFx
{
    public interface IAsyncPlayable
    {
        UniTask PlayAsync(CancellationToken cancellation);
    }
}