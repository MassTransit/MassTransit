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
            IPipe<ProducerContext<TKey, TValue>>
        {
            readonly CancellationToken _cancellationToken;
            readonly KafkaSendTransportContext<TKey, TValue> _context;
            readonly TKey _key;
            readonly IPipe<KafkaSendContext<TKey, TValue>> _pipe;
            readonly IPipe<SendContext<TValue>> _sendPipe;
            readonly TValue _value;

            public SendPipe(KafkaSendTransportContext<TKey, TValue> context, TKey key, TValue value, IPipe<KafkaSendContext<TKey, TValue>> pipe,
                CancellationToken cancellationToken, IPipe<SendContext<TValue>> sendPipe = null)
            {
                _context = context;
                _key = key;
                _value = value;
                _pipe = pipe;
                _cancellationToken = cancellationToken;
                _sendPipe = sendPipe;
            }

            public async Task Send(ProducerContext<TKey, TValue> context)
            {
                LogContext.SetCurrentIfNull(_context.LogContext);

                var sendContext = new KafkaMessageSendContext<TKey, TValue>(_key, _value, _cancellationToken) { DestinationAddress = _context.TopicAddress };

                await _context.SendPipe.Send(sendContext).ConfigureAwait(false);

                if (_sendPipe.IsNotEmpty())
                    await _sendPipe.Send(sendContext).ConfigureAwait(false);
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

                    var message = new Message<TKey, TValue>
                    {
                        Key = sendContext.Key,
                        Value = sendContext.Message
                    };

                    if (sendContext.SentTime.HasValue)
                        message.Timestamp = new Timestamp(sendContext.SentTime.Value);

                    message.Headers = context.HeadersSerializer.Serialize(sendContext);

                    var topic = new TopicPartition(_context.TopicAddress.Topic, sendContext.Partition);

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
