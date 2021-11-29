namespace MassTransit
{
    using System;


    public interface IKafkaFetchConfigurator
    {
        /// <summary>
        /// Maximum time the broker may wait to fill the response with fetch.min.bytes.
        /// default: 100
        /// importance: low
        /// </summary>
        TimeSpan? WaitMaxInterval { set; }

        /// <summary>
        /// Initial maximum number of bytes per topic+partition to request when fetching messages from the broker. If the client encounters a message larger than this value it will
        /// gradually try to increase it until the entire message can be fetched.
        /// default: 1048576
        /// importance: medium
        /// </summary>
        int? MaxPartitionBytes { set; }

        /// <summary>
        /// Maximum amount of data the broker shall return for a Fetch request. Messages are fetched in batches by the consumer and if the first message batch in the first non-empty
        /// partition of the Fetch request is larger than this value, then the message batch will still be returned to ensure the consumer can make progress. The maximum message batch
        /// size accepted by the broker is defined via `message.max.bytes` (broker config) or `max.message.bytes` (broker topic config). `fetch.max.bytes` is automatically adjusted
        /// upwards to be at least `message.max.bytes` (consumer config).
        /// default: 52428800
        /// importance: medium
        /// </summary>
        int? MaxBytes { set; }

        /// <summary>
        /// Minimum number of bytes the broker responds with. If fetch.wait.max.ms expires the accumulated data will be sent to the client regardless of this setting.
        /// default: 1
        /// importance: low
        /// </summary>
        int? MinBytes { set; }

        /// <summary>
        /// How long to postpone the next fetch request for a topic+partition in case of a fetch error.
        /// default: 500
        /// importance: medium
        /// </summary>
        TimeSpan? ErrorBackoffInterval { set; }
    }
}
