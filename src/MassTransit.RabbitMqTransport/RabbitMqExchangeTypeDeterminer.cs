namespace MassTransit.RabbitMqTransport
{
    public class RabbitMqExchangeTypeDeterminer: IExchangeTypeDeterminer
    {
        public string getTypeForExchangeName(string exchangeName)
        {
            return RabbitMQ.Client.ExchangeType.Fanout;
        }
    }
}