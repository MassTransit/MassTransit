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
            //var sendPipe = new BatchSendPipe<T>(messages, _context, pipe, cancellationToken);
            //return _context.ProducerContextSupervisor.Send(sendPipe, cancellationToken);
            throw new NotImplementedException();
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

                var sendContext = new EventStoreDbMessageSendContext<T>(_message, _cancellationToken)
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

                    //var eventData = new EventData(sendContext.Body);

                    //eventData.Properties.Set(sendContext.Headers);

                    //await context.Produce(new[] { eventData }, options, sendContext.CancellationToken).ConfigureAwait(false);

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
    }
}
