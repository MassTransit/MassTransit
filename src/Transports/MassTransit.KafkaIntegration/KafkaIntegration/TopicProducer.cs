namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Initializers;
    using Logging;
    using Transports;


    public class TopicProducer<TKey, TValue> :
        ITopicProducer<TKey, TValue>
        where TValue : class
    {
        readonly KafkaSendTransportContext<TKey, TValue> _context;

        public TopicProducer(KafkaSendTransportContext<TKey, TValue> context)
        {
            _context = context;
        }

        public Task Produce(TKey key, TValue value, CancellationToken cancellationToken = default)
        {
            return Produce(key, value, Pipe.Empty<KafkaSendContext<TKey, TValue>>(), cancellationToken);
        }

        public Task Produce(TKey key, TValue value, IPipe<KafkaSendContext<TKey, TValue>> pipe, CancellationToken cancellationToken)
        {
            return _context.Send(new SendPipe(_context, key, value, pipe, cancellationToken), cancellationToken);
        }

        public Task Produce(TKey key, object values, CancellationToken cancellationToken = default)
        {
            return Produce(key, values, Pipe.Empty<KafkaSendContext<TKey, TValue>>(), cancellationToken);
        }

        public async Task Produce(TKey key, object values, IPipe<KafkaSendContext<TKey, TValue>> pipe, CancellationToken cancellationToken = default)
        {
            (var message, IPipe<SendContext<TValue>> sendPipe) = await MessageInitializerCache<TValue>.InitializeMessage(values, cancellationToken);

            await _context.Send(new SendPipe(_context, key, message, pipe, cancellationToken, sendPipe), cancellationToken).ConfigureAwait(false);
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
            readonly TKey _key;
            readonly IPipe<KafkaSendContext<TKey, TValue>> _pipe;
            readonly ISendContextPipe _sendContextPipe;
            readonly IPipe<SendContext<TValue>> _initializerPipe;
            readonly TValue _value;

            public SendPipe(KafkaSendTransportContext<TKey, TValue> context, TKey key, TValue value, IPipe<KafkaSendContext<TKey, TValue>> pipe,
                CancellationToken cancellationToken, IPipe<SendContext<TValue>> initializerPipe = null)
            {
                _context = context;
                _key = key;
                _value = value;
                _pipe = pipe;
                _sendContextPipe = pipe as ISendContextPipe;

                _cancellationToken = cancellationToken;
                _initializerPipe = initializerPipe;
            }

            public async Task Send(ProducerContext context)
            {
                LogContext.SetCurrentIfNull(_context.LogContext);

                var sendContext = new KafkaMessageSendContext<TKey, TValue>(_key, _value, _cancellationToken) { DestinationAddress = _context.TopicAddress };

                if (_sendContextPipe != null)
                    await _sendContextPipe.Send(sendContext).ConfigureAwait(false);

                await _context.SendPipe.Send(sendContext).ConfigureAwait(false);

                if (_initializerPipe.IsNotEmpty())
                    await _initializerPipe.Send(sendContext).ConfigureAwait(false);

                if (_pipe.IsNotEmpty())
                    await _pipe.Send(sendContext).ConfigureAwait(false);

                sendContext.SourceAddress ??= _context.HostAddress;
                sendContext.ConversationId ??= NewId.NextGuid();

                StartedActivity? activity = LogContext.Current?.StartSendActivity(_context, sendContext,
                    (nameof(sendContext.Partition), sendContext.Partition.ToString()));
                try
                {
                    if (_context.SendObservers.Count > 0)
                        await _context.SendObservers.PreSend(sendContext).ConfigureAwait(false);

                    var topic = new TopicPartition(_context.TopicAddress.Topic, sendContext.Partition);
                    Message<byte[], byte[]> message = await CreateMessage(topic, sendContext).ConfigureAwait(false);

                    if (sendContext.SentTime.HasValue)
                        message.Timestamp = new Timestamp(sendContext.SentTime.Value);

                    await context.Produce(topic, message, sendContext.CancellationToken).ConfigureAwait(false);

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

                    throw;
                }
                finally
                {
                    activity?.Stop();
                }
            }

            async Task<Message<byte[], byte[]>> CreateMessage(TopicPartition topic, KafkaSendContext<TKey, TValue> sendContext)
            {
                var headers = _context.HeadersSerializer.Serialize(sendContext);

                Task<byte[]> keyTask = _context.KeySerializer
                    .SerializeAsync(sendContext.Key, new SerializationContext(MessageComponentType.Key, topic.Topic, headers));
                Task<byte[]> valueTask = _context.ValueSerializer
                    .SerializeAsync(sendContext.Message, new SerializationContext(MessageComponentType.Value, topic.Topic, headers));

                await Task.WhenAll(keyTask, valueTask).ConfigureAwait(false);

                return new Message<byte[], byte[]>
                {
                    Key = keyTask.Result,
                    Value = valueTask.Result,
                    Headers = headers
                };
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }
}
