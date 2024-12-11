namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Internals;
    using Logging;
    using Middleware;
    using RabbitMQ.Client;
    using Transports;


    public class RabbitMqSendTransportContext :
        BaseSendTransportContext,
        SendTransportContext<ChannelContext>
    {
        readonly ConfigureRabbitMqTopologyFilter<SendSettings> _configureTopologyFilter;
        readonly IPipe<ChannelContext> _delayConfigureTopologyPipe;
        readonly string _delayExchange;
        readonly string _exchange;

        readonly IRabbitMqHostConfiguration _hostConfiguration;
        readonly IChannelContextSupervisor _supervisor;

        public RabbitMqSendTransportContext(IRabbitMqHostConfiguration hostConfiguration, ReceiveEndpointContext receiveEndpointContext,
            IChannelContextSupervisor supervisor,
            ConfigureRabbitMqTopologyFilter<SendSettings> configureTopologyFilter, string exchange,
            IPipe<ChannelContext> delayConfigureTopologyPipe, string delayExchange)
            : base(hostConfiguration, receiveEndpointContext.Serialization)
        {
            _hostConfiguration = hostConfiguration;
            _supervisor = supervisor;

            _configureTopologyFilter = configureTopologyFilter;
            _exchange = exchange;

            _delayConfigureTopologyPipe = delayConfigureTopologyPipe;
            _delayExchange = delayExchange;
        }

        public override string EntityName => _exchange;
        public override string ActivitySystem => "rabbitmq";

        public Task Send(IPipe<ChannelContext> pipe, CancellationToken cancellationToken = default)
        {
            return _hostConfiguration.Retry(() => _supervisor.Send(pipe, cancellationToken), cancellationToken, _supervisor.SendStopping);
        }

        public void Probe(ProbeContext context)
        {
            _supervisor.Probe(context);
        }

        public override IEnumerable<IAgent> GetAgentHandles()
        {
            return [_supervisor];
        }

        public async Task<SendContext<T>> CreateSendContext<T>(ChannelContext context, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            var properties = new BasicProperties();

            var sendContext = new RabbitMqMessageSendContext<T>(properties, _exchange, message, cancellationToken);

            await pipe.Send(sendContext).ConfigureAwait(false);

            CopyIncomingPropertiesIfPresent(sendContext);

            if (sendContext.Exchange.Equals(RabbitMqExchangeNames.ReplyTo) && string.IsNullOrWhiteSpace(sendContext.RoutingKey))
                throw new TransportException(sendContext.DestinationAddress, "RoutingKey must be specified when sending to reply-to address");

            return sendContext;
        }

        public override async Task<SendContext<T>> CreateSendContext<T>(T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            var properties = new BasicProperties();

            var sendContext = new RabbitMqMessageSendContext<T>(properties, _exchange, message, cancellationToken);

            await pipe.Send(sendContext).ConfigureAwait(false);

            CopyIncomingPropertiesIfPresent(sendContext);

            if (sendContext.Exchange.Equals(RabbitMqExchangeNames.ReplyTo) && string.IsNullOrWhiteSpace(sendContext.RoutingKey))
                throw new TransportException(sendContext.DestinationAddress, "RoutingKey must be specified when sending to reply-to address");

            return sendContext;
        }

        public async Task Send<T>(ChannelContext transportContext, SendContext<T> sendContext)
            where T : class
        {
            RabbitMqMessageSendContext<T> context = sendContext as RabbitMqMessageSendContext<T>
                ?? throw new ArgumentException("Invalid SendContext<T> type", nameof(sendContext));

            sendContext.CancellationToken.ThrowIfCancellationRequested();

            OneTimeContext<ConfigureTopologyContext<SendSettings>> oneTimeContext =
                await _configureTopologyFilter.Configure(transportContext).ConfigureAwait(false);

            sendContext.CancellationToken.ThrowIfCancellationRequested();

            var exchange = context.Exchange;
            if (exchange.Equals(RabbitMqExchangeNames.ReplyTo))
                exchange = "";

            var body = context.Body.GetBytes();

            if (context.TryGetPayload(out PublishContext publishContext))
                context.Mandatory = context.Mandatory || publishContext.Mandatory;

            context.BasicProperties.Headers ??= new Dictionary<string, object>();

            context.BasicProperties.ContentType = context.ContentType?.ToString();

            SetHeaders(context.BasicProperties.Headers, context.Headers);

            context.BasicProperties.Persistent = context.Durable;

            if (context.MessageId.HasValue)
                context.BasicProperties.MessageId = context.MessageId.ToString();

            if (context.CorrelationId.HasValue)
                context.BasicProperties.CorrelationId = context.CorrelationId.ToString();

            if (context.TimeToLive.HasValue)
            {
                context.BasicProperties.Expiration =
                    (context.TimeToLive > TimeSpan.Zero ? context.TimeToLive.Value : TimeSpan.FromSeconds(1))
                    .TotalMilliseconds
                    .ToString("F0", CultureInfo.InvariantCulture);
            }

            if (context.RequestId.HasValue && context.ResponseAddress.IsReplyToAddress())
                context.BasicProperties.ReplyTo ??= RabbitMqExchangeNames.ReplyTo;

            var delay = context.Delay?.TotalMilliseconds;
            if (delay > 0 && exchange != "")
            {
                await _delayConfigureTopologyPipe.Send(transportContext).ConfigureAwait(false);
                context.SetTransportHeader("x-delay", (long)delay.Value);

                exchange = _delayExchange;
            }

            var routingKey = context.RoutingKey ?? "";

            if (Activity.Current?.IsAllDataRequested ?? false)
            {
                if (!string.IsNullOrEmpty(routingKey))
                    Activity.Current.SetTag(DiagnosticHeaders.Messaging.RabbitMq.RoutingKey, routingKey);
            }

            var publishTask = transportContext.BasicPublishAsync(exchange, routingKey, context.Mandatory, context.BasicProperties, body,
                context.AwaitAck);

            try
            {
                await publishTask.OrCanceled(context.CancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                oneTimeContext.Evict();
                throw;
            }
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

                if (header.Key == RabbitMqHeaders.Exchange
                    || header.Key == RabbitMqHeaders.RoutingKey
                    || header.Key == RabbitMqHeaders.DeliveryTag
                    || header.Key == RabbitMqHeaders.ConsumerTag)
                    continue;

                if (dictionary.ContainsKey(header.Key))
                    continue;

                switch (header.Value)
                {
                    case DateTimeOffset value:
                        dictionary.SetAmqpTimestamp(header.Key, value.UtcDateTime);
                        break;

                    case DateTime value:
                        if (value.Kind == DateTimeKind.Local)
                            value = value.ToUniversalTime();
                        dictionary.SetAmqpTimestamp(header.Key, value);
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

        static void CopyIncomingPropertiesIfPresent<T>(RabbitMqSendContext<T> context)
            where T : class
        {
            if (context.TryGetPayload<ConsumeContext>(out var consumeContext)
                && consumeContext.TryGetPayload<RabbitMqBasicConsumeContext>(out var basicConsumeContext))
            {
                if (context.BasicProperties.IsPriorityPresent() == false)
                {
                    if (basicConsumeContext.Properties.IsPriorityPresent())
                        context.TrySetPriority(basicConsumeContext.Properties.Priority);
                }

                if (!string.IsNullOrWhiteSpace(basicConsumeContext.Properties.ReplyTo) && context.ResponseAddress.IsReplyToAddress())
                    context.BasicProperties.ReplyTo = basicConsumeContext.Properties.ReplyTo;
            }
        }
    }
}
