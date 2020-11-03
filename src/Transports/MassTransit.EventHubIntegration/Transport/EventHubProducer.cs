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

        public Task Produce<T>(T message, CancellationToken cancellationToken = default)
            where T : class
        {
            return Produce(message, Pipe.Empty<EventHubSendContext<T>>(), cancellationToken);
        }

        public Task Produce<T>(IEnumerable<T> messages, CancellationToken cancellationToken = default)
            where T : class
        {
            return Produce(messages, Pipe.Empty<EventHubSendContext<T>>(), cancellationToken);
        }

        public async Task Produce<T>(T message, IPipe<EventHubSendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            var context = new EventHubMessageSendContext<T>(message, cancellationToken) {Serializer = _context.Serializer};

            if (_consumeContext != null)
                context.TransferConsumeContextHeaders(_consumeContext);

            context.DestinationAddress = _topicAddress;

            await _context.Send(context).ConfigureAwait(false);

            if (pipe.IsNotEmpty())
                await pipe.Send(context).ConfigureAwait(false);

            context.SourceAddress ??= _context.HostAddress;
            context.ConversationId ??= NewId.NextGuid();

            var options = new SendEventOptions
            {
                PartitionId = context.PartitionId,
                PartitionKey = context.PartitionKey
            };

            StartedActivity? activity = LogContext.IfEnabled(OperationName.Transport.Send)?.StartSendActivity(context,
                (nameof(context.PartitionId), options.PartitionId), (nameof(context.PartitionKey), options.PartitionKey));
            try
            {
                if (_context.SendObservers.Count > 0)
                    await _context.SendObservers.PreSend(context).ConfigureAwait(false);

                var eventData = new EventData(context.Body);

                eventData.Properties.Set(context.Headers);

                await _context.Produce(new[] {eventData}, options, context.CancellationToken).ConfigureAwait(false);

                context.LogSent();
                activity.AddSendContextHeadersPostSend(context);

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

        public async Task Produce<T>(IEnumerable<T> messages, IPipe<EventHubSendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            if (messages == null)
                throw new ArgumentNullException(nameof(messages));

            LogContext.SetCurrentIfNull(_context.LogContext);

            EventHubMessageSendContext<T>[] contexts = messages
                .Select(x => new EventHubMessageSendContext<T>(x, cancellationToken) {Serializer = _context.Serializer})
                .ToArray();

            if (contexts.Length == 0)
                return;

            NewId[] ids = NewId.Next(contexts.Length);

            async Task Send(EventHubMessageSendContext<T> c, int idx)
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

            EventHubMessageSendContext<T> context = contexts[0];
            var options = new CreateBatchOptions
            {
                PartitionId = context.PartitionId,
                PartitionKey = context.PartitionKey
            };

            StartedActivity? activity = LogContext.IfEnabled(OperationName.Transport.Send)?.StartSendActivity(context,
                (nameof(EventHubMessageSendContext<T>.PartitionId), options.PartitionId),
                (nameof(EventHubMessageSendContext<T>.PartitionKey), options.PartitionKey));
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
                    EventHubMessageSendContext<T> c = contexts[i];

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
                activity.AddSendContextHeadersPostSend(context);

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

        public Task Produce<T>(object values, CancellationToken cancellationToken = default)
            where T : class
        {
            return Produce(values, Pipe.Empty<EventHubSendContext<T>>(), cancellationToken);
        }

        public Task Produce<T>(IEnumerable<object> values, CancellationToken cancellationToken = default)
            where T : class
        {
            return Produce(values, Pipe.Empty<EventHubSendContext<T>>(), cancellationToken);
        }

        public Task Produce<T>(object values, IPipe<EventHubSendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            Task<InitializeContext<T>> messageTask = MessageInitializerCache<T>.Initialize(values, cancellationToken);
            if (messageTask.IsCompletedSuccessfully())
                return Produce(messageTask.GetAwaiter().GetResult().Message, pipe, cancellationToken);

            async Task ProduceAsync()
            {
                InitializeContext<T> context = await messageTask.ConfigureAwait(false);

                await Produce(context.Message, pipe, cancellationToken).ConfigureAwait(false);
            }

            return ProduceAsync();
        }

        public Task Produce<T>(IEnumerable<object> values, IPipe<EventHubSendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            Task<InitializeContext<T>>[] messageTasks = values
                .Select(value => MessageInitializerCache<T>.Initialize(value, cancellationToken))
                .ToArray();

            async Task ProduceAsync()
            {
                InitializeContext<T>[] contexts = await Task.WhenAll(messageTasks).ConfigureAwait(false);

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
