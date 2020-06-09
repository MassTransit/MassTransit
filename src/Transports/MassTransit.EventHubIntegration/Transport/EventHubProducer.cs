namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Producer;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using Initializers;
    using Logging;
    using Transports;


    public class EventHubProducer :
        IEventHubProducer
    {
        readonly ConsumeContext _consumeContext;
        readonly IEventHubProducerContext _context;
        readonly EventHubEndpointAddress _topicAddress;

        public EventHubProducer(EventHubEndpointAddress topicAddress, IEventHubProducerContext context, ConsumeContext consumeContext = null)
        {
            _topicAddress = topicAddress;
            _context = context;
            _consumeContext = consumeContext;
        }

        public Task Produce<TValue>(TValue value, CancellationToken cancellationToken = default)
            where TValue : class
        {
            return Produce(value, Pipe.Empty<EventHubSendContext<TValue>>(), cancellationToken);
        }

        public Task Produce<TValue>(IEnumerable<TValue> values, CancellationToken cancellationToken = default)
            where TValue : class
        {
            return Produce(values, Pipe.Empty<EventHubSendContext<TValue>>(), cancellationToken);
        }

        public async Task Produce<TValue>(TValue value, IPipe<EventHubSendContext<TValue>> pipe, CancellationToken cancellationToken)
            where TValue : class
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            var context = new EventHubMessageSendContext<TValue>(value, cancellationToken) {Serializer = _context.Serializer};

            if (_consumeContext != null)
                context.TransferConsumeContextHeaders(_consumeContext);

            context.DestinationAddress = _topicAddress;

            await _context.Send(context).ConfigureAwait(false);

            if (pipe.IsNotEmpty())
                await pipe.Send(context).ConfigureAwait(false);

            context.SourceAddress ??= _context.HostAddress;
            context.ConversationId ??= NewId.NextGuid();

            StartedActivity? activity = LogContext.IfEnabled(OperationName.Transport.Send)?.StartSendActivity(context,
                (nameof(context.PartitionId), _topicAddress.PartitionId ?? context.PartitionId),
                (nameof(context.PartitionKey), _topicAddress.PartitionKey ?? context.PartitionKey));
            try
            {
                if (_context.SendObservers.Count > 0)
                    await _context.SendObservers.PreSend(context).ConfigureAwait(false);

                var eventData = new EventData(context.Body);

                eventData.Properties.Set(context.Headers);

                var options = new SendEventOptions
                {
                    PartitionId = _topicAddress.PartitionId ?? context.PartitionId,
                    PartitionKey = _topicAddress.PartitionKey ?? context.PartitionKey
                };

                await _context.Produce(new[] {eventData}, options, context.CancellationToken).ConfigureAwait(false);

                context.LogSent();

                if (_context.SendObservers.Count > 0)
                    await _context.SendObservers.PostSend(context).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                context.LogFaulted(exception);

                if (_context.SendObservers.Count > 0)
                    await _context.SendObservers.SendFault(context, exception).ConfigureAwait(false);

                throw;
            }
            finally
            {
                activity?.Stop();
            }
        }

        public async Task Produce<TValue>(IEnumerable<TValue> values, IPipe<EventHubSendContext<TValue>> pipe, CancellationToken cancellationToken = default)
            where TValue : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            LogContext.SetCurrentIfNull(_context.LogContext);

            EventHubMessageSendContext<TValue>[] contexts = values
                .Select(x => new EventHubMessageSendContext<TValue>(x, cancellationToken) {Serializer = _context.Serializer})
                .ToArray();

            if (contexts.Length == 0)
                return;

            NewId[] ids = NewId.Next(contexts.Length);

            async Task Send(EventHubMessageSendContext<TValue> c, int idx)
            {
                if (_consumeContext != null)
                    c.TransferConsumeContextHeaders(_consumeContext);

                c.DestinationAddress = _topicAddress;

                await _context.Send(c).ConfigureAwait(false);

                if (pipe.IsNotEmpty())
                    await pipe.Send(c).ConfigureAwait(false);

                c.SourceAddress ??= _context.HostAddress;
                c.ConversationId ??= ids[idx].ToGuid();
            }

            await Task.WhenAll(contexts.Select(Send)).ConfigureAwait(false);

            EventHubMessageSendContext<TValue> context = contexts[0];
            var options = new CreateBatchOptions
            {
                PartitionId = _topicAddress.PartitionId ?? context.PartitionId,
                PartitionKey = _topicAddress.PartitionKey ?? context.PartitionKey
            };

            StartedActivity? activity = LogContext.IfEnabled(OperationName.Transport.Send)?.StartSendActivity(context,
                (nameof(EventHubMessageSendContext<TValue>.PartitionId), options.PartitionId),
                (nameof(EventHubMessageSendContext<TValue>.PartitionKey), options.PartitionKey));
            try
            {
                var eventDataBatch = await _context.CreateBatch(options, context.CancellationToken).ConfigureAwait(false);

                if (_context.SendObservers.Count > 0)
                    await Task.WhenAll(contexts.Select(c => _context.SendObservers.PreSend(c))).ConfigureAwait(false);

                async Task FlushAsync(EventDataBatch batch)
                {
                    await _context.Produce(batch, context.CancellationToken).ConfigureAwait(false);
                    batch.Dispose();
                }

                for (var i = 0; i < contexts.Length; i++)
                {
                    EventHubMessageSendContext<TValue> c = contexts[i];

                    var eventData = new EventData(c.Body);

                    eventData.Properties.Set(c.Headers);

                    while (!eventDataBatch.TryAdd(eventData) && eventDataBatch.Count > 0)
                    {
                        await FlushAsync(eventDataBatch);
                        eventDataBatch = await _context.CreateBatch(options, context.CancellationToken).ConfigureAwait(false);
                    }
                }

                if (eventDataBatch.Count > 0)
                    await FlushAsync(eventDataBatch);

                context.LogSent();

                if (_context.SendObservers.Count > 0)
                    await Task.WhenAll(contexts.Select(c => _context.SendObservers.PostSend(c))).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                context.LogFaulted(exception);

                if (_context.SendObservers.Count > 0)
                    await Task.WhenAll(contexts.Select(c => _context.SendObservers.SendFault(c, exception))).ConfigureAwait(false);

                throw;
            }
            finally
            {
                activity?.Stop();
            }
        }

        public Task Produce<TValue>(object values, CancellationToken cancellationToken = default)
            where TValue : class
        {
            return Produce(values, Pipe.Empty<EventHubSendContext<TValue>>(), cancellationToken);
        }

        public Task Produce<TValue>(IEnumerable<object> values, CancellationToken cancellationToken = default)
            where TValue : class
        {
            return Produce(values, Pipe.Empty<EventHubSendContext<TValue>>(), cancellationToken);
        }

        public Task Produce<TValue>(object values, IPipe<EventHubSendContext<TValue>> pipe, CancellationToken cancellationToken = default)
            where TValue : class
        {
            Task<InitializeContext<TValue>> messageTask = MessageInitializerCache<TValue>.Initialize(values, cancellationToken);
            if (messageTask.IsCompletedSuccessfully())
                return Produce(messageTask.GetAwaiter().GetResult().Message, pipe, cancellationToken);

            async Task ProduceAsync()
            {
                InitializeContext<TValue> context = await messageTask.ConfigureAwait(false);

                await Produce(context.Message, pipe, cancellationToken).ConfigureAwait(false);
            }

            return ProduceAsync();
        }

        public Task Produce<TValue>(IEnumerable<object> values, IPipe<EventHubSendContext<TValue>> pipe, CancellationToken cancellationToken = default)
            where TValue : class
        {
            Task<InitializeContext<TValue>>[] messageTasks = values
                .Select(value => MessageInitializerCache<TValue>.Initialize(value, cancellationToken))
                .ToArray();

            async Task ProduceAsync()
            {
                InitializeContext<TValue>[] contexts = await Task.WhenAll(messageTasks).ConfigureAwait(false);

                await Produce(contexts.Select(x => x.Message), pipe, cancellationToken).ConfigureAwait(false);
            }

            return ProduceAsync();
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _context.SendObservers.Connect(observer);
        }
    }
}
