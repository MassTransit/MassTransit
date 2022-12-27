#nullable enable
namespace MassTransit
{
    using System.Collections.Generic;
    using Confluent.Kafka;


    public class KafkaTestHarnessOptions
    {
        /// <summary>
        /// Number of partitions for given TopicNames
        /// </summary>
        public int Partitions { get; set; } = 1;

        /// <summary>
        /// Number of replicas for given TopicNames
        /// </summary>
        public short Replicas { get; set; } = 1;

        /// <summary>
        /// Topic names which will be created when starting the test harness (via a hosted service) if CreateTopicsIfNotExists set to true
        /// </summary>
        public IReadOnlyList<string> TopicNames { get; set; } = new List<string>();

        /// <summary>
        /// Attempts to create the topics on the broker using the admin client. The <see cref="ClientConfig" /> will be
        /// used to obtain the host, port, etc.
        /// </summary>
        public bool CreateTopicsIfNotExists { get; set; }

        /// <summary>
        /// Remove topic names from broker when starting the test harness (via a hosted service)
        /// </summary>
        public bool CleanTopicsOnStart { get; set; }
    }
}
