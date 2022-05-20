#nullable enable
namespace MassTransit
{
    using InMemoryTransport.Configuration;
    using Transports.Fabric;


    public interface IInMemoryConsumeTopologyConfigurator :
        IConsumeTopologyConfigurator,
        IInMemoryConsumeTopology
    {
        new IInMemoryMessageConsumeTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;

        void AddSpecification(IInMemoryConsumeTopologySpecification specification);

        void Bind(string exchangeName, ExchangeType exchangeType = ExchangeType.FanOut, string? routingKey = default);
    }
}
