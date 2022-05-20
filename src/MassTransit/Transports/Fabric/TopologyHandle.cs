namespace MassTransit.Transports.Fabric
{
    public interface TopologyHandle
    {
        long Id { get; }

        void Disconnect();
    }
}
