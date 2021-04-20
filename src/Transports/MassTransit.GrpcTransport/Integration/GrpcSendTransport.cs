namespace MassTransit.GrpcTransport.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using Contracts;
    using Fabric;
    using Google.Protobuf;
    using Google.Protobuf.WellKnownTypes;
    using GreenPipes;
    using Initializers.TypeConverters;
    using Logging;
    using Metadata;
    using Transports;


    /// <summary>
    /// Support in-memory message queue that is not durable, but supports parallel delivery of messages based on TPL usage.
    /// </summary>
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

            StartedActivity? activity = LogContext.IfEnabled(OperationName.Transport.Send)?.StartSendActivity(context);
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
                            RoutingKey = context.RoutingKey.ToNullableString()
                        },
                        Envelope = new Envelope
                        {
                            MessageId = messageId.ToUuid(),
                            RequestId = context.RequestId.ToUuid(),
                            ConversationId = context.ConversationId.ToUuid(),
                            CorrelationId = context.CorrelationId.ToUuid(),
                            InitiatorId = context.InitiatorId.ToUuid(),
                            SourceAddress = context.SourceAddress.ToNullableString(),
                            DestinationAddress = context.DestinationAddress.ToNullableString(),
                            ResponseAddress = context.ResponseAddress.ToNullableString(),
                            FaultAddress = context.FaultAddress.ToNullableString(),
                            ContentType = context.ContentType.MediaType,
                            Body = ByteString.CopyFrom(context.Body),
                            EnqueueTime = context.Delay.ToFutureDateTime(),
                            ExpirationTime = context.TimeToLive.ToFutureDateTime(),
                            SentTime = Timestamp.FromDateTime(context.SentTime ?? DateTime.UtcNow),
                        }
                    }
                };

                transportMessage.Deliver.Envelope.MessageType.AddRange(TypeMetadataCache<T>.MessageTypeNames);

                SetHeaders(transportMessage.Deliver.Envelope.Headers, context.Headers);

                var grpcTransportMessage = new GrpcTransportMessage(transportMessage, HostMetadataCache.Host);

                await _context.Exchange.Send(grpcTransportMessage, cancellationToken).ConfigureAwait(false);

                context.LogSent();
                activity.AddSendContextHeadersPostSend(context);

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
            foreach (var (key, headerValue) in headers.GetAll())
            {
                if (headerValue == null)
                {
                    if (dictionary.ContainsKey(key))
                        dictionary.Remove(key);

                    continue;
                }

                if (dictionary.ContainsKey(key))
                    continue;

                switch (headerValue)
                {
                    case DateTimeOffset value:
                        if (_dateTimeOffsetConverter.TryConvert(value, out string text))
                            dictionary[key] = text;
                        break;

                    case DateTime value:
                        if (_dateTimeConverter.TryConvert(value, out text))
                            dictionary[key] = text;
                        break;

                    case string value:
                        dictionary[key] = value;
                        break;

                    case bool value when value:
                        dictionary[key] = bool.TrueString;
                        break;

                    case IFormattable formatValue:
                        dictionary[key] = formatValue.ToString();
                        break;
                }
            }
        }
    }
}
