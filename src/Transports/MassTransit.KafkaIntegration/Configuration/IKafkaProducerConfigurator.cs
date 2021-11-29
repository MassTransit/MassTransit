namespace MassTransit
{
    using System;
    using Confluent.Kafka;
    using KafkaIntegration.Serializers;


    public interface IKafkaProducerConfigurator
    {
        /// <summary>
        /// Maximum number of messages batched in one MessageSet. The total MessageSet size is also limited by message.max.bytes.
        /// default: 10000
        /// importance: medium
        /// </summary>
        int? BatchNumMessages { set; }

        /// <summary>
        /// compression codec to use for compressing message sets. This is the default value for all topics, may be overridden by the topic configuration property
        /// `compression.codec`.
        /// default: none
        /// importance: medium
        /// </summary>
        CompressionType? CompressionType { set; }

        /// <summary>
        /// The threshold of outstanding not yet transmitted broker requests needed to backpressure the producer's message accumulator. If the number of not yet
        /// transmitted requests equals or
        /// exceeds this number, produce request creation that would have otherwise been triggered (for example, in accordance with linger.ms) will be delayed. A lower
        /// number yields larger
        /// and more effective batches. A higher value can improve latency when using compression on slow machines.
        /// default: 1
        /// importance: low
        /// </summary>
        int? QueueBufferingBackpressureThreshold { set; }

        /// <summary>
        /// The backoff time before retrying a protocol request.
        /// default: 100 ms
        /// importance: medium
        /// </summary>
        TimeSpan? RetryBackoff { set; }

        /// <summary>
        /// How many times to retry sending a failing Message. **Note:** retrying may cause reordering unless `enable.idempotence` is set to true.
        /// default: 2
        /// importance: high
        /// </summary>
        int? MessageSendMaxRetries { set; }

        /// <summary>
        /// Delay to wait for messages in the producer queue to accumulate before constructing message batches (MessageSets) to transmit to brokers. A higher value allows
        /// larger and more
        /// effective (less overhead, improved compression) batches of messages to accumulate at the expense of increased message delivery latency.
        /// default: 0.5
        /// importance: high
        /// </summary>
        TimeSpan? Linger { set; }

        /// <summary>
        /// Maximum total message size sum allowed on the producer queue. This queue is shared by all topics and partitions. This property has higher priority than
        /// queue.buffering.max.messages.
        /// default: 1048576
        /// importance: high
        /// </summary>
        int? QueueBufferingMaxKbytes { set; }

        /// <summary>
        /// Maximum number of messages allowed on the producer queue. This queue is shared by all topics and partitions.
        /// default: 100000
        /// importance: high
        /// </summary>
        int? QueueBufferingMaxMessages { set; }

        /// <summary>
        /// **EXPERIMENTAL**: subject to change or removal. When set to `true`, any error that could result in a gap in the produced message series when a batch of
        /// messages fails, will raise
        /// a fatal error (ERR__GAPLESS_GUARANTEE) and stop the producer. Messages failing due to `message.timeout.ms` are not covered by this guarantee. Requires
        /// `enable.idempotence=true`.
        /// default: false
        /// importance: low
        /// </summary>
        bool? EnableGaplessGuarantee { set; }

        /// <summary>
        /// When set to `true`, the producer will ensure that messages are successfully produced exactly once and in the original produce order. The following
        /// configuration properties are
        /// adjusted automatically (if not modified by the user) when idempotence is enabled: `max.in.flight.requests.per.connection=5` (must be less than or equal to 5),
        /// `retries=INT32_MAX`
        /// (must be greater than 0), `acks=all`, `queuing.strategy=fifo`. Producer instantation will fail if user-supplied configuration is incompatible.
        /// default: false
        /// importance: high
        /// </summary>
        bool? EnableIdempotence { set; }

        /// <summary>
        /// The maximum amount of time in milliseconds that the transaction coordinator will wait for a transaction status update from the producer before proactively
        /// aborting the ongoing
        /// transaction. If this value is larger than the `transaction.max.timeout.ms` setting in the broker, the init_transactions() call will fail with
        /// ERR_INVALID_TRANSACTION_TIMEOUT. The
        /// transaction timeout automatically adjusts `message.timeout.ms` and `socket.timeout.ms`, unless explicitly configured in which case they must not exceed the
        /// transaction timeout
        /// (`socket.timeout.ms` must be at least 100ms lower than `transaction.timeout.ms`). This is also the default timeout value if no timeout (-1) is supplied to the
        /// transactional API
        /// methods.
        /// default: 60000
        /// importance: medium
        /// </summary>
        TimeSpan? TransactionTimeout { set; }

        /// <summary>
        /// Enables the transactional producer. The transactional.id is used to identify the same transactional producer instance across process restarts. It allows the
        /// producer to guarantee
        /// that transactions corresponding to earlier instances of the same producer have been finalized prior to starting any new transactions, and that any zombie
        /// instances are fenced off.
        /// If no transactional.id is provided, then the producer is limited to idempotent delivery (if enable.idempotence is set). Requires broker version &gt;= 0.11.0.
        /// default: ''
        /// importance: high
        /// </summary>
        string TransactionalId { set; }

        /// <summary>
        /// Compression level parameter for algorithm selected by configuration property `compression.codec`. Higher values will result in better compression at the cost
        /// of more CPU usage.
        /// Usable range is algorithm-dependent: [0-9] for gzip; [0-12] for lz4; only 0 for snappy; -1 = codec-dependent default compression level.
        /// default: -1
        /// importance: medium
        /// </summary>
        int? CompressionLevel { set; }

        /// <summary>
        /// Partitioner: `random` - random distribution, `consistent` - CRC32 hash of key (Empty and NULL keys are mapped to single partition), `consistent_random` - CRC32
        /// hash of key (Empty
        /// and NULL keys are randomly partitioned), `murmur2` - Java Producer compatible Murmur2 hash of key (NULL keys are mapped to single partition), `murmur2_random`
        /// - Java Producer
        /// compatible Murmur2 hash of key (NULL keys are randomly partitioned. This is functionally equivalent to the default partitioner in the Java Producer.), `fnv1a`
        /// - FNV-1a hash of key
        /// (NULL keys are mapped to single partition), `fnv1a_random` - FNV-1a hash of key (NULL keys are randomly partitioned).
        /// default: consistent_random
        /// importance: high
        /// </summary>
        Partitioner? Partitioner { set; }

        /// <summary>
        /// Local message timeout. This value is only enforced locally and limits the time a produced message waits for successful delivery. A time of 0 is infinite. This
        /// is the maximum time
        /// librdkafka may use to deliver a message (including retries). Delivery error occurs when either the retry count or the message timeout are exceeded. The message
        /// timeout is
        /// automatically adjusted to `transaction.timeout.ms` if `transactional.id` is configured.
        /// default: 300000
        /// importance: high
        /// </summary>
        TimeSpan? MessageTimeout { set; }

        /// <summary>
        /// The ack timeout of the producer request in milliseconds. This value is only enforced by the broker and relies on `request.required.acks` being != 0.
        /// default: 5000
        /// importance: medium
        /// </summary>
        TimeSpan? RequestTimeout { set; }

        /// <summary>
        /// A comma separated list of fields that may be optionally set in delivery
        /// reports. Disabling delivery report fields that you do not require will
        /// improve maximum throughput and reduce memory usage. Allowed values:
        /// key, value, timestamp, headers, all, none.
        /// default: all
        /// importance: low
        /// </summary>
        string DeliveryReportFields { set; }

        /// <summary>
        /// Specifies whether to enable notification of delivery reports. Typically
        /// you should set this parameter to true. Set it to false for "fire and
        /// forget" semantics and a small boost in performance.
        /// default: true
        /// importance: low
        /// </summary>
        bool? EnableDeliveryReports { set; }

        /// <summary>
        /// Specifies whether or not the producer should start a background poll
        /// thread to receive delivery reports and event notifications. Generally,
        /// this should be set to true. If set to false, you will need to call
        /// the Poll function manually.
        /// default: true
        /// importance: low
        /// </summary>
        bool? EnableBackgroundPoll { set; }

        /// <summary>
        /// Set the serializer to use to serialize headers.
        /// </summary>
        /// <param name="serializer"></param>
        void SetHeadersSerializer(IHeadersSerializer serializer);
    }


    public interface IKafkaProducerConfigurator<TKey, TValue> :
        IKafkaProducerConfigurator
    {
        /// <summary>Set the serializer to use to serialize keys.</summary>
        /// <remarks>
        /// If your key serializer throws an exception, this will be
        /// wrapped in a ConsumeException with ErrorCode
        /// Local_KeyDeserialization and thrown by the initiating call to
        /// Consume.
        /// </remarks>
        void SetKeySerializer(ISerializer<TKey> serializer);

        /// <summary>
        /// Set the serializer to use to serialize values.
        /// </summary>
        /// <remarks>
        /// If your value serializer throws an exception, this will be
        /// wrapped in a ConsumeException with ErrorCode
        /// Local_ValueDeserialization and thrown by the initiating call to
        /// Consume.
        /// </remarks>
        void SetValueSerializer(ISerializer<TValue> serializer);
    }
}
