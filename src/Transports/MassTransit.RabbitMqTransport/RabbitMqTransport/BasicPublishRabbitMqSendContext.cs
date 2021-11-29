namespace MassTransit.RabbitMqTransport
{
    using System.Threading;
    using Context;
    using RabbitMQ.Client;


    public class BasicPublishRabbitMqSendContext<T> :
        MessageSendContext<T>,
        RabbitMqSendContext<T>
        where T : class
    {
        public BasicPublishRabbitMqSendContext(IBasicProperties basicProperties, string exchange, T message, CancellationToken cancellationToken)
            : base(message, cancellationToken)
        {
            BasicProperties = basicProperties;

            AwaitAck = true;

            RoutingKey = "";

            Exchange = exchange;
        }

        public string Exchange { get; }
        public string RoutingKey { get; set; }
        public IBasicProperties BasicProperties { get; }
        public bool AwaitAck { get; set; }
    }
}
