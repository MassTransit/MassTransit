namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Threading;
    using Confluent.Kafka;
    using Context;


    public class KafkaMessageSendContext<TKey, T> :
        MessageSendContext<T>,
        KafkaSendContext<TKey, T>
        where T : class
    {
        IAsyncSerializer<TKey> _keySerializer;
        IAsyncSerializer<T> _valueSerializer;

        public KafkaMessageSendContext(TKey key, T message, IAsyncSerializer<TKey> keySerializer, IAsyncSerializer<T> valueSerializer,
            CancellationToken cancellationToken)
            : base(message, cancellationToken)
        {
            _keySerializer = keySerializer;
            _valueSerializer = valueSerializer;
            Key = key;
            Partition = Partition.Any;
        }

        public Partition Partition { get; set; }
        public TKey Key { get; set; }

        public IAsyncSerializer<TKey> KeySerializer
        {
            get => _keySerializer;
            set => _keySerializer = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IAsyncSerializer<T> ValueSerializer
        {
            get => _valueSerializer;
            set => _valueSerializer = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
