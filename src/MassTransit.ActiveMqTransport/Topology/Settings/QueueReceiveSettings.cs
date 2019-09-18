namespace MassTransit.ActiveMqTransport.Topology.Settings
{
    using System;
    using Configurators;


    public class QueueReceiveSettings :
        QueueBindingConfigurator,
        ReceiveSettings
    {
        public QueueReceiveSettings(string queueName, bool durable, bool autoDelete)
            : base(queueName, durable, autoDelete)
        {
            PrefetchCount = (ushort)Math.Min(Environment.ProcessorCount * 2, 16);
        }

        public ushort PrefetchCount { get; set; }

        public bool Exclusive { get; set; }

        public bool PurgeOnStartup { get; set; }

        public Uri GetInputAddress(Uri hostAddress)
        {
            var builder = new UriBuilder(hostAddress);

            builder.Path = builder.Path == "/"
                ? $"/{EntityName}"
                : $"/{string.Join("/", builder.Path.Trim('/'), EntityName)}";

            builder.Query += string.Join("&", GetQueryStringOptions());

            return builder.Uri;
        }
    }
}
