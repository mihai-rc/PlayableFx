namespace PlayableFx
{
    public interface ITimeSampler
    {
        float Duration { get; }
        
        float Time { get; set; }
    }
}