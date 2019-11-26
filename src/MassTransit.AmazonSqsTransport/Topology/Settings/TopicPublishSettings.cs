namespace MassTransit.AmazonSqsTransport.Topology.Settings
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using Configuration.Configurators;


    public class TopicPublishSettings :
        TopicConfigurator,
        PublishSettings
    {
        public TopicPublishSettings(string topicName, bool durable, bool autoDelete)
            : base(topicName, durable, autoDelete)
        {
        }

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
            var builder = new PublishEndpointBrokerTopologyBuilder();

            builder.Topic = builder.CreateTopic(EntityName, Durable, AutoDelete);

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
