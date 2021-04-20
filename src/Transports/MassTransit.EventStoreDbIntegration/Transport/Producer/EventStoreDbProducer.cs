using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using GreenPipes;
using GreenPipes.Internals.Extensions;
using MassTransit.Context;
using MassTransit.EventStoreDbIntegration.Contexts;
using MassTransit.Initializers;
using MassTransit.Logging;
using MassTransit.Transports;

namespace MassTransit.EventStoreDbIntegration
{
    public class EventStoreDbProducer :
        IEventStoreDbProducer
    {
        readonly EventStoreDbSendTransportContext _context;

        public EventStoreDbProducer(EventStoreDbSendTransportContext context)
        {
            _context = context;
        }

        public Task Produce<T>(T message, CancellationToken cancellationToken = default)
            where T : class
        {
            return Produce(message, Pipe.Empty<EventStoreDbSendContext<T>>(), cancellationToken);
        }

        public Task Produce<T>(IEnumerable<T> messages, CancellationToken cancellationToken = default)
            where T : class
        {
            return Produce(messages, Pipe.Empty<EventStoreDbSendContext<T>>(), cancellationToken);
        }

        public Task Produce<T>(T message, IPipe<EventStoreDbSendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            var sendPipe = new SendPipe<T>(message, _context, pipe, cancellationToken);

            return _context.ProducerContextSupervisor.Send(sendPipe, cancellationToken);
        }

        public Task Produce<T>(IEnumerable<T> messages, IPipe<EventStoreDbSendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            var sendPipe = new BatchSendPipe<T>(messages, _context, pipe, cancellationToken);

            return _context.ProducerContextSupervisor.Send(sendPipe, cancellationToken);
        }

        public Task Produce<T>(object values, CancellationToken cancellationToken = default)
            where T : class
        {
            return Produce(values, Pipe.Empty<EventStoreDbSendContext<T>>(), cancellationToken);
        }

        public Task Produce<T>(IEnumerable<object> values, CancellationToken cancellationToken = default)
            where T : class
        {
            return Produce(values, Pipe.Empty<EventStoreDbSendContext<T>>(), cancellationToken);
        }

        public Task Produce<T>(object values, IPipe<EventStoreDbSendContext<T>> pipe, CancellationToken cancellationToken = default)
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

        public Task Produce<T>(IEnumerable<object> values, IPipe<EventStoreDbSendContext<T>> pipe, CancellationToken cancellationToken = default)
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
            readonly EventStoreDbSendTransportContext _context;
            readonly T _message;
            readonly IPipe<EventStoreDbSendContext<T>> _pipe;

            public SendPipe(T message, EventStoreDbSendTransportContext context, IPipe<EventStoreDbSendContext<T>> pipe, CancellationToken cancellationToken)
            {
                _message = message;
                _context = context;
                _pipe = pipe;
                _cancellationToken = cancellationToken;
            }

            public async Task Send(ProducerContext context)
            {
                LogContext.SetCurrentIfNull(_context.LogContext);

                var sendContext = new EventStoreDbMessageSendContext<T>(_context.EndpointAddress.StreamName, _message, _cancellationToken)
                {
                    Serializer = context.Serializer,
                    DestinationAddress = _context.EndpointAddress
                };

                await _context.SendPipe.Send(sendContext).ConfigureAwait(false);

                if (_pipe.IsNotEmpty())
                    await _pipe.Send(sendContext).ConfigureAwait(false);

                sendContext.SourceAddress ??= _context.HostAddress;
                sendContext.ConversationId ??= NewId.NextGuid();

                StartedActivity? activity = LogContext.IfEnabled(OperationName.Transport.Send)?.StartSendActivity(sendContext);
                try
                {
                    if (_context.SendObservers.Count > 0)
                        await _context.SendObservers.PreSend(sendContext).ConfigureAwait(false);

                    var eventData = new EventData(
                        Uuid.FromGuid(sendContext.MessageId.Value),
                        typeof(T).Name,
                        sendContext.Body,
                        context.HeadersSerializer.Serialize(sendContext),
                        sendContext.EventStoreDbContentType);

                    await context.Produce(sendContext.StreamName, new[] { eventData }, sendContext.CancellationToken).ConfigureAwait(false);

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
            readonly EventStoreDbSendTransportContext _context;
            readonly IEnumerable<T> _messages;
            readonly IPipe<EventStoreDbSendContext<T>> _pipe;

            public BatchSendPipe(IEnumerable<T> messages, EventStoreDbSendTransportContext context, IPipe<EventStoreDbSendContext<T>> pipe,
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

                EventStoreDbMessageSendContext<T>[] contexts = _messages
                    .Select(x => new EventStoreDbMessageSendContext<T>(_context.EndpointAddress.StreamName, x, _cancellationToken)
                    {
                        Serializer = context.Serializer,
                        DestinationAddress = _context.EndpointAddress
                    })
                    .ToArray();

                if (contexts.Length == 0)
                    return;

                NewId[] ids = NewId.Next(contexts.Length);

                async Task SendInner(EventStoreDbMessageSendContext<T> c, int idx)
                {
                    await _context.SendPipe.Send(c).ConfigureAwait(false);

                    if (_pipe.IsNotEmpty())
                        await _pipe.Send(c).ConfigureAwait(false);

                    c.SourceAddress ??= _context.HostAddress;
                    c.ConversationId ??= ids[idx].ToGuid();
                }

                await Task.WhenAll(contexts.Select(SendInner)).ConfigureAwait(false);

                var sendContext = contexts[0];

                StartedActivity? activity = LogContext.IfEnabled(OperationName.Transport.Send)?.StartSendActivity(sendContext);
                try
                {
                    if (_context.SendObservers.Count > 0)
                        await Task.WhenAll(contexts.Select(c => _context.SendObservers.PreSend(c))).ConfigureAwait(false);

                    var esdbContentType = sendContext.EventStoreDbContentType;
                    var eventTypeName = typeof(T).Name;
                    var eventDataBatch = new List<EventData>();

                    for (int i = 0; i < contexts.Length; i++)
                    {
                        var currSendContext = contexts[i];

                        eventDataBatch.Add(
                            new EventData(
                                Uuid.FromGuid(currSendContext.MessageId.Value),
                                eventTypeName,
                                currSendContext.Body,
                                context.HeadersSerializer.Serialize(currSendContext),
                                esdbContentType
                                )
                            );
                    }

                    await context.Produce(sendContext.StreamName, eventDataBatch, sendContext.CancellationToken).ConfigureAwait(false);

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
