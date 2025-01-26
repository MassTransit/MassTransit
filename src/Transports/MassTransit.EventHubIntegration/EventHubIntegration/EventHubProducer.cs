namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Initializers;
    using Logging;
    using MassTransit.Middleware;
    using Transports;


    public class EventHubProducer :
        Supervisor,
        IAsyncDisposable,
        IEventHubProducer
    {
        readonly ConnectHandle _connectHandle;
        readonly EventHubSendTransportContext _context;

        public EventHubProducer(EventHubSendTransportContext context, ConnectHandle connectHandle = null)
        {
            _context = context;
            _connectHandle = connectHandle;

            foreach (var handle in _context.GetAgentHandles())
                Add(handle);
        }

        public async ValueTask DisposeAsync()
        {
            _connectHandle?.Disconnect();
            await this.Stop("Disposing Agent").ConfigureAwait(false);
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
            return _context.Send(new SendPipe<T>(message, _context, pipe, cancellationToken), cancellationToken);
        }

        public Task Produce<T>(IEnumerable<T> messages, IPipe<EventHubSendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return _context.Send(new BatchSendPipe<T>(messages, _context, pipe, cancellationToken), cancellationToken);
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

        public async Task Produce<T>(object values, IPipe<EventHubSendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            (var message, IPipe<SendContext<T>> sendPipe) = await MessageInitializerCache<T>.InitializeMessage(values, cancellationToken);

            await _context.Send(new SendPipe<T>(message, _context, pipe, cancellationToken, sendPipe), cancellationToken).ConfigureAwait(false);
        }

        public async Task Produce<T>(IEnumerable<object> values, IPipe<EventHubSendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            SendTuple<T>[] contexts = await Task.WhenAll(values.Select(value => MessageInitializerCache<T>.InitializeMessage(value, cancellationToken)))
                .ConfigureAwait(false);

            await _context.Send(new BatchSendPipe<T>(contexts.Select(x => x.Message), _context, pipe, cancellationToken, contexts.Select(x => x.Pipe)),
                cancellationToken).ConfigureAwait(false);
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
            readonly IPipe<SendContext<T>> _sendPipe;

            public SendPipe(T message, EventHubSendTransportContext context, IPipe<EventHubSendContext<T>> pipe, CancellationToken cancellationToken,
                IPipe<SendContext<T>> sendPipe = null)
            {
                _message = message;
                _context = context;
                _pipe = pipe;
                _cancellationToken = cancellationToken;
                _sendPipe = sendPipe;
            }

            public async Task Send(ProducerContext context)
            {
                LogContext.SetCurrentIfNull(_context.LogContext);

                EventHubSendContext<T> sendContext = await _context.CreateContext(_message, _pipe, _cancellationToken, _sendPipe).ConfigureAwait(false);

                sendContext.CancellationToken.ThrowIfCancellationRequested();

                StartedActivity? activity = LogContext.Current?.StartSendActivity(_context, sendContext);
                StartedInstrument? instrument = LogContext.Current?.StartSendInstrument(_context, sendContext);

                try
                {
                    if (_context.SendObservers.Count > 0)
                        await _context.SendObservers.PreSend(sendContext).ConfigureAwait(false);

                    await _context.Send(context, sendContext).ConfigureAwait(false);

                    activity?.Update(sendContext);
                    sendContext.LogSent();

                    if (_context.SendObservers.Count > 0)
                        await _context.SendObservers.PostSend(sendContext).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    sendContext.LogFaulted(exception);

                    if (_context.SendObservers.Count > 0)
                        await _context.SendObservers.SendFault(sendContext, exception).ConfigureAwait(false);

                    activity?.AddExceptionEvent(exception);
                    instrument?.AddException(exception);

                    throw;
                }
                finally
                {
                    activity?.Stop();
                    instrument?.Stop();
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
            readonly IPipe<SendContext<T>>[] _initializerPipes;
            readonly T[] _messages;
            readonly IPipe<EventHubSendContext<T>> _pipe;

            public BatchSendPipe(IEnumerable<T> messages, EventHubSendTransportContext context, IPipe<EventHubSendContext<T>> pipe,
                CancellationToken cancellationToken, IEnumerable<IPipe<SendContext<T>>> sendPipes = null)
            {
                _messages = messages as T[] ?? messages.ToArray();
                _context = context;
                _pipe = pipe;
                _initializerPipes = sendPipes as IPipe<SendContext<T>>[] ?? sendPipes?.ToArray() ?? [];
                _cancellationToken = cancellationToken;
            }

            public async Task Send(ProducerContext context)
            {
                if (_messages == null)
                    throw new ArgumentNullException(nameof(_messages));

                LogContext.SetCurrentIfNull(_context.LogContext);

                var contexts = new EventHubSendContext<T>[_messages.Length];
                if (contexts.Length == 0)
                    return;

                for (var i = 0; i < contexts.Length; i++)
                {
                    contexts[i] = await _context.CreateContext(_messages[i], _pipe, _cancellationToken,
                        _initializerPipes.Length > i ? _initializerPipes[i] : null).ConfigureAwait(false);
                }

                EventHubSendContext<T> sendContext = contexts[0];

                sendContext.CancellationToken.ThrowIfCancellationRequested();

                StartedActivity? activity = LogContext.Current?.StartSendActivity(_context, sendContext);
                try
                {
                    if (_context.SendObservers.Count > 0)
                        await Task.WhenAll(contexts.Select(c => _context.SendObservers.PreSend(c))).ConfigureAwait(false);

                    await _context.Send(context, contexts).ConfigureAwait(false);

                    activity?.Update(sendContext);
                    sendContext.LogSent();

                    if (_context.SendObservers.Count > 0)
                        await Task.WhenAll(contexts.Select(c => _context.SendObservers.PostSend(c))).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    sendContext.LogFaulted(exception);

                    if (_context.SendObservers.Count > 0)
                        await Task.WhenAll(contexts.Select(c => _context.SendObservers.SendFault(c, exception))).ConfigureAwait(false);

                    activity?.AddExceptionEvent(exception);

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
