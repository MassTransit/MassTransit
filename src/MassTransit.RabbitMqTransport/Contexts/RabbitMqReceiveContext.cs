namespace MassTransit.RabbitMqTransport.Contexts
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using RabbitMQ.Client;


    public sealed class RabbitMqReceiveContext :
        BaseReceiveContext,
        RabbitMqBasicConsumeContext
    {
        readonly byte[] _body;
        readonly Lazy<ISendEndpointProvider> _sendEndpointProvider;

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

            _sendEndpointProvider = new Lazy<ISendEndpointProvider>(CreateSendEndpointProvider);
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

        protected override ISendEndpointProvider GetSendEndpointProvider()
        {
            return _sendEndpointProvider.Value;
        }

        ISendEndpointProvider CreateSendEndpointProvider()
        {
            var provider = base.GetSendEndpointProvider();

            return new ReceiveSendEndpointProvider(provider, this);
        }


        class ReceiveSendEndpointProvider :
            ISendEndpointProvider
        {
            readonly ISendEndpointProvider _sendEndpointProvider;
            readonly RabbitMqReceiveContext _context;

            public ReceiveSendEndpointProvider(ISendEndpointProvider sendEndpointProvider, RabbitMqReceiveContext context)
            {
                _sendEndpointProvider = sendEndpointProvider;
                _context = context;
            }

            public ConnectHandle ConnectSendObserver(ISendObserver observer)
            {
                return _sendEndpointProvider.ConnectSendObserver(observer);
            }

            public async Task<ISendEndpoint> GetSendEndpoint(Uri address)
            {
                var endpoint = await _sendEndpointProvider.GetSendEndpoint(address).ConfigureAwait(false);

                if ((address.AbsolutePath?.EndsWith(RabbitMqExchangeNames.ReplyTo) ?? false) && _context.Properties.IsReplyToPresent())
                {
                    return new ReplyToSendEndpoint(endpoint, _context.Properties.ReplyTo);
                }

                return endpoint;
            }
        }
    }
}
