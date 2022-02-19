namespace MassTransit.Configuration
{
    using Transports.Fabric;


    public interface IMessageFabricTopologyBuilder
    {
        void ExchangeBind(string source, string destination, string routingKey);

        void QueueBind(string source, string destination);

        void ExchangeDeclare(string name, ExchangeType exchangeType);

        void QueueDeclare(string name);
    }
}
