namespace MassTransit.Transports.InMemory.Configuration
{
    using MassTransit.Configuration;


    public interface IInMemoryEndpointConfiguration :
        IEndpointConfiguration
    {
        new IInMemoryTopologyConfiguration Topology { get; }
    }
}
