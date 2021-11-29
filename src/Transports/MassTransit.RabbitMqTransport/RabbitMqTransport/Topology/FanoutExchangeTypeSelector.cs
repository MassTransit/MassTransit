namespace MassTransit.RabbitMqTransport.Topology
{
    using RabbitMQ.Client;


    public class FanoutExchangeTypeSelector :
        IExchangeTypeSelector
    {
        string IExchangeTypeSelector.GetExchangeType<T>(string exchangeName)
        {
            return ExchangeType.Fanout;
        }

        public string DefaultExchangeType => ExchangeType.Fanout;
    }
}
