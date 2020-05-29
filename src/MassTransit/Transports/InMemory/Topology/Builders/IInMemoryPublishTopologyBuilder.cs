namespace MassTransit.Transports.InMemory.Topology.Builders
{
    /// <summary>
    /// A builder for creating the topology when publishing a message
    /// </summary>
    public interface IInMemoryPublishTopologyBuilder :
        IInMemoryTopologyBuilder
    {
        string ExchangeName { get; set; }

        IInMemoryPublishTopologyBuilder CreateImplementedBuilder();
    }
}
