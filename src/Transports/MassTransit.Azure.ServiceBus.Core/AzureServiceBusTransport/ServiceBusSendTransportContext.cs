namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using Configuration;
    using Scheduling;
    using Transports;


    public class ServiceBusSendTransportContext :
        BaseSendTransportContext,
        SendTransportContext<SendEndpointContext>
    {
        static readonly ITransportSetHeaderAdapter<object> _adapter = new DictionaryTransportSetHeaderAdapter(new SimpleHeaderValueConverter());

        readonly IServiceBusHostConfiguration _hostConfiguration;
        readonly ISendEndpointContextSupervisor _supervisor;

        public ServiceBusSendTransportContext(IServiceBusHostConfiguration hostConfiguration, ReceiveEndpointContext receiveEndpointContext,
            ISendEndpointContextSupervisor supervisor, SendSettings settings)
            : base(hostConfiguration, receiveEndpointContext.Serialization)
        {
            _hostConfiguration = hostConfiguration;
            _supervisor = supervisor;

            EntityName = settings.EntityPath;
        }

        public override string EntityName { get; }
        public override string ActivitySystem => "servicebus";

        public Task Send(IPipe<SendEndpointContext> pipe, CancellationToken cancellationToken = default)
        {
            return _hostConfiguration.Retry(() => _supervisor.Send(pipe, cancellationToken), cancellationToken, _supervisor.SendStopping);
        }

        public void Probe(ProbeContext context)
        {
            _supervisor.Probe(context);
        }

        public override async Task<SendContext<T>> CreateSendContext<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            var sendContext = new AzureServiceBusSendContext<T>(message, cancellationToken);

            await pipe.Send(sendContext).ConfigureAwait(false);

            CopyIncomingIdentifiersIfPresent(sendContext);

            return sendContext;
        }

        public override IEnumerable<IAgent> GetAgentHandles()
        {
            return new IAgent[] { _supervisor };
        }

        public Task<SendContext<T>> CreateSendContext<T>(SendEndpointContext context, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            return CreateSendContext(message, pipe, cancellationToken);
        }

        public async Task Send<T>(SendEndpointContext sendEndpointContext, SendContext<T> sendContext)
            where T : class
        {
            AzureServiceBusSendContext<T> context = sendContext as AzureServiceBusSendContext<T>
                ?? throw new ArgumentException("Invalid SendContext<T> type", nameof(sendContext));

            if (Activity.Current?.IsAllDataRequested ?? false)
            {
                if (!string.IsNullOrWhiteSpace(context.PartitionKey))
                    Activity.Current.SetTag(nameof(context.PartitionKey), context.PartitionKey);
                if (!string.IsNullOrWhiteSpace(context.SessionId))
                    Activity.Current.SetTag(nameof(context.SessionId), context.SessionId);
            }

            sendContext.CancellationToken.ThrowIfCancellationRequested();

            if (IsCancelScheduledSend(context, out var tokenId, out var sequenceNumber))
            {
                await CancelScheduledSend(sendEndpointContext, tokenId, sequenceNumber).ConfigureAwait(false);

                return;
            }

            if (context.ScheduledEnqueueTimeUtc.HasValue)
            {
                var scheduled = await ScheduleSend(sendEndpointContext, context).ConfigureAwait(false);
                if (scheduled)
                    return;
            }

            var message = CreateMessage(context);

            await sendEndpointContext.Send(message).ConfigureAwait(false);
        }

        static async Task<bool> ScheduleSend<T>(SendEndpointContext clientContext, AzureServiceBusSendContext<T> context)
            where T : class
        {
            var now = DateTime.UtcNow;

            var enqueueTimeUtc = context.ScheduledEnqueueTimeUtc.Value;
            if (enqueueTimeUtc < now)
            {
                MassTransit.LogContext.Debug?.Log("The scheduled time was in the past, sending: {ScheduledTime}", context.ScheduledEnqueueTimeUtc);

                return false;
            }

            try
            {
                context.Headers.Set(MessageHeaders.SchedulingTokenId, null);

                var message = CreateMessage(context);

                var sequenceNumber = await clientContext.ScheduleSend(message, enqueueTimeUtc).ConfigureAwait(false);

                context.SetScheduledMessageId(sequenceNumber);

                context.LogScheduled(enqueueTimeUtc);

                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                MassTransit.LogContext.Debug?.Log("The scheduled time was rejected by the server, sending: {MessageId}", context.MessageId);

                return false;
            }
        }

        async Task CancelScheduledSend(SendEndpointContext clientContext, Guid tokenId, long sequenceNumber)
        {
            try
            {
                await clientContext.CancelScheduledSend(sequenceNumber).ConfigureAwait(false);

                MassTransit.LogContext.Debug?.Log("CANCEL {DestinationAddress} {TokenId}", EntityName, tokenId);
            }
            catch (ServiceBusException exception) when (exception.Reason == ServiceBusFailureReason.MessageNotFound)
            {
                MassTransit.LogContext.Debug?.Log("CANCEL {DestinationAddress} {TokenId} message not found", EntityName, tokenId);
            }
            catch (InvalidOperationException exception) when (exception.Message.Contains("already being cancelled"))
            {
                MassTransit.LogContext.Debug?.Log("CANCEL {DestinationAddress} {TokenId} message already being canceled", EntityName, tokenId);
            }
        }

        static bool IsCancelScheduledSend<T>(AzureServiceBusSendContext<T> context, out Guid tokenId, out long sequenceNumber)
            where T : class
        {
            if (context.Message is CancelScheduledMessage cancelScheduledMessage)
            {
                tokenId = cancelScheduledMessage.TokenId;

                if (context.TryGetScheduledMessageId(out sequenceNumber)
                    || context.TryGetSequenceNumber(cancelScheduledMessage.TokenId, out sequenceNumber))
                    return true;
            }

            tokenId = default;
            sequenceNumber = default;
            return false;
        }

        static ServiceBusMessage CreateMessage<T>(AzureServiceBusSendContext<T> context)
            where T : class
        {
            var message = new ServiceBusMessage(context.Body.GetBytes()) { ContentType = context.ContentType.ToString() };

            _adapter.Set(message.ApplicationProperties, context.Headers);

            if (context.TimeToLive.HasValue)
                message.TimeToLive = context.TimeToLive > TimeSpan.Zero ? context.TimeToLive.Value : TimeSpan.FromSeconds(1);

            if (context.MessageId.HasValue)
                message.MessageId = context.MessageId.Value.ToString("N");

            if (context.CorrelationId.HasValue)
                message.CorrelationId = context.CorrelationId.Value.ToString("N");

            if (context.PartitionKey != null)
                message.PartitionKey = context.PartitionKey;

            if (!string.IsNullOrWhiteSpace(context.SessionId))
            {
                message.SessionId = context.SessionId;

                if (context.ReplyToSessionId == null)
                    message.ReplyToSessionId = context.SessionId;
            }

            if (context.ReplyToSessionId != null)
                message.ReplyToSessionId = context.ReplyToSessionId;

            if (context.ReplyTo != null)
                message.ReplyTo = context.ReplyTo;

            if (context.Label != null)
                message.Subject = context.Label;

            return message;
        }

        static void CopyIncomingIdentifiersIfPresent<T>(AzureServiceBusSendContext<T> context)
            where T : class
        {
            if (context.TryGetPayload<ConsumeContext>(out var consumeContext)
                && consumeContext.TryGetPayload<ServiceBusMessageContext>(out var brokeredMessageContext))
            {
                if (context.SessionId == null)
                {
                    if (brokeredMessageContext.ReplyToSessionId != null)
                        context.SessionId = brokeredMessageContext.ReplyToSessionId;
                    else if (brokeredMessageContext.SessionId != null)
                        context.SessionId = brokeredMessageContext.SessionId;
                }

                if (context.PartitionKey == null && brokeredMessageContext.PartitionKey != null)
                    context.PartitionKey = brokeredMessageContext.PartitionKey;
            }
        }
    }
}
