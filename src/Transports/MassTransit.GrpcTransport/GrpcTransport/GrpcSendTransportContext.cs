namespace MassTransit.GrpcTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using Fabric;
    using Google.Protobuf;
    using Google.Protobuf.WellKnownTypes;
    using Initializers.TypeConverters;
    using MassTransit.Configuration;
    using MassTransit.Middleware;
    using Metadata;
    using Transports;
    using Transports.Fabric;


    public class GrpcSendTransportContext :
        BaseSendTransportContext,
        SendTransportContext<PipeContext>
    {
        static readonly DateTimeOffsetTypeConverter _dateTimeOffsetConverter = new DateTimeOffsetTypeConverter();
        static readonly DateTimeTypeConverter _dateTimeConverter = new DateTimeTypeConverter();

        readonly IMessageExchange<GrpcTransportMessage> _exchange;

        public GrpcSendTransportContext(IHostConfiguration hostConfiguration, ReceiveEndpointContext receiveEndpointContext,
            IMessageExchange<GrpcTransportMessage> exchange)
            : base(hostConfiguration, receiveEndpointContext.Serialization)
        {
            _exchange = exchange;
        }

        public override string EntityName => _exchange.Name;
        public override string ActivitySystem => "grpc";

        public override async Task<SendContext<T>> CreateSendContext<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            var sendContext = new TransportGrpcSendContext<T>(_exchange.Name, message, cancellationToken);

            await pipe.Send(sendContext).ConfigureAwait(false);

            return sendContext;
        }

        public void Probe(ProbeContext context)
        {
        }

        public Task<SendContext<T>> CreateSendContext<T>(PipeContext context, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            return CreateSendContext(message, pipe, cancellationToken);
        }

        public async Task Send<T>(PipeContext transportContext, SendContext<T> sendContext)
            where T : class
        {
            TransportGrpcSendContext<T> context = sendContext as TransportGrpcSendContext<T>
                ?? throw new ArgumentException("Invalid SendContext<T> type", nameof(sendContext));

            sendContext.CancellationToken.ThrowIfCancellationRequested();

            var messageId = context.MessageId ?? NewId.NextGuid();

            var transportMessage = new TransportMessage
            {
                Deliver = new Deliver
                {
                    Exchange = new ExchangeDestination
                    {
                        Name = _exchange.Name,
                        RoutingKey = context.RoutingKey ?? ""
                    },
                    Envelope = new Envelope
                    {
                        MessageId = messageId.ToString("D"),
                        RequestId = context.RequestId?.ToString("D") ?? "",
                        ConversationId = context.ConversationId?.ToString("D") ?? "",
                        CorrelationId = context.CorrelationId?.ToString("D") ?? "",
                        InitiatorId = context.InitiatorId?.ToString("D") ?? "",
                        SourceAddress = context.SourceAddress?.ToString() ?? "",
                        DestinationAddress = context.DestinationAddress?.ToString() ?? "",
                        ResponseAddress = context.ResponseAddress?.ToString() ?? "",
                        FaultAddress = context.FaultAddress?.ToString() ?? "",
                        ContentType = context.ContentType?.ToString() ?? "",
                        Body = ByteString.CopyFrom(context.Body.GetBytes()),
                        EnqueueTime = context.Delay.ToFutureDateTime(),
                        ExpirationTime = context.TimeToLive.ToFutureDateTime(),
                        SentTime = Timestamp.FromDateTime(context.SentTime ?? DateTime.UtcNow),
                    }
                }
            };

            transportMessage.Deliver.Envelope.MessageType.AddRange(MessageTypeCache<T>.MessageTypeNames);

            SetHeaders(transportMessage.Deliver.Envelope.Headers, context.Headers);

            var grpcTransportMessage = new GrpcTransportMessage(transportMessage, HostMetadataCache.Host);

            await _exchange.Send(grpcTransportMessage, context.CancellationToken).ConfigureAwait(false);
        }

        public Task Send(IPipe<PipeContext> pipe, CancellationToken cancellationToken = default)
        {
            var pipeContext = new Context(cancellationToken);

            return pipe.Send(pipeContext);
        }

        static void SetHeaders(IDictionary<string, string> dictionary, SendHeaders headers)
        {
            foreach (KeyValuePair<string, object> header in headers.GetAll())
            {
                if (header.Value == null)
                {
                    if (dictionary.ContainsKey(header.Key))
                        dictionary.Remove(header.Key);

                    continue;
                }

                if (dictionary.ContainsKey(header.Key))
                    continue;

                switch (header.Value)
                {
                    case DateTimeOffset value:
                        if (_dateTimeOffsetConverter.TryConvert(value, out string text))
                            dictionary[header.Key] = text;
                        break;

                    case DateTime value:
                        if (_dateTimeConverter.TryConvert(value, out text))
                            dictionary[header.Key] = text;
                        break;

                    case string value:
                        dictionary[header.Key] = value;
                        break;

                    case bool value when value:
                        dictionary[header.Key] = bool.TrueString;
                        break;

                    case IFormattable formatValue:
                        dictionary[header.Key] = formatValue.ToString();
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
