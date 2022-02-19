namespace MassTransit
{
    using Transports.Fabric;


    public interface IGrpcConsumeTopologyConfigurator :
        IConsumeTopologyConfigurator,
        IGrpcConsumeTopology
    {
        new IGrpcMessageConsumeTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;

        void AddSpecification(IGrpcConsumeTopologySpecification specification);

        void Bind(string exchangeName, ExchangeType exchangeType = ExchangeType.FanOut, string routingKey = default);
    }
}
