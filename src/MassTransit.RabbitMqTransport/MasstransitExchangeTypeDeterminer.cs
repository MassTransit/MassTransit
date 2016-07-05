namespace MassTransit.RabbitMqTransport
{
    /// <summary>
    /// The default class for determining the type of an exchange
    /// For the Masstransit routing logic only fanout exchanges need to be made
    /// </summary>
    public class MasstransitExchangeTypeDeterminer: IExchangeTypeDeterminer
    {
        public string GetTypeForExchangeName(string exchangeName)
        {
            return RabbitMQ.Client.ExchangeType.Fanout;
        }
    }
}