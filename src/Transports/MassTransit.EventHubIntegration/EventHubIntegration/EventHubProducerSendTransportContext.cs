namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Producer;
    using MassTransit.Configuration;
    using Transports;


    public class EventHubProducerSendTransportContext :
        BaseSendTransportContext,
        EventHubSendTransportContext
    {
        readonly IHostConfiguration _configuration;
        readonly EventHubEndpointAddress _endpointAddress;
        readonly ISendPipe _sendPipe;
        readonly IProducerContextSupervisor _supervisor;

        public EventHubProducerSendTransportContext(IProducerContextSupervisor supervisor, ISendPipe sendPipe,
            IHostConfiguration configuration, string eventHubName, ISerialization serialization)
            : base(configuration, serialization)
        {
            _sendPipe = sendPipe;
            _supervisor = supervisor;
            _configuration = configuration;
            _endpointAddress = new EventHubEndpointAddress(configuration.HostAddress, eventHubName);
        }

        public override IEnumerable<IAgent> GetAgentHandles()
        {
            return new[] { _supervisor };
        }

        public async Task<EventHubSendContext<T>> CreateContext<T>(T value, IPipe<EventHubSendContext<T>> pipe, CancellationToken cancellationToken,
            IPipe<SendContext<T>> initializerPipe = null)
            where T : class
        {
            var context = new EventHubMessageSendContext<T>(value, cancellationToken)
            {
                Serializer = Serialization.GetMessageSerializer(),
                DestinationAddress = _endpointAddress
            };

            if (pipe is ISendContextPipe sendPipe)
                await sendPipe.Send(context).ConfigureAwait(false);

            await _sendPipe.Send(context).ConfigureAwait(false);

            if (initializerPipe.IsNotEmpty())
                await initializerPipe.Send(context).ConfigureAwait(false);

            if (pipe.IsNotEmpty())
                await pipe.Send(context).ConfigureAwait(false);

            context.SourceAddress ??= _configuration.HostAddress;

            return context;
        }

        public Task Send<T>(ProducerContext producerContext, EventHubSendContext<T> sendContext)
            where T : class
        {
            EventHubMessageSendContext<T> context = sendContext as EventHubMessageSendContext<T>
                ?? throw new ArgumentException("Invalid SendContext<T> type", nameof(sendContext));

            context.ConversationId ??= NewId.NextGuid();

            var options = new SendEventOptions
            {
                PartitionId = context.PartitionId,
                PartitionKey = context.PartitionKey
            };

            if (Activity.Current?.IsAllDataRequested ?? false)
            {
                if (!string.IsNullOrEmpty(options.PartitionId))
                    Activity.Current.SetTag(nameof(context.PartitionId), options.PartitionId);
                if (!string.IsNullOrEmpty(options.PartitionKey))
                    Activity.Current.SetTag(nameof(context.PartitionKey), options.PartitionKey);
            }

            var eventData = new EventData(context.Body.GetBytes());

            eventData.Properties.Set(context.Headers);

            if (context.MessageId.HasValue)
                eventData.MessageId = context.MessageId.Value.ToString("N");

            if (context.CorrelationId.HasValue)
                eventData.CorrelationId = context.CorrelationId.Value.ToString("N");

            eventData.ContentType = context.ContentType.ToString();

            context.CancellationToken.ThrowIfCancellationRequested();

            return producerContext.Produce(new[] { eventData }, options, context.CancellationToken);
        }

        public async Task Send<T>(ProducerContext producerContext, EventHubSendContext<T>[] sendContexts)
            where T : class
        {
            EventHubSendContext<T> sendContext = sendContexts[0];
            var options = new CreateBatchOptions
            {
                PartitionId = sendContext.PartitionId,
                PartitionKey = sendContext.PartitionKey
            };

            if (Activity.Current?.IsAllDataRequested ?? false)
            {
                Activity.Current.SetTag(nameof(sendContext.PartitionId), options.PartitionId);
                Activity.Current.SetTag(nameof(sendContext.PartitionKey), options.PartitionKey);
            }

            sendContext.CancellationToken.ThrowIfCancellationRequested();

            var eventDataBatch = await producerContext.CreateBatch(options, sendContext.CancellationToken).ConfigureAwait(false);

            async Task FlushAsync(EventDataBatch batch)
            {
                try
                {
                    sendContext.CancellationToken.ThrowIfCancellationRequested();

                    await producerContext.Produce(batch, sendContext.CancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    batch.Dispose();
                }
            }

            NewId[] ids = NewId.Next(sendContexts.Length);

            for (var i = 0; i < sendContexts.Length; i++)
            {
                EventHubMessageSendContext<T> context = sendContexts[i] as EventHubMessageSendContext<T>
                    ?? throw new ArgumentException("Invalid SendContext<T> type", nameof(sendContext));

                context.ConversationId ??= ids[i].ToGuid();

                var eventData = new EventData(context.Body.GetBytes());

                if (context.MessageId.HasValue)
                    eventData.MessageId = context.MessageId.Value.ToString("N");

                if (context.CorrelationId.HasValue)
                    eventData.CorrelationId = context.CorrelationId.Value.ToString("N");

                eventData.ContentType = context.ContentType.ToString();

                eventData.Properties.Set(context.Headers);

                if (eventDataBatch.TryAdd(eventData))
                    continue;

                await FlushAsync(eventDataBatch).ConfigureAwait(false);
                eventDataBatch = await producerContext.CreateBatch(options, context.CancellationToken).ConfigureAwait(false);

                if (!eventDataBatch.TryAdd(eventData))
                    throw new ApplicationException("Message can not be added to the empty EventDataBatch");
            }

            if (eventDataBatch.Count > 0)
                await FlushAsync(eventDataBatch).ConfigureAwait(false);
        }

        public Task Send(IPipe<ProducerContext> pipe, CancellationToken cancellationToken)
        {
            return _configuration.Retry(() => _supervisor.Send(pipe, cancellationToken), cancellationToken, _supervisor.SendStopping);
        }

        public override string EntityName => _endpointAddress.EventHubName;
        public override string ActivitySystem => "eventhubs";

        public override Task<SendContext<T>> CreateSendContext<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            throw new NotImplementedByDesignException("Event Hub is a producer, not an outbox compatible transport");
        }

        public void Probe(ProbeContext context)
        {
            _supervisor.Probe(context);
        }
    }
}
