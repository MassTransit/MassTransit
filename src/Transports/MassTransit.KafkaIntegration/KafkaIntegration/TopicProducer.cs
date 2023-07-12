namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Initializers;
    using Logging;
    using MassTransit.Middleware;
    using Transports;


    public class TopicProducer<TKey, TValue> :
        Supervisor,
        IAsyncDisposable,
        ITopicProducer<TKey, TValue>
        where TValue : class
    {
        readonly ConnectHandle _connectHandle;
        readonly KafkaSendTransportContext<TKey, TValue> _context;

        public TopicProducer(KafkaSendTransportContext<TKey, TValue> context, ConnectHandle connectHandle = null)
        {
            _context = context;
            _connectHandle = connectHandle;

            foreach (var handle in _context.GetAgentHandles())
                Add(handle);
        }

        public async ValueTask DisposeAsync()
        {
            _connectHandle?.Disconnect();
            await this.Stop("Disposing Producer").ConfigureAwait(false);
        }

        public Task Produce(TKey key, TValue value, CancellationToken cancellationToken = default)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return Produce(key, value, Pipe.Empty<KafkaSendContext<TKey, TValue>>(), cancellationToken);
        }

        public Task Produce(TKey key, TValue value, IPipe<KafkaSendContext<TKey, TValue>> pipe, CancellationToken cancellationToken)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return _context.Send(new SendPipe(_context, key, value, pipe, cancellationToken), cancellationToken);
        }

        public Task Produce(TKey key, object values, CancellationToken cancellationToken = default)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return Produce(key, values, Pipe.Empty<KafkaSendContext<TKey, TValue>>(), cancellationToken);
        }

        public async Task Produce(TKey key, object values, IPipe<KafkaSendContext<TKey, TValue>> pipe, CancellationToken cancellationToken = default)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            (var message, IPipe<SendContext<TValue>> sendPipe) = await MessageInitializerCache<TValue>.InitializeMessage(values, cancellationToken);

            await _context.Send(new SendPipe(_context, key, message, pipe, cancellationToken, sendPipe), cancellationToken)
                .ConfigureAwait(false);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _context.ConnectSendObserver(observer);
        }


        class SendPipe :
            IPipe<ProducerContext>
        {
            readonly CancellationToken _cancellationToken;
            readonly KafkaSendTransportContext<TKey, TValue> _context;
            readonly IPipe<SendContext<TValue>> _initializerPipe;
            readonly TKey _key;
            readonly IPipe<KafkaSendContext<TKey, TValue>> _pipe;
            readonly TValue _value;

            public SendPipe(KafkaSendTransportContext<TKey, TValue> context, TKey key, TValue value, IPipe<KafkaSendContext<TKey, TValue>> pipe,
                CancellationToken cancellationToken, IPipe<SendContext<TValue>> initializerPipe = null)
            {
                _context = context;
                _key = key;
                _value = value;
                _pipe = pipe;
                _cancellationToken = cancellationToken;
                _initializerPipe = initializerPipe;
            }

            public async Task Send(ProducerContext context)
            {
                LogContext.SetCurrentIfNull(_context.LogContext);

                KafkaSendContext<TKey, TValue> sendContext =
                    await _context.CreateContext(_key, _value, _pipe, _cancellationToken, _initializerPipe).ConfigureAwait(false);

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
    }
}
