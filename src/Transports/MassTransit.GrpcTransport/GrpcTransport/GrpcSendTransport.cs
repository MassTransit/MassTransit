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
    using Logging;
    using Metadata;
    using Transports;


    public class GrpcSendTransport :
        ISendTransport
    {
        static readonly DateTimeOffsetTypeConverter _dateTimeOffsetConverter = new DateTimeOffsetTypeConverter();
        static readonly DateTimeTypeConverter _dateTimeConverter = new DateTimeTypeConverter();
        readonly GrpcSendTransportContext _context;

        public GrpcSendTransport(GrpcSendTransportContext context)
        {
            _context = context;
        }

        async Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            var context = new TransportGrpcSendContext<T>(_context.Exchange.Name, message, cancellationToken);

            await pipe.Send(context).ConfigureAwait(false);

            StartedActivity? activity = LogContext.Current?.StartSendActivity(_context, context);
            try
            {
                if (_context.SendObservers.Count > 0)
                    await _context.SendObservers.PreSend(context).ConfigureAwait(false);

                var messageId = context.MessageId ?? NewId.NextGuid();

                var transportMessage = new TransportMessage
                {
                    Deliver = new Deliver
                    {
                        Exchange = new ExchangeDestination
                        {
                            Name = _context.Exchange.Name,
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

                await _context.Exchange.Send(grpcTransportMessage, cancellationToken).ConfigureAwait(false);

                activity?.Update(context);
                context.LogSent();

                if (_context.SendObservers.Count > 0)
                    await _context.SendObservers.PostSend(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                context.LogFaulted(ex);

                if (_context.SendObservers.Count > 0)
                    await _context.SendObservers.SendFault(context, ex).ConfigureAwait(false);

                throw;
            }
            finally
            {
                activity?.Stop();
            }
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _context.ConnectSendObserver(observer);
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
    }
}
