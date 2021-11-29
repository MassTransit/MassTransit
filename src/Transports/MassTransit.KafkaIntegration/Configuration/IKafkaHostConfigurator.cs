namespace MassTransit
{
    using System;


    public interface IKafkaHostConfigurator
    {
        /// <summary>
        /// Configure the use of SSL to connection to Kafka
        /// </summary>
        /// <param name="configure"></param>
        void UseSsl(Action<IKafkaSslConfigurator> configure);

        /// <summary>
        /// Configure the use of SASL to connection to Kafka
        /// </summary>
        /// <param name="configure"></param>
        void UseSasl(Action<IKafkaSaslConfigurator> configure);

        /// <summary>
        /// The maximum length of time (in milliseconds) before a cancellation request
        /// is acted on. Low values may result in measurably higher CPU usage.
        /// default: 100
        /// range: 1 &lt;= dotnet.cancellation.delay.max.ms &lt;= 10000
        /// importance: low
        /// </summary>
        void CancellationDelay(TimeSpan timeSpan);
    }
}
