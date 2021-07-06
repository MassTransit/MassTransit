using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.ActiveMqTransport.Configuration.Definition
{
    using Topology;


    public class ArtemisConsumerEndpointQueueNameFormatter : IActiveMqConsumerEndpointQueueNameFormatter
    {
        public string Format(string topic, string endpointName)
        {
            return $"{topic}::Consumer.{endpointName}.{topic}";
        }
    }
}
