namespace MassTransit.Riders
{
    public interface RiderReady
    {
        string Name { get; }
        bool IsStarted { get; }
    }
}
