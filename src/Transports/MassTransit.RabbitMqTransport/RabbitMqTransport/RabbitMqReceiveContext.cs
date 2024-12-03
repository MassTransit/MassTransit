namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Collections.Generic;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using Context;
    using RabbitMQ.Client;
    using Serialization;
    using Transports;


    public sealed class RabbitMqReceiveContext :
        BaseReceiveContext,
        RabbitMqBasicConsumeContext,
        TransportReceiveContext,
        ITransportSequenceNumber
    {
        public RabbitMqReceiveContext(string exchange, string routingKey, string consumerTag, ulong deliveryTag, ReadOnlyMemory<byte> body,
            bool redelivered, IReadOnlyBasicProperties properties, RabbitMqReceiveEndpointContext receiveEndpointContext, params object[] payloads)
            : base(redelivered, receiveEndpointContext, payloads)
        {
            Exchange = exchange;
            RoutingKey = routingKey;
            ConsumerTag = consumerTag;
            DeliveryTag = deliveryTag;
            Properties = properties;

            Body = new MemoryMessageBody(body);
        }

        protected override IHeaderProvider HeaderProvider => new RabbitMqHeaderProvider(this);

        public override MessageBody Body { get; }

        public ulong? SequenceNumber => DeliveryTag;

        public string ConsumerTag { get; }
        public ulong DeliveryTag { get; }
        public string Exchange { get; }
        public string RoutingKey { get; }
        public IReadOnlyBasicProperties Properties { get; }

        public IDictionary<string, object> GetTransportProperties()
        {
            var properties = new Lazy<Dictionary<string, object>>(() => new Dictionary<string, object>());

            if (!string.IsNullOrWhiteSpace(RoutingKey))
                properties.Value[RabbitMqTransportPropertyNames.RoutingKey] = RoutingKey;

            if (Properties.IsAppIdPresent())
                properties.Value[RabbitMqTransportPropertyNames.AppId] = Properties.AppId;
            if (Properties.IsPriorityPresent())
                properties.Value[RabbitMqTransportPropertyNames.Priority] = Properties.Priority;
            if (Properties.IsReplyToPresent())
                properties.Value[RabbitMqTransportPropertyNames.ReplyTo] = Properties.ReplyTo;
            if (Properties.IsTypePresent())
                properties.Value[RabbitMqTransportPropertyNames.Type] = Properties.Type;
            if (Properties.IsUserIdPresent())
                properties.Value[RabbitMqTransportPropertyNames.UserId] = Properties.UserId;

            return properties.IsValueCreated ? properties.Value : null;
        }

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

                return address.IsReplyToAddress()
                    ? new ReplyToSendEndpoint(endpoint, _replyTo)
                    : endpoint;
            }
        }
    }
}
