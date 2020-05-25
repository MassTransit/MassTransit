namespace MassTransit.Contracts.Conductor
{
    /// <summary>
    /// Sent to call connected clients when an instance goes down
    /// </summary>
    public interface InstanceDown
    {
        InstanceInfo Instance { get; }

        ServiceInfo Service { get; }
    }
}
