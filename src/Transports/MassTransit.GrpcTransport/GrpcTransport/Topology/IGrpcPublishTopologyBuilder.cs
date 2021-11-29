namespace MassTransit.GrpcTransport.Topology
{
    using Contracts;


    /// <summary>
    /// A builder for creating the topology when publishing a message
    /// </summary>
    public interface IGrpcPublishTopologyBuilder :
        IGrpcTopologyBuilder
    {
        string ExchangeName { get; set; }
        ExchangeType ExchangeType { get; set; }

        IGrpcPublishTopologyBuilder CreateImplementedBuilder();
    }
}
