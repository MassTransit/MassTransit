namespace MassTransit.ServiceBus.HealthMonitoring
{
    using Messages;

    public interface IHeartbeatTimer
    {
        void Add(Heartbeat message);
        void Remove(Heartbeat message);
    }
}