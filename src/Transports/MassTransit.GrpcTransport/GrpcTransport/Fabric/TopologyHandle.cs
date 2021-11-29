namespace MassTransit.GrpcTransport.Fabric
{
    public interface TopologyHandle
    {
        long Id { get; }

        void Disconnect();
    }
}
