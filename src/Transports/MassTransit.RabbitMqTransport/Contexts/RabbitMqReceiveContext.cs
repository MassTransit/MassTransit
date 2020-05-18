namespace MassTransit.RabbitMqTransport.Contexts
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using RabbitMQ.Client;


    public sealed class RabbitMqReceiveContext :
        BaseReceiveContext,
        RabbitMqBasicConsumeContext
    {
        readonly byte[] _body;

        public RabbitMqReceiveContext(string exchange, string routingKey, string consumerTag, ulong deliveryTag, byte[] body,
            bool redelivered, IBasicProperties properties, RabbitMqReceiveEndpointContext receiveEndpointContext, params object[] payloads)
            : base(redelivered, receiveEndpointContext, payloads)
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

        protected override ContentType GetContentType()
        {
            return !string.IsNullOrWhiteSpace(Properties.ContentType) ? new ContentType(Properties.ContentType) : base.GetContentType();
        }

        protected override ISendEndpointProvider GetSendEndpointProvider()
        {
            var provider = base.GetSendEndpointProvider();

            return Properties.IsReplyToPresent()
                ? new ReceiveSendEndpointProvider(provider, Properties.ReplyTo)
                : provider;
        }


        class ReceiveSendEndpointProvider :
            ISendEndpointProvider
        {
            readonly ISendEndpointProvider _sendEndpointProvider;
            readonly string _replyTo;

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
