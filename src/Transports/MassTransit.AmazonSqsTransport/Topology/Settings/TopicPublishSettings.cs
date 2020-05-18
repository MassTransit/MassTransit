namespace MassTransit.AmazonSqsTransport.Topology.Settings
{
    using System;
    using System.Collections.Generic;
    using Configurators;


    public class TopicPublishSettings :
        TopicConfigurator,
        PublishSettings
    {
        public TopicPublishSettings(AmazonSqsEndpointAddress address)
            : base(address.Name, address.Durable, address.AutoDelete)
        {
        }

        public Uri GetSendAddress(Uri hostAddress) => GetEndpointAddress(hostAddress);

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
