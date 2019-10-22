namespace MassTransit.RabbitMqTransport.Contexts
{
    using System;
    using System.IO;
    using Context;
    using RabbitMQ.Client;


    public sealed class RabbitMqReceiveContext :
        BaseReceiveContext,
        RabbitMqBasicConsumeContext
    {
        readonly byte[] _body;

        public RabbitMqReceiveContext(Uri inputAddress, string exchange, string routingKey, string consumerTag, ulong deliveryTag, byte[] body,
            bool redelivered, IBasicProperties properties, RabbitMqReceiveEndpointContext receiveEndpointContext, params object[] payloads)
            : base(inputAddress, redelivered, receiveEndpointContext, payloads)
        {
            Exchange = exchange;
            RoutingKey = routingKey;
            ConsumerTag = consumerTag;
            DeliveryTag = deliveryTag;
            _body = body;
            Properties = properties;
        }

        protected override IHeaderProvider HeaderProvider => new RabbitMqHeaderProvider(this);

        public string ConsumerTag { get; }
        public ulong DeliveryTag { get; }
        public string Exchange { get; }
        public string RoutingKey { get; }
        public IBasicProperties Properties { get; }

        byte[] RabbitMqBasicConsumeContext.Body => _body;

        public override byte[] GetBody()
        {
            return _body;
        }

        public override Stream GetBodyStream()
        {
            return new MemoryStream(_body, false);
        }
    }
}
