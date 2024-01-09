namespace MassTransit.SqlTransport.Configuration
{
    using MassTransit.Configuration;


    public interface ISqlEndpointConfiguration :
        IEndpointConfiguration
    {
        new ISqlTopologyConfiguration Topology { get; }
    }
}
