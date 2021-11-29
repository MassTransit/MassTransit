namespace MassTransit
{
    using AzureServiceBusTransport.Topology;


    public interface IServiceBusConsumeTopology :
        IConsumeTopology
    {
        new IServiceBusMessageConsumeTopology<T> GetMessageTopology<T>()
            where T : class;

        /// <summary>
        /// Apply the entire topology to the builder
        /// </summary>
        /// <param name="builder"></param>
        void Apply(IReceiveEndpointBrokerTopologyBuilder builder);
    }
}
