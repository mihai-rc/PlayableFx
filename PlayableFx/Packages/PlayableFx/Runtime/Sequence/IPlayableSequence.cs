namespace PlayableFx
{
    public interface IPlayableSequence
    {
        IPlayableEffect Effect { get; }
        ISequencePlayer Player { get; }
    }
}
