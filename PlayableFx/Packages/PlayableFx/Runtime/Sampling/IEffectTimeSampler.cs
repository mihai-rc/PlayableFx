namespace PlayableFx
{
    public interface IEffectTimeSampler<out T> : ITimeSampler
        where T : IPlayableEffect
    {
        T Effect { get; }
    }
}