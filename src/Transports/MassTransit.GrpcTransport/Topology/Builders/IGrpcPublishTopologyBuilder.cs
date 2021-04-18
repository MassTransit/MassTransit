namespace MassTransit.GrpcTransport.Topology.Builders
{
    /// <summary>
    /// A builder for creating the topology when publishing a message
    /// </summary>
    public interface IGrpcPublishTopologyBuilder :
        IGrpcTopologyBuilder
    {
        string ExchangeName { get; set; }

        IGrpcPublishTopologyBuilder CreateImplementedBuilder();
    }
}
