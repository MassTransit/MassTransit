namespace MassTransit.KafkaIntegration
{
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using GreenPipes;
    using Transport;


    public static class KafkaProducerExtensions
    {
        /// <summary>
        /// Produces a message to the configured Kafka topic.
        /// </summary>
        /// <param name="producer">The producer</param>
        /// <param name="value">The message value</param>
        /// <param name="cancellationToken"></param>
        public static Task Produce<TValue>(this IKafkaProducer<Null, TValue> producer, TValue value, CancellationToken cancellationToken = default)
            where TValue : class
        {
            return producer.Produce(default, value, cancellationToken);
        }

        /// <summary>
        /// Produces a message to the configured Kafka topic.
        /// </summary>
        /// <param name="producer">The producer</param>
        /// <param name="value">The message value</param>
        /// <param name="pipe">A pipe which is called to customize the produced message context</param>
        /// <param name="cancellationToken"></param>
        public static Task Produce<TValue>(this IKafkaProducer<Null, TValue> producer, TValue value, IPipe<KafkaSendContext<TValue>> pipe,
            CancellationToken cancellationToken = default)
            where TValue : class
        {
            return producer.Produce(default, value, pipe, cancellationToken);
        }

        /// <summary>
        /// Produces a message to the configured Kafka topic.
        /// </summary>
        /// <param name="producer">The producer</param>
        /// <param name="values">An object which is used to initialize the message</param>
        /// <param name="cancellationToken"></param>
        public static Task Produce<TValue>(this IKafkaProducer<Null, TValue> producer, object values, CancellationToken cancellationToken = default)
            where TValue : class
        {
            return producer.Produce(default, values, cancellationToken);
        }

        /// <summary>
        /// Produces a message to the configured Kafka topic.
        /// </summary>
        /// <param name="producer">The producer</param>
        /// <param name="values">An object which is used to initialize the message</param>
        /// <param name="pipe">A pipe which is called to customize the produced message context</param>
        /// <param name="cancellationToken"></param>
        public static Task Produce<TValue>(this IKafkaProducer<Null, TValue> producer, object values, IPipe<KafkaSendContext<TValue>> pipe,
            CancellationToken cancellationToken = default)
            where TValue : class
        {
            return producer.Produce(default, values, pipe, cancellationToken);
        }

        /// <summary>
        /// Produces a message to the configured Kafka topic.
        /// </summary>
        /// <param name="producer">The producer</param>
        /// <param name="value">The message value</param>
        /// <param name="cancellationToken"></param>
        public static Task Produce<TValue>(this IKafkaProducer<Ignore, TValue> producer, TValue value, CancellationToken cancellationToken = default)
            where TValue : class
        {
            return producer.Produce(default, value, cancellationToken);
        }

        /// <summary>
        /// Produces a message to the configured Kafka topic.
        /// </summary>
        /// <param name="producer">The producer</param>
        /// <param name="value">The message value</param>
        /// <param name="pipe">A pipe which is called to customize the produced message context</param>
        /// <param name="cancellationToken"></param>
        public static Task Produce<TValue>(this IKafkaProducer<Ignore, TValue> producer, TValue value, IPipe<KafkaSendContext<TValue>> pipe,
            CancellationToken cancellationToken = default)
            where TValue : class
        {
            return producer.Produce(default, value, pipe, cancellationToken);
        }

        /// <summary>
        /// Produces a message to the configured Kafka topic.
        /// </summary>
        /// <param name="producer">The producer</param>
        /// <param name="values">An object which is used to initialize the message</param>
        /// <param name="cancellationToken"></param>
        public static Task Produce<TValue>(this IKafkaProducer<Ignore, TValue> producer, object values, CancellationToken cancellationToken = default)
            where TValue : class
        {
            return producer.Produce(default, values, cancellationToken);
        }

        /// <summary>
        /// Produces a message to the configured Kafka topic.
        /// </summary>
        /// <param name="producer">The producer</param>
        /// <param name="values">An object which is used to initialize the message</param>
        /// <param name="pipe">A pipe which is called to customize the produced message context</param>
        /// <param name="cancellationToken"></param>
        public static Task Produce<TValue>(this IKafkaProducer<Ignore, TValue> producer, object values, IPipe<KafkaSendContext<TValue>> pipe,
            CancellationToken cancellationToken = default)
            where TValue : class
        {
            return producer.Produce(default, values, pipe, cancellationToken);
        }
    }
}
