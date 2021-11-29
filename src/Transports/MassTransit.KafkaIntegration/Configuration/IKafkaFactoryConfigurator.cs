namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;
    using KafkaIntegration.Serializers;


    public interface IKafkaFactoryConfigurator :
        IRiderFactoryConfigurator,
        ISendObserverConnector,
        ISendPipelineConfigurator
    {
        /// <summary>
        /// This field indicates the number of acknowledgements the leader broker must receive from ISR brokers
        /// before responding to the request: Zero=Broker does not send any response/ack to client, One=The
        /// leader will write the record to its local log but will respond without awaiting full acknowledgement
        /// from all followers. All=Broker will block until message is committed by all in sync replicas (ISRs).
        /// If there are less than min.insync.replicas (broker configuration) in the ISR set the produce request
        /// will fail.
        /// </summary>
        Acks? Acks { set; }

        /// <summary>
        /// Client identifier.
        /// default: rdkafka
        /// importance: low
        /// </summary>
        string ClientId { set; }

        /// <summary>
        /// Maximum Kafka protocol request message size. Due to differing framing overhead between protocol versions the producer is unable to reliably enforce a strict
        /// max message limit
        /// at produce time and may exceed the maximum size by one message in protocol ProduceRequests, the broker will enforce the the topic's `max.message.bytes` limit
        /// (see Apache Kafka
        /// documentation).
        /// default: 1000000
        /// importance: medium
        /// </summary>
        int? MessageMaxBytes { set; }

        /// <summary>
        /// Maximum size for message to be copied to buffer. Messages larger than this will be passed by reference (zero-copy) at the expense of larger iovecs.
        /// default: 65535
        /// importance: low
        /// </summary>
        int? MessageCopyMaxBytes { set; }

        /// <summary>
        /// Maximum Kafka protocol response message size. This serves as a safety precaution to avoid memory exhaustion in case of protocol hickups. This value must be at
        /// least
        /// `fetch.max.bytes`  + 512 to allow for protocol overhead; the value is adjusted automatically unless the configuration property is explicitly set.
        /// default: 100000000
        /// importance: medium
        /// </summary>
        int? ReceiveMessageMaxBytes { set; }

        /// <summary>
        /// Maximum number of in-flight requests per broker connection. This is a generic property applied to all broker communication, however it is primarily relevant to
        /// produce
        /// requests. In particular, note that other mechanisms limit the number of outstanding consumer fetch request per broker to one.
        /// default: 1000000
        /// importance: low
        /// </summary>
        int? MaxInFlight { set; }

        /// <summary>
        /// Period of time in milliseconds at which topic and broker metadata is refreshed in order to proactively discover any new brokers, topics, partitions or
        /// partition leader
        /// changes. Use -1 to disable the intervalled refresh (not recommended). If there are no locally referenced topics (no topic objects created, no messages
        /// produced, no
        /// subscription or no assignment) then only the broker list will be refreshed every interval but no more often than every 10s.
        /// default: 300000
        /// importance: low
        /// </summary>
        TimeSpan? TopicMetadataRefreshInterval { set; }

        /// <summary>
        /// Metadata cache max age. Defaults to topic.metadata.refresh.interval.ms * 3
        /// default: 900000
        /// importance: low
        /// </summary>
        TimeSpan? MetadataMaxAge { set; }

        /// <summary>
        /// When a topic loses its leader a new metadata request will be enqueued with this initial interval, exponentially increasing until the topic metadata has been
        /// refreshed. This is
        /// used to recover quickly from transitioning leader brokers.
        /// default: 250
        /// importance: low
        /// </summary>
        TimeSpan? TopicMetadataRefreshFastInterval { set; }

        /// <summary>
        /// Sparse metadata requests (consumes less network bandwidth)
        /// default: true
        /// importance: low
        /// </summary>
        bool? TopicMetadataRefreshSparse { set; }

        /// <summary>
        /// Topic blacklist, a comma-separated list of regular expressions for matching topic names that should be ignored in broker metadata information as if the topics
        /// did not exist.
        /// default: ''
        /// importance: low
        /// </summary>
        string TopicBlacklist { set; }

        /// <summary>
        /// A comma-separated list of debug contexts to enable. Detailed Producer debugging: broker,topic,msg. Consumer: consumer,cgrp,topic,fetch
        /// default: ''
        /// importance: medium
        /// </summary>
        string Debug { set; }

        /// <summary>
        /// How long to cache the broker address resolving results (milliseconds).
        /// default: 1000
        /// importance: low
        /// </summary>
        int? BrokerAddressTtl { set; }

        /// <summary>
        /// Allowed broker IP address families: any, v4, v6
        /// default: any
        /// importance: low
        /// </summary>
        BrokerAddressFamily? BrokerAddressFamily { set; }

        /// <summary>
        /// The initial time to wait before reconnecting to a broker after the connection has been closed. The time is increased exponentially until
        /// `reconnect.backoff.max.ms` is reached.
        /// -25% to +50% jitter is applied to each reconnect backoff. A value of 0 disables the backoff and reconnects immediately.
        /// default: 100
        /// importance: medium
        /// </summary>
        TimeSpan? ReconnectBackoff { set; }

        /// <summary>
        /// The maximum time to wait before reconnecting to a broker after the connection has been closed.
        /// default: 10000
        /// importance: medium
        /// </summary>
        TimeSpan? ReconnectBackoffMax { set; }

        /// <summary>
        /// librdkafka statistics emit interval. The application also needs to register a stats callback using `rd_kafka_conf_set_stats_cb()`. The granularity is 1000ms. A
        /// value of 0
        /// disables statistics.
        /// default: 0
        /// importance: high
        /// </summary>
        TimeSpan? StatisticsInterval { set; }

        /// <summary>
        /// Disable spontaneous log_cb from internal librdkafka threads, instead enqueue log messages on queue set with `rd_kafka_set_log_queue()` and serve log callbacks
        /// or events
        /// through the standard poll APIs. **NOTE**: Log messages will linger in a temporary queue until the log queue has been set.
        /// default: false
        /// importance: low
        /// </summary>
        bool? LogQueue { set; }

        /// <summary>
        /// Print internal thread name in log messages (useful for debugging librdkafka internals)
        /// default: true
        /// importance: low
        /// </summary>
        bool? LogThreadName { set; }

        /// <summary>
        /// Log broker disconnects. It might be useful to turn this off when interacting with 0.9 brokers with an aggressive `connection.max.idle.ms` value.
        /// default: true
        /// importance: low
        /// </summary>
        bool? LogConnectionClose { set; }

        /// <summary>
        /// Signal that librdkafka will use to quickly terminate on rd_kafka_destroy(). If this signal is not set then there will be a delay before
        /// rd_kafka_wait_destroyed() returns true
        /// as internal threads are timing out their system calls. If this signal is set however the delay will be minimal. The application should mask this signal as an
        /// internal signal
        /// handler is installed.
        /// default: 0
        /// importance: low
        /// </summary>
        int? InternalTerminationSignal { set; }

        /// <summary>
        /// Protocol used to communicate with brokers.
        /// default: plaintext
        /// importance: high
        /// </summary>
        SecurityProtocol? SecurityProtocol { set; }

        /// <summary>
        /// List of plugin libraries to load (; separated). The library search path is platform dependent (see dlopen(3) for Unix and LoadLibrary() for Windows). If no
        /// filename extension
        /// is specified the platform-specific extension (such as .dll or .so) will be appended automatically.
        /// default: ''
        /// importance: low
        /// </summary>
        string PluginLibraryPaths { set; }

        /// <summary>
        /// A rack identifier for this client. This can be any string value which indicates where this client is physically located. It corresponds with the broker config
        /// `broker.rack`.
        /// default: ''
        /// importance: low
        /// </summary>
        string ClientRack { set; }

        /// <summary>
        /// Configure Kafka host
        /// </summary>
        /// <param name="servers"></param>
        /// <param name="configure"></param>
        void Host(IReadOnlyList<string> servers, Action<IKafkaHostConfigurator> configure = null);

        /// <summary>
        /// Configure API versions
        /// </summary>
        /// <param name="configure"></param>
        void ConfigureApi(Action<IKafkaApiConfigurator> configure);

        /// <summary>
        /// Configure socket
        /// </summary>
        /// <param name="configure"></param>
        void ConfigureSocket(Action<IKafkaSocketConfigurator> configure);

        /// <summary>
        /// Subscribe to kafka topic
        /// </summary>
        /// <param name="topicName">The topic name</param>
        /// <param name="groupId">
        /// Client group id string. All clients sharing the same group.id belong to the same group.
        /// </param>
        /// <param name="configure"></param>
        /// <typeparam name="TKey">Message key type</typeparam>
        /// <typeparam name="TValue">Value key type</typeparam>
        void TopicEndpoint<TKey, TValue>(string topicName, string groupId, Action<IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class;

        /// <summary>
        /// Subscribe to kafka topic
        /// </summary>
        /// <param name="topicName">The topic name</param>
        /// <param name="consumerConfig">Consumer config</param>
        /// <param name="configure"></param>
        /// <typeparam name="TKey">Message key type</typeparam>
        /// <typeparam name="TValue">Value key type</typeparam>
        void TopicEndpoint<TKey, TValue>(string topicName, ConsumerConfig consumerConfig,
            Action<IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class;

        /// <summary>
        /// Configure kafka topic producer
        /// </summary>
        /// <param name="topicName">The topic name</param>
        /// <param name="configure"></param>
        /// <typeparam name="TKey">Message key type</typeparam>
        /// <typeparam name="TValue">Value key type</typeparam>
        internal void TopicProducer<TKey, TValue>(string topicName, Action<IKafkaProducerConfigurator<TKey, TValue>> configure)
            where TValue : class;

        /// <summary>
        /// Configure kafka topic producer
        /// </summary>
        /// <param name="topicName">The topic name</param>
        /// <param name="producerConfig">Producer config</param>
        /// <param name="configure"></param>
        /// <typeparam name="TKey">Message key type</typeparam>
        /// <typeparam name="TValue">Value key type</typeparam>
        internal void TopicProducer<TKey, TValue>(string topicName, ProducerConfig producerConfig, Action<IKafkaProducerConfigurator<TKey, TValue>> configure)
            where TValue : class;

        /// <summary>
        /// Set default headers deserializer for all subscriptions
        /// </summary>
        /// <param name="deserializer"></param>
        void SetHeadersDeserializer(IHeadersDeserializer deserializer);

        /// <summary>
        /// Set default headers serializer for all producers
        /// </summary>
        /// <param name="serializer"></param>
        void SetHeadersSerializer(IHeadersSerializer serializer);
    }
}
