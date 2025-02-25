namespace MassTransit.ActiveMqTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Apache.NMS.ActiveMQ;
    using Configuration;
    using Internals;
    using Transports;


    public class ActiveMqSendTransportContext :
        BaseSendTransportContext,
        SendTransportContext<SessionContext>
    {
        readonly IPipe<SessionContext> _configureTopologyPipe;
        readonly DestinationType _destinationType;
        readonly IActiveMqHostConfiguration _hostConfiguration;
        readonly ISessionContextSupervisor _supervisor;

        public ActiveMqSendTransportContext(IActiveMqHostConfiguration hostConfiguration, ReceiveEndpointContext receiveEndpointContext,
            ISessionContextSupervisor supervisor, IPipe<SessionContext> configureTopologyPipe, string entityName, DestinationType destinationType)
            : base(hostConfiguration, receiveEndpointContext.Serialization)
        {
            _hostConfiguration = hostConfiguration;
            _supervisor = supervisor;
            _configureTopologyPipe = configureTopologyPipe;
            _destinationType = destinationType;

            EntityName = entityName;
        }

        public override string EntityName { get; }
        public override string ActivitySystem => "activemq";

        public Task Send(IPipe<SessionContext> pipe, CancellationToken cancellationToken = default)
        {
            return _hostConfiguration.Retry(() => _supervisor.Send(pipe, cancellationToken), cancellationToken, _supervisor.SendStopping);
        }

        public void Probe(ProbeContext context)
        {
            _supervisor.Probe(context);
        }

        public override async Task<SendContext<T>> CreateSendContext<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            var sendContext = new TransportActiveMqSendContext<T>(message, cancellationToken);

            await pipe.Send(sendContext).ConfigureAwait(false);

            return sendContext;
        }

        public override IEnumerable<IAgent> GetAgentHandles()
        {
            return new IAgent[] { _supervisor };
        }

        public Task<SendContext<T>> CreateSendContext<T>(SessionContext sessionContext, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            return CreateSendContext(message, pipe, cancellationToken);
        }

        public async Task Send<T>(SessionContext sessionContext, SendContext<T> sendContext)
            where T : class
        {
            TransportActiveMqSendContext<T> context = sendContext as TransportActiveMqSendContext<T>
                ?? throw new ArgumentException("Invalid SendContext<T> type", nameof(sendContext));

            sendContext.CancellationToken.ThrowIfCancellationRequested();

            await _configureTopologyPipe.Send(sessionContext).ConfigureAwait(false);

            sendContext.CancellationToken.ThrowIfCancellationRequested();

            var destination = context.ReplyDestination ?? await sessionContext.GetDestination(EntityName, _destinationType).ConfigureAwait(false);

            var transportMessage = sessionContext.CreateBytesMessage(context.Body.GetBytes());

            await SetResponseTo(transportMessage, context, sessionContext);

            transportMessage.Properties.SetHeaders(context.Headers);

            transportMessage.Properties[MessageHeaders.ContentType] = context.ContentType.ToString();

            if (context.MessageId.HasValue)
                transportMessage.NMSMessageId = context.MessageId.ToString();

            if (context.CorrelationId.HasValue)
                transportMessage.NMSCorrelationID = context.CorrelationId.ToString();

            transportMessage.NMSDeliveryMode = context.Durable ? MsgDeliveryMode.Persistent : MsgDeliveryMode.NonPersistent;

            if (context.TimeToLive.HasValue)
                transportMessage.NMSTimeToLive = context.TimeToLive > TimeSpan.Zero ? context.TimeToLive.Value : TimeSpan.FromSeconds(1);
            //If protocol is AMQP TTL cannot be TimeSpan.Zero = NMSConstants.defaultTimeToLive. A message sent with TTL 0 will be discarded when broker receive it.
            //Otherwise OpenWire protocol does not set TTL=0 to a message when is 0.
            else if (sessionContext.Session is Session)
                transportMessage.NMSTimeToLive = NMSConstants.defaultTimeToLive;

            transportMessage.NMSPriority = context.Priority ?? NMSConstants.defaultPriority;

            if (!string.IsNullOrWhiteSpace(context.GroupId))
                transportMessage.SetGroupId(context.GroupId);

            if (context.GroupSequence.HasValue)
                transportMessage.SetGroupSequence(context.GroupSequence.Value);

            var delay = context.Delay?.TotalMilliseconds;
            if (delay > 0)
            {
                if (_hostConfiguration.IsArtemis)
                    transportMessage.Properties["_AMQ_SCHED_DELIVERY"] = (DateTimeOffset.UtcNow + context.Delay.Value).ToUnixTimeMilliseconds();
                else
                    transportMessage.Properties["AMQ_SCHEDULED_DELAY"] = (long)delay.Value;
            }

            await sessionContext.SendAsync(destination, transportMessage, context.CancellationToken).ConfigureAwait(false);
        }

        static async Task SetResponseTo(IMessage transportMessage, SendContext context, SessionContext sessionContext)
        {
            if (context.ResponseAddress == null)
                return;

            var endpointName = context.ResponseAddress.GetEndpointName();

            transportMessage.NMSReplyTo = sessionContext.GetTemporaryDestination(endpointName)
                ?? (context.ResponseAddress.TryGetValueFromQueryString("temporary", out _)
                    ? await sessionContext.GetDestination(endpointName, DestinationType.TemporaryQueue)
                    : await sessionContext.GetDestination(endpointName, DestinationType.Queue));
        }
    }
}
