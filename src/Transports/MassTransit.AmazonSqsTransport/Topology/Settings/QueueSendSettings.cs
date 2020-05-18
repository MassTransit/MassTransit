namespace MassTransit.AmazonSqsTransport.Topology.Settings
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using Configurators;


    public class QueueSendSettings :
        QueueConfigurator,
        SendSettings
    {
        public QueueSendSettings(AmazonSqsEndpointAddress address)
            : base(address.Name, address.Durable,address.AutoDelete)
        {
        }

        public Uri GetSendAddress(Uri hostAddress) => GetEndpointAddress(hostAddress);

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
