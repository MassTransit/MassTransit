namespace MassTransit
{
    using AmazonSqsTransport.Topology;
    using Topology;


    public interface IAmazonSqsConsumeTopologyConfigurator :
        IConsumeTopologyConfigurator,
        IAmazonSqsConsumeTopology
    {
        new IAmazonSqsMessageConsumeTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;

        void AddSpecification(IAmazonSqsConsumeTopologySpecification specification);
    }
}
