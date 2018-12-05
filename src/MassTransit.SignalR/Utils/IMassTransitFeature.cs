namespace MassTransit.SignalR.Utils
{
    public interface IMassTransitFeature
    {
        ConcurrentHashSet<string> Groups { get; }
    }
}
