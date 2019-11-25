namespace MassTransit.AmazonSqsTransport.Topology.Settings
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using Configuration.Configurators;


    public class QueueSendSettings :
        QueueConfigurator,
        SendSettings
    {
        public QueueSendSettings(string queueName, bool durable, bool autoDelete)
            : base(queueName, durable, autoDelete)
        {
        }

        public bool PushContextHeadersOverMessageAttributes { get; set; } = true;

        public Uri GetSendAddress(Uri hostAddress)
        {
            var builder = new UriBuilder(hostAddress);

            builder.Path = builder.Path == "/"
                ? $"/{EntityName}"
                : $"/{string.Join("/", builder.Path.Trim('/'), EntityName)}";

            builder.Query += string.Join("&", GetQueryStringOptions());

            return builder.Uri;
        }

        public BrokerTopology GetBrokerTopology()
        {
            var builder = new SendEndpointBrokerTopologyBuilder();

            builder.Queue = builder.CreateQueue(EntityName, Durable, AutoDelete);

            return builder.BuildBrokerTopology();
        }

        IEnumerable<string> GetSettingStrings()
        {
            if (Durable)
                yield return "durable";

            if (AutoDelete)
                yield return "auto-delete";
        }

        public override string ToString()
        {
            return string.Join(", ", GetSettingStrings());
        }
    }
}
