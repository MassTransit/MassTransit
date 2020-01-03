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
        public TopicPublishSettings(AmazonSqsEndpointAddress address)
            : base(address.Name, address.Durable, address.AutoDelete)
        {
        }

        public Uri GetSendAddress(Uri hostAddress) => GetEndpointAddress(hostAddress);

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

        public override string ToString() => string.Join(", ", GetSettingStrings());
    }
}
