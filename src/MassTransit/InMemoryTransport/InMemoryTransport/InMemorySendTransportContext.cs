namespace MassTransit.InMemoryTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Initializers.TypeConverters;
    using MassTransit.Configuration;
    using Middleware;
    using Transports;
    using Transports.Fabric;


    public class InMemorySendTransportContext :
        BaseSendTransportContext,
        SendTransportContext<PipeContext>
    {
        static readonly DateTimeOffsetTypeConverter _dateTimeOffsetConverter = new DateTimeOffsetTypeConverter();
        static readonly DateTimeTypeConverter _dateTimeConverter = new DateTimeTypeConverter();
        readonly IMessageExchange<InMemoryTransportMessage> _exchange;

        public InMemorySendTransportContext(IHostConfiguration hostConfiguration, ReceiveEndpointContext context,
            IMessageExchange<InMemoryTransportMessage> exchange)
            : base(hostConfiguration, context.Serialization)
        {
            _exchange = exchange;
        }

        public override string EntityName => _exchange.Name;
        public override string ActivitySystem => "in-memory";

        public override async Task<SendContext<T>> CreateSendContext<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            var sendContext = new InMemorySendContext<T>(message, cancellationToken);

            await pipe.Send(sendContext).ConfigureAwait(false);

            return sendContext;
        }

        public Task<SendContext<T>> CreateSendContext<T>(PipeContext context, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            return CreateSendContext(message, pipe, cancellationToken);
        }

        public Task Send<T>(PipeContext transportContext, SendContext<T> sendContext)
            where T : class
        {
            InMemorySendContext<T> context = sendContext as InMemorySendContext<T>
                ?? throw new ArgumentException("Invalid SendContext<T> type", nameof(sendContext));

            sendContext.CancellationToken.ThrowIfCancellationRequested();

            var messageId = context.MessageId ?? NewId.NextGuid();

            var transportMessage = new InMemoryTransportMessage(messageId, context.Body.GetBytes(), context.ContentType.ToString())
            {
                Delay = context.Delay,
                RoutingKey = context.RoutingKey
            };

            SetHeaders(transportMessage.Headers, context.Headers);

            var deliveryContext = new InMemoryDeliveryContext(transportMessage, context.CancellationToken);

            return _exchange.Deliver(deliveryContext);
        }

        public Task Send(IPipe<PipeContext> pipe, CancellationToken cancellationToken = default)
        {
            var pipeContext = new Context(cancellationToken);

            return pipe.Send(pipeContext);
        }

        public void Probe(ProbeContext context)
        {
            _exchange.Probe(context);
        }

        static void SetHeaders(SendHeaders sendHeaders, Headers headers)
        {
            foreach (KeyValuePair<string, object> header in headers.GetAll())
            {
                if (header.Value == null)
                {
                    sendHeaders.Set(header.Key, null);

                    continue;
                }

                if (sendHeaders.TryGetHeader(header.Key, out _))
                    continue;

                switch (header.Value)
                {
                    case DateTimeOffset value:
                        if (_dateTimeOffsetConverter.TryConvert(value, out string text))
                            sendHeaders.Set(header.Key, text);
                        break;

                    case DateTime value:
                        if (_dateTimeConverter.TryConvert(value, out text))
                            sendHeaders.Set(header.Key, text);
                        break;

                    case Uri value:
                        sendHeaders.Set(header.Key, value.ToString());
                        break;

                    case string value:
                        sendHeaders.Set(header.Key, value);
                        break;

                    case bool value when value:
                        sendHeaders.Set(header.Key, bool.TrueString);
                        break;

                    case IFormattable formatValue:
                        if (header.Value.GetType().IsValueType)
                            sendHeaders.Set(header.Key, header.Value);
                        else
                            sendHeaders.Set(header.Key, formatValue.ToString());
                        break;
                }
            }
        }


        class Context :
            BasePipeContext
        {
            public Context(CancellationToken cancellationToken)
                : base(cancellationToken)
            {
            }
        }
    }
}
