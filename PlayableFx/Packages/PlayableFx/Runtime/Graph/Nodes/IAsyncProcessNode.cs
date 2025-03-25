using Cysharp.Threading.Tasks;

namespace PlayableFx
{
    public interface IAsyncProcessNode
    {
        UniTask ProcessAsync();
    }
}