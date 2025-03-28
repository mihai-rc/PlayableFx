using System.Threading;
using Cysharp.Threading.Tasks;

namespace PlayableFx
{
    public interface IAsyncProcessNode
    {
        UniTask ProcessAsync(CancellationToken cancellation);
    }
}