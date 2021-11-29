namespace MassTransit.ActiveMqTransport.Topology
{
    using System;
    using System.Collections.Generic;
    using Configuration;


    public class ActiveMqTopicSendSettings :
        ActiveMqTopicConfigurator,
        SendSettings
    {
        public ActiveMqTopicSendSettings(ActiveMqEndpointAddress address)
            : base(address.Name, address.Durable, address.AutoDelete)
        {
        }

        public Uri GetSendAddress(Uri hostAddress)
        {
            return GetEndpointAddress(hostAddress);
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
