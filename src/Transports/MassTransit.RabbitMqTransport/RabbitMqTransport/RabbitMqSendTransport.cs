namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Initializers.TypeConverters;
    using Internals;
    using Logging;
    using MassTransit.Middleware;
    using RabbitMQ.Client;
    using Transports;


    public class RabbitMqSendTransport :
        Supervisor,
        ISendTransport,
        IAsyncDisposable
    {
        static readonly DateTimeOffsetTypeConverter _dateTimeOffsetConverter = new DateTimeOffsetTypeConverter();
        static readonly DateTimeTypeConverter _dateTimeConverter = new DateTimeTypeConverter();
        readonly RabbitMqSendTransportContext _context;

        public RabbitMqSendTransport(RabbitMqSendTransportContext context)
        {
            _context = context;

            Add(context.ModelContextSupervisor);
        }

        public async ValueTask DisposeAsync()
        {
            await this.Stop("Disposed").ConfigureAwait(false);
        }

        Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            if (IsStopped)
                throw new TransportUnavailableException($"The RabbitMQ send transport is stopped: {_context.Exchange}");

            LogContext.SetCurrentIfNull(_context.LogContext);

            var sendPipe = new SendPipe<T>(_context, message, pipe, cancellationToken);

            return _context.Send(sendPipe, cancellationToken);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _context.ConnectSendObserver(observer);
        }

        protected override Task StopSupervisor(StopSupervisorContext context)
        {
            TransportLogMessages.StoppingSendTransport(_context.Exchange);

            return base.StopSupervisor(context);
        }


        class SendPipe<T> :
            IPipe<ModelContext>
            where T : class
        {
            readonly CancellationToken _cancellationToken;
            readonly RabbitMqSendTransportContext _context;
            readonly T _message;
            readonly IPipe<SendContext<T>> _pipe;

            public SendPipe(RabbitMqSendTransportContext context, T message, IPipe<SendContext<T>> pipe, CancellationToken
                cancellationToken)
            {
                _context = context;
                _message = message;
                _pipe = pipe;
                _cancellationToken = cancellationToken;
            }

            public async Task Send(ModelContext modelContext)
            {
                await _context.ConfigureTopologyPipe.Send(modelContext).ConfigureAwait(false);

                var properties = modelContext.Model.CreateBasicProperties();

                var context = new BasicPublishRabbitMqSendContext<T>(properties, _context.Exchange, _message, _cancellationToken);

                await _pipe.Send(context).ConfigureAwait(false);

                var activityName = _context.ActivityName;

                var exchange = context.Exchange;
                if (exchange.Equals(RabbitMqExchangeNames.ReplyTo))
                {
                    if (string.IsNullOrWhiteSpace(context.RoutingKey))
                        throw new TransportException(context.DestinationAddress, "RoutingKey must be specified when sending to reply-to address");

                    exchange = "";
                    activityName = "reply-to send";
                }

                StartedActivity? activity = LogContext.Current?.StartSendActivity(_context, context);
                try
                {
                    if (_context.SendObservers.Count > 0)
                        await _context.SendObservers.PreSend(context).ConfigureAwait(false);

                    var body = context.Body.GetBytes();

                    if (context.TryGetPayload(out PublishContext publishContext))
                        context.Mandatory = context.Mandatory || publishContext.Mandatory;

                    properties.Headers ??= new Dictionary<string, object>();

                    properties.ContentType = context.ContentType.ToString();

                    SetHeaders(properties.Headers, context.Headers);

                    properties.Persistent = context.Durable;

                    if (context.MessageId.HasValue)
                        properties.MessageId = context.MessageId.ToString();

                    if (context.CorrelationId.HasValue)
                        properties.CorrelationId = context.CorrelationId.ToString();

                    if (context.TimeToLive.HasValue)
                    {
                        properties.Expiration = (context.TimeToLive > TimeSpan.Zero ? context.TimeToLive.Value : TimeSpan.FromSeconds(1)).TotalMilliseconds
                            .ToString("F0", CultureInfo.InvariantCulture);
                    }

                    if (context.RequestId.HasValue && (context.ResponseAddress?.AbsolutePath?.EndsWith(RabbitMqExchangeNames.ReplyTo) ?? false))
                        context.BasicProperties.ReplyTo = RabbitMqExchangeNames.ReplyTo;

                    var delay = context.Delay?.TotalMilliseconds;
                    if (delay > 0 && exchange != "")
                    {
                        await _context.DelayConfigureTopologyPipe.Send(modelContext).ConfigureAwait(false);
                        context.SetTransportHeader("x-delay", (long)delay.Value);

                        exchange = _context.DelayExchange;
                    }

                    var routingKey = context.RoutingKey ?? "";
                    if (!string.IsNullOrEmpty(routingKey))
                        activity?.AddTag(DiagnosticHeaders.Messaging.RabbitMq.RoutingKey, routingKey);

                    var publishTask = modelContext.BasicPublishAsync(exchange, routingKey, context.Mandatory, context.BasicProperties, body,
                        context.AwaitAck);

                    await publishTask.OrCanceled(context.CancellationToken).ConfigureAwait(false);

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

            public void Probe(ProbeContext context)
            {
            }

            static void SetHeaders(IDictionary<string, object> dictionary, SendHeaders headers)
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
                            if (_dateTimeOffsetConverter.TryConvert(value, out long result))
                                dictionary[header.Key] = new AmqpTimestamp(result);
                            else if (_dateTimeOffsetConverter.TryConvert(value, out string text))
                                dictionary[header.Key] = text;

                            break;

                        case DateTime value:
                            if (_dateTimeConverter.TryConvert(value, out result))
                                dictionary[header.Key] = new AmqpTimestamp(result);
                            else if (_dateTimeConverter.TryConvert(value, out string text))
                                dictionary[header.Key] = text;

                            break;

                        case Guid value:
                            dictionary[header.Key] = value.ToString("D");
                            break;

                        case string value when header.Key == "CC" || header.Key == "BCC":
                            dictionary[header.Key] = new[] { value };
                            break;

                        case IEnumerable<string> strings when header.Key == "CC" || header.Key == "BCC":
                            dictionary[header.Key] = strings.ToArray();
                            break;

                        case Uri value:
                            dictionary[header.Key] = value.ToString();
                            break;

                        case string value:
                            dictionary[header.Key] = value;
                            break;

                        case bool value when value:
                            dictionary[header.Key] = bool.TrueString;
                            break;

                        case IFormattable formatValue:
                            if (header.Value.GetType().IsValueType)
                                dictionary[header.Key] = header.Value;
                            else
                                dictionary[header.Key] = formatValue.ToString();
                            break;
                    }
                }
            }
        }
    }
}
