namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using MassTransit.Configuration;
    using Serializers;
    using Transports;


    public class KafkaTopicSendTransportContext<TKey, TValue> :
        BaseSendTransportContext,
        KafkaSendTransportContext<TKey, TValue>
        where TValue : class
    {
        readonly IHeadersSerializer _headersSerializer;
        readonly IHostConfiguration _hostConfiguration;
        readonly IAsyncSerializer<TKey> _keySerializer;
        readonly ISendPipe _sendPipe;
        readonly IProducerContextSupervisor _supervisor;
        readonly KafkaTopicAddress _topicAddress;
        readonly IAsyncSerializer<TValue> _valueSerializer;

        public KafkaTopicSendTransportContext(IHostConfiguration hostConfiguration, string topicName,
            IProducerContextSupervisor supervisor, IHeadersSerializer headersSerializer, IAsyncSerializer<TKey> keySerializer,
            IAsyncSerializer<TValue> valueSerializer, IReadOnlyList<Action<ISendPipeConfigurator>> configureSend, ISerialization serialization)
            : base(hostConfiguration, serialization)
        {
            var sendConfiguration = new SendPipeConfiguration(hostConfiguration.Topology.SendTopology);
            for (var i = 0; i < configureSend.Count; i++)
                configureSend[i](sendConfiguration.Configurator);

            _hostConfiguration = hostConfiguration;
            _supervisor = supervisor;
            _sendPipe = sendConfiguration.CreatePipe();
            _topicAddress = new KafkaTopicAddress(hostConfiguration.HostAddress, topicName);
            _headersSerializer = headersSerializer;
            _keySerializer = keySerializer;
            _valueSerializer = valueSerializer;
        }

        public override IEnumerable<IAgent> GetAgentHandles()
        {
            return new[] { _supervisor };
        }

        public async Task<KafkaSendContext<TKey, TValue>> CreateContext(TKey key, TValue value, IPipe<KafkaSendContext<TKey, TValue>> pipe,
            CancellationToken cancellationToken, IPipe<SendContext<TValue>> initializerPipe = null)
        {
            var sendContext = new KafkaMessageSendContext<TKey, TValue>(key, value, _keySerializer, _valueSerializer, cancellationToken)
            {
                DestinationAddress = _topicAddress
            };

            if (pipe is ISendContextPipe sendPipe)
                await sendPipe.Send(sendContext).ConfigureAwait(false);

            await _sendPipe.Send(sendContext).ConfigureAwait(false);

            if (initializerPipe.IsNotEmpty())
                await initializerPipe.Send(sendContext).ConfigureAwait(false);

            if (pipe.IsNotEmpty())
                await pipe.Send(sendContext).ConfigureAwait(false);

            sendContext.SourceAddress ??= _hostConfiguration.HostAddress;

            return sendContext;
        }

        public async Task Send(ProducerContext producerContext, KafkaSendContext<TKey, TValue> sendContext)
        {
            var topic = new TopicPartition(_topicAddress.Topic, sendContext.Partition);

            sendContext.SourceAddress ??= _hostConfiguration.HostAddress;
            sendContext.ConversationId ??= NewId.NextGuid();

            if (Activity.Current?.IsAllDataRequested ?? false)
            {
                Activity.Current.SetTag(nameof(sendContext.Partition), sendContext.Partition);
                Activity.Current.SetTag("TopicPartition", topic);
            }

            Message<byte[], byte[]> message = await CreateMessage(topic, sendContext).ConfigureAwait(false);

            if (sendContext.SentTime.HasValue)
                message.Timestamp = new Timestamp(sendContext.SentTime.Value);

            sendContext.CancellationToken.ThrowIfCancellationRequested();

            await producerContext.Produce(topic, message, sendContext.CancellationToken).ConfigureAwait(false);
        }

        public Task Send(IPipe<ProducerContext> pipe, CancellationToken cancellationToken)
        {
            return _hostConfiguration.Retry(() => _supervisor.Send(pipe, cancellationToken), cancellationToken, _supervisor.SendStopping);
        }

        public override string EntityName => _topicAddress.Topic;
        public override string ActivitySystem => KafkaTopicAddress.PathPrefix;

        public override Task<SendContext<T>> CreateSendContext<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            throw new NotImplementedByDesignException("Kafka is a producer, not an outbox compatible transport");
        }

        public void Probe(ProbeContext context)
        {
            _supervisor.Probe(context);
        }

        async Task<Message<byte[], byte[]>> CreateMessage(TopicPartition topic, KafkaSendContext<TKey, TValue> sendContext)
        {
            var headers = _headersSerializer.Serialize(sendContext);

            Task<byte[]> keyTask =
                sendContext.KeySerializer.SerializeAsync(sendContext.Key, new SerializationContext(MessageComponentType.Key, topic.Topic, headers));
            Task<byte[]> valueTask =
                sendContext.ValueSerializer.SerializeAsync(sendContext.Message, new SerializationContext(MessageComponentType.Value, topic.Topic, headers));

            await Task.WhenAll(keyTask, valueTask).ConfigureAwait(false);

            return new Message<byte[], byte[]>
            {
                Key = keyTask.Result,
                Value = valueTask.Result,
                Headers = headers
            };
        }
    }
}
