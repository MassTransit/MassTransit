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
        readonly EventHubSendTransportContext _context;

        public EventHubProducer(EventHubSendTransportContext context)
        {
            _context = context;
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

        public Task Produce<T>(T message, IPipe<EventHubSendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            var sendPipe = new SendPipe<T>(message, _context, pipe, cancellationToken);
            return _context.Send(sendPipe, cancellationToken);
        }

        public Task Produce<T>(IEnumerable<T> messages, IPipe<EventHubSendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            var sendPipe = new BatchSendPipe<T>(messages, _context, pipe, cancellationToken);
            return _context.Send(sendPipe, cancellationToken);
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
            return _context.ConnectSendObserver(observer);
        }


        class SendPipe<T> :
            IPipe<ProducerContext>
            where T : class
        {
            readonly CancellationToken _cancellationToken;
            readonly EventHubSendTransportContext _context;
            readonly T _message;
            readonly IPipe<EventHubSendContext<T>> _pipe;

            public SendPipe(T message, EventHubSendTransportContext context, IPipe<EventHubSendContext<T>> pipe, CancellationToken cancellationToken)
            {
                _message = message;
                _context = context;
                _pipe = pipe;
                _cancellationToken = cancellationToken;
            }

            public async Task Send(ProducerContext context)
            {
                LogContext.SetCurrentIfNull(_context.LogContext);

                var sendContext = new EventHubMessageSendContext<T>(_message, _cancellationToken)
                {
                    Serializer = context.Serializer,
                    DestinationAddress = _context.EndpointAddress
                };

                await _context.SendPipe.Send(sendContext).ConfigureAwait(false);

                if (_pipe.IsNotEmpty())
                    await _pipe.Send(sendContext).ConfigureAwait(false);

                sendContext.SourceAddress ??= _context.HostAddress;
                sendContext.ConversationId ??= NewId.NextGuid();

                var options = new SendEventOptions
                {
                    PartitionId = sendContext.PartitionId,
                    PartitionKey = sendContext.PartitionKey
                };

                StartedActivity? activity = LogContext.IfEnabled(OperationName.Transport.Send)?.StartSendActivity(sendContext,
                    (nameof(sendContext.PartitionId), options.PartitionId), (nameof(sendContext.PartitionKey), options.PartitionKey));
                try
                {
                    if (_context.SendObservers.Count > 0)
                        await _context.SendObservers.PreSend(sendContext).ConfigureAwait(false);

                    var eventData = new EventData(sendContext.Body);

                    eventData.Properties.Set(sendContext.Headers);

                    await context.Produce(new[] { eventData }, options, sendContext.CancellationToken).ConfigureAwait(false);

                    sendContext.LogSent();
                    activity.AddSendContextHeadersPostSend(sendContext);

                    if (_context.SendObservers.Count > 0)
                        await _context.SendObservers.PostSend(sendContext).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    sendContext.LogFaulted(exception);

                    if (_context.SendObservers.Count > 0)
                        await _context.SendObservers.SendFault(sendContext, exception).ConfigureAwait(false);

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
        }


        class BatchSendPipe<T> :
            IPipe<ProducerContext>
            where T : class
        {
            readonly CancellationToken _cancellationToken;
            readonly EventHubSendTransportContext _context;
            readonly IEnumerable<T> _messages;
            readonly IPipe<EventHubSendContext<T>> _pipe;

            public BatchSendPipe(IEnumerable<T> messages, EventHubSendTransportContext context, IPipe<EventHubSendContext<T>> pipe,
                CancellationToken cancellationToken)
            {
                _messages = messages;
                _context = context;
                _pipe = pipe;
                _cancellationToken = cancellationToken;
            }

            public async Task Send(ProducerContext context)
            {
                if (_messages == null)
                    throw new ArgumentNullException(nameof(_messages));

                LogContext.SetCurrentIfNull(_context.LogContext);

                EventHubMessageSendContext<T>[] contexts = _messages
                    .Select(x => new EventHubMessageSendContext<T>(x, _cancellationToken) { Serializer = context.Serializer })
                    .ToArray();

                if (contexts.Length == 0)
                    return;

                NewId[] ids = NewId.Next(contexts.Length);

                async Task SendInner(EventHubMessageSendContext<T> c, int idx)
                {
                    c.DestinationAddress = _context.EndpointAddress;

                    await _context.SendPipe.Send(c).ConfigureAwait(false);

                    if (_pipe.IsNotEmpty())
                        await _pipe.Send(c).ConfigureAwait(false);

                    c.SourceAddress ??= _context.HostAddress;
                    c.ConversationId ??= ids[idx].ToGuid();
                }

                await Task.WhenAll(contexts.Select(SendInner)).ConfigureAwait(false);

                EventHubMessageSendContext<T> sendContext = contexts[0];
                var options = new CreateBatchOptions
                {
                    PartitionId = sendContext.PartitionId,
                    PartitionKey = sendContext.PartitionKey
                };

                StartedActivity? activity = LogContext.IfEnabled(OperationName.Transport.Send)?.StartSendActivity(sendContext,
                    (nameof(EventHubMessageSendContext<T>.PartitionId), options.PartitionId),
                    (nameof(EventHubMessageSendContext<T>.PartitionKey), options.PartitionKey));
                try
                {
                    var eventDataBatch = await context.CreateBatch(options, context.CancellationToken).ConfigureAwait(false);

                    if (_context.SendObservers.Count > 0)
                        await Task.WhenAll(contexts.Select(c => _context.SendObservers.PreSend(c))).ConfigureAwait(false);

                    async Task FlushAsync(EventDataBatch batch)
                    {
                        await context.Produce(batch, _cancellationToken).ConfigureAwait(false);
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

                            if (contexts.Length - i > 1)
                                eventDataBatch = await context.CreateBatch(options, _cancellationToken).ConfigureAwait(false);
                        }
                    }

                    if (eventDataBatch.Count > 0)
                        await FlushAsync(eventDataBatch);

                    sendContext.LogSent();
                    activity.AddSendContextHeadersPostSend(sendContext);

                    if (_context.SendObservers.Count > 0)
                        await Task.WhenAll(contexts.Select(c => _context.SendObservers.PostSend(c))).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    sendContext.LogFaulted(exception);

                    if (_context.SendObservers.Count > 0)
                        await Task.WhenAll(contexts.Select(c => _context.SendObservers.SendFault(c, exception))).ConfigureAwait(false);

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
        }
    }
}
