namespace MassTransit
{
    using SqlTransport.Topology;


    public interface ISqlConsumeTopology :
        IConsumeTopology
    {
        new ISqlMessageConsumeTopology<T> GetMessageTopology<T>()
            where T : class;

        /// <summary>
        /// Apply the entire topology to the builder
        /// </summary>
        /// <param name="builder"></param>
        void Apply(IReceiveEndpointBrokerTopologyBuilder builder);
    }
}
