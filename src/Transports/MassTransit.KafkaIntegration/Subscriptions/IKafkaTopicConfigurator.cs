namespace MassTransit.KafkaIntegration.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Confluent.Kafka;
    using Serializers;


    public interface IKafkaTopicConfigurator :
        IReceiveEndpointConfigurator
    {
        /// <summary>
        ///     Action to take when there is no initial offset in offset store or the desired offset is out of range: 'smallest','earliest' - automatically reset the offset to the smallest
        ///     offset, 'largest','latest' - automatically reset the offset to the largest offset, 'error' - trigger an error which is retrieved by consuming messages and checking 'message-
        ///     &gt;err'.
        ///     default: largest
        ///     importance: high
        /// </summary>
        AutoOffsetReset? AutoOffsetReset { set; }

        /// <summary>
        ///     Enable static group membership. Static group members are able to leave and rejoin a group within the configured `session.timeout.ms` without prompting a group rebalance. This
        ///     should be used in combination with a larger `session.timeout.ms` to avoid group rebalances caused by transient unavailability (e.g. process restarts). Requires broker version
        ///     &gt;= 2.3.0.
        ///     default: ''
        ///     importance: medium
        /// </summary>
        string GroupInstanceId { set; }

        /// <summary>
        ///     Name of partition assignment strategy to use when elected group leader assigns partitions to group members.
        ///     default: range,roundrobin
        ///     importance: medium
        /// </summary>
        PartitionAssignmentStrategy? PartitionAssignmentStrategy { set; }

        /// <summary>
        ///     Client group session and failure detection timeout. The consumer sends periodic heartbeats (heartbeat.interval.ms) to indicate its liveness to the broker. If no hearts are
        ///     received by the broker for a group member within the session timeout, the broker will remove the consumer from the group and trigger a rebalance. The allowed range is
        ///     configured with the **broker** configuration properties `group.min.session.timeout.ms` and `group.max.session.timeout.ms`. Also see `max.poll.interval.ms`.
        ///     default: 10000
        ///     importance: high
        /// </summary>
        TimeSpan? SessionTimeout { set; }

        /// <summary>
        ///     Group session keepalive heartbeat interval.
        ///     default: 3000
        ///     importance: low
        /// </summary>
        TimeSpan? HeartbeatInterval { set; }

        /// <summary>
        ///     Group protocol type
        ///     default: consumer
        ///     importance: low
        /// </summary>
        string GroupProtocolType { set; }

        /// <summary>
        ///     How often to query for the current client group coordinator. If the currently assigned coordinator is down the configured query interval will be divided by ten to more quickly
        ///     recover in case of coordinator reassignment.
        ///     default: 600000
        ///     importance: low
        /// </summary>
        TimeSpan? CoordinatorQueryInterval { set; }

        /// <summary>
        ///     Maximum allowed time between calls to consume messages (e.g., rd_kafka_consumer_poll()) for high-level consumers. If this interval is exceeded the consumer is considered
        ///     failed and the group will rebalance in order to reassign the partitions to another consumer group member. Warning: Offset commits may be not possible at this point. Note: It
        ///     is recommended to set `enable.auto.offset.store=false` for long-time processing applications and then explicitly store offsets (using offsets_store()) *after* message
        ///     processing, to make sure offsets are not auto-committed prior to processing has finished. The interval is checked two times per second. See KIP-62 for more information.
        ///     default: 300000
        ///     importance: high
        /// </summary>
        TimeSpan? MaxPollInterval { set; }

        /// <summary>
        ///     Automatically store offset of last message provided to application. The offset store is an in-memory store of the next offset to (auto-)commit for each partition.
        ///     default: true
        ///     importance: high
        /// </summary>
        bool? EnableAutoOffsetStore { set; }

        /// <summary>
        ///     Minimum number of messages per topic+partition librdkafka tries to maintain in the local consumer queue.
        ///     default: 100000
        ///     importance: medium
        /// </summary>
        int? QueuedMinMessages { set; }

        /// <summary>
        ///     Maximum number of kilobytes per topic+partition in the local consumer queue. This value may be overshot by fetch.message.max.bytes. This property has higher priority than
        ///     queued.min.messages.
        ///     default: 1048576
        ///     importance: medium
        /// </summary>
        int? QueuedMaxMessagesKbytes { set; }

        /// <summary>
        ///     Emit RD_KAFKA_RESP_ERR__PARTITION_EOF event whenever the consumer reaches the end of a partition.
        ///     default: false
        ///     importance: low
        /// </summary>
        bool? EnablePartitionEof { set; }

        /// <summary>
        ///     Verify CRC32 of consumed messages, ensuring no on-the-wire or on-disk corruption to the messages occurred. This check comes at slightly increased CPU usage.
        ///     default: false
        ///     importance: medium
        /// </summary>
        bool? CheckCrcs { set; }

        void DisableAutoCommit();

        /// <summary>
        ///     Automatically and periodically commit offsets in the background. Note: setting this to false does not prevent the consumer from fetching previously committed start offsets. To
        ///     circumvent this behaviour set specific start offsets per partition in the call to assign().
        ///     default: true
        ///     importance: high
        /// </summary>
        void EnableAutoCommit(TimeSpan interval);

        void ConfigureFetch(Action<IKafkaFetchConfigurator> configure);

        /// <summary>
        ///     Controls how to read messages written transactionally: `read_committed` - only return transactional messages which have been committed. `read_uncommitted` - return all
        ///     messages, even transactional messages which have been aborted.
        ///     default: read_committed
        ///     importance: high
        /// </summary>
        void UseIsolationLevel(IsolationLevel isolationLevel);
    }


    public interface IKafkaTopicConfigurator<TKey, TValue> :
        IKafkaTopicConfigurator
        where TValue : class
    {
        /// <summary>Set the deserializer to use to deserialize keys.</summary>
        /// <remarks>
        ///     If your key deserializer throws an exception, this will be
        ///     wrapped in a ConsumeException with ErrorCode
        ///     Local_KeyDeserialization and thrown by the initiating call to
        ///     Consume.
        /// </remarks>
        void SetKeyDeserializer(IDeserializer<TKey> deserializer);

        /// <summary>
        ///     Set the deserializer to use to deserialize values.
        /// </summary>
        /// <remarks>
        ///     If your value deserializer throws an exception, this will be
        ///     wrapped in a ConsumeException with ErrorCode
        ///     Local_ValueDeserialization and thrown by the initiating call to
        ///     Consume.
        /// </remarks>
        void SetValueDeserializer(IDeserializer<TValue> deserializer);

        /// <summary>
        ///     Set the deserializer to use to deserialize headers.
        /// </summary>
        /// <param name="deserializer"></param>
        void SetHeadersDeserializer(IHeadersDeserializer deserializer);

        /// <summary>
        ///     This handler is called when a new consumer group partition assignment has been received
        ///     by this consumer.
        ///     Note: corresponding to every call to this handler there will be a corresponding call to
        ///     the partitions revoked handler (if one has been set using SetPartitionsRevokedHandler).
        ///     The actual partitions to consume from and start offsets are specified by the return value
        ///     of the handler. This set of partitions is not required to match the assignment provided
        ///     by the consumer group, but typically will. Partition offsets may be a specific offset, or
        ///     special value (Beginning, End or Unset). If Unset, consumption will resume from the
        ///     last committed offset for each partition, or if there is no committed offset, in accordance
        ///     with the `auto.offset.reset` configuration property.
        /// </summary>
        /// <remarks>
        ///     May execute as a side-effect of the Consumer.Consume call (on the same thread).
        ///     Assign/Unassign must not be called in the handler.
        ///     Exceptions: Any exception thrown by your partitions assigned handler
        ///     will be wrapped in a ConsumeException with ErrorCode
        ///     ErrorCode.Local_Application and thrown by the initiating call to Consume.
        /// </remarks>
        void SetPartitionsAssignedHandler(Func<IConsumer<TKey, TValue>, List<TopicPartition>, IEnumerable<TopicPartitionOffset>> partitionsAssignedHandler);

        /// <summary>
        ///     This handler is called when a new consumer group partition assignment has been received
        ///     by this consumer.
        ///     Note: corresponding to every call to this handler there will be a corresponding call to
        ///     the partitions revoked handler (if one has been set using SetPartitionsRevokedHandler").
        ///     Consumption will resume from the last committed offset for each partition, or if there is
        ///     no committed offset, in accordance with the `auto.offset.reset` configuration property.
        /// </summary>
        /// <remarks>
        ///     May execute as a side-effect of the Consumer.Consume call (on the same thread).
        ///     Assign/Unassign must not be called in the handler.
        ///     Exceptions: Any exception thrown by your partitions assigned handler
        ///     will be wrapped in a ConsumeException with ErrorCode
        ///     ErrorCode.Local_Application and thrown by the initiating call to Consume.
        /// </remarks>
        void SetPartitionsAssignedHandler(Action<IConsumer<TKey, TValue>, List<TopicPartition>> partitionAssignmentHandler);

        /// <summary>
        ///     This handler is called immediately prior to a group partition assignment being
        ///     revoked. The second parameter provides the set of partitions the consumer is
        ///     currently assigned to, and the current position of the consumer on each of these
        ///     partitions.
        /// </summary>
        /// <remarks>
        ///     May execute as a side-effect of the Consumer.Consume call (on the same thread).
        ///     Assign/Unassign must not be called in the handler.
        ///     Exceptions: Any exception thrown by your partitions revoked handler
        ///     will be wrapped in a ConsumeException with ErrorCode
        ///     ErrorCode.Local_Application and thrown by the initiating call to Consume/Close.
        /// </remarks>
        void SetPartitionsRevokedHandler(Func<IConsumer<TKey, TValue>, List<TopicPartitionOffset>, IEnumerable<TopicPartitionOffset>> partitionsRevokedHandler);

        /// <summary>
        ///     This handler is called immediately prior to a group partition assignment being
        ///     revoked. The second parameter provides the set of partitions the consumer is
        ///     currently assigned to, and the current position of the consumer on each of these
        ///     partitions.
        ///     The return value of the handler specifies the partitions/offsets the consumer
        ///     should be assigned to following completion of this method (typically empty).
        /// </summary>
        /// <remarks>
        ///     May execute as a side-effect of the Consumer.Consume call (on the same thread).
        ///     Assign/Unassign must not be called in the handler.
        ///     Exceptions: Any exception thrown by your partitions revoked handler
        ///     will be wrapped in a ConsumeException with ErrorCode
        ///     ErrorCode.Local_Application and thrown by the initiating call to Consume.
        /// </remarks>
        void SetPartitionsRevokedHandler(Action<IConsumer<TKey, TValue>, List<TopicPartitionOffset>> partitionsRevokedHandler);

        /// <summary>
        ///     A handler that is called to report the result of (automatic) offset
        ///     commits. It is not called as a result of the use of the Commit method.
        /// </summary>
        /// <remarks>
        ///     Executes as a side-effect of the Consumer.Consume call (on the same thread).
        ///     Exceptions: Any exception thrown by your offsets committed handler
        ///     will be wrapped in a ConsumeException with ErrorCode
        ///     ErrorCode.Local_Application and thrown by the initiating call to Consume/Close.
        /// </remarks>
        void SetOffsetsCommittedHandler(Action<IConsumer<TKey, TValue>, CommittedOffsets> offsetsCommittedHandler);
    }
}
