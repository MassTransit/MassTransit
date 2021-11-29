namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using RabbitMQ.Client;
    using Transports;


    public sealed class RabbitMqReceiveContext :
        BaseReceiveContext,
        RabbitMqBasicConsumeContext
    {
        public RabbitMqReceiveContext(string exchange, string routingKey, string consumerTag, ulong deliveryTag, byte[] body,
            bool redelivered, IBasicProperties properties, RabbitMqReceiveEndpointContext receiveEndpointContext, params object[] payloads)
            : base(redelivered, receiveEndpointContext, payloads)
        {
            Exchange = exchange;
            RoutingKey = routingKey;
            ConsumerTag = consumerTag;
            DeliveryTag = deliveryTag;
            Properties = properties;

            Body = new BytesMessageBody(body);
        }

        protected override IHeaderProvider HeaderProvider => new RabbitMqHeaderProvider(this);

        public override MessageBody Body { get; }

        public string ConsumerTag { get; }
        public ulong DeliveryTag { get; }
        public string Exchange { get; }
        public string RoutingKey { get; }
        public IBasicProperties Properties { get; }

        byte[] RabbitMqBasicConsumeContext.Body => Body.GetBytes();

        protected override ContentType GetContentType()
        {
            ContentType contentType = default;
            if (!string.IsNullOrWhiteSpace(Properties.ContentType))
                contentType = ConvertToContentType(Properties.ContentType);

            return contentType ?? base.GetContentType();
        }

        protected override ISendEndpointProvider GetSendEndpointProvider()
        {
            var provider = base.GetSendEndpointProvider();

            return Properties.IsReplyToPresent() && !string.IsNullOrWhiteSpace(Properties.ReplyTo)
                ? new ReceiveSendEndpointProvider(provider, Properties.ReplyTo)
                : provider;
        }


        class ReceiveSendEndpointProvider :
            ISendEndpointProvider
        {
            readonly string _replyTo;
            readonly ISendEndpointProvider _sendEndpointProvider;

            public ReceiveSendEndpointProvider(ISendEndpointProvider sendEndpointProvider, string replyTo)
            {
                _replyTo = replyTo;

                _sendEndpointProvider = sendEndpointProvider;
            }

            public ConnectHandle ConnectSendObserver(ISendObserver observer)
            {
                return _sendEndpointProvider.ConnectSendObserver(observer);
            }

            public async Task<ISendEndpoint> GetSendEndpoint(Uri address)
            {
                var endpoint = await _sendEndpointProvider.GetSendEndpoint(address).ConfigureAwait(false);

                return new ReplyToSendEndpoint(endpoint, _replyTo);
            }
        }
    }
}
