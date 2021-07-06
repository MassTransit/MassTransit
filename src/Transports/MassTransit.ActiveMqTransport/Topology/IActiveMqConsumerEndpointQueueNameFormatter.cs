using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.ActiveMqTransport.Topology
{
    public interface IActiveMqConsumerEndpointQueueNameFormatter
    {
        public string Format(string topic, string endpointName);
    }


    public interface IActiveMqTemporaryQueueNameFormatter
    {
        public string Format(string Tag, string systemGeneratedQueueName);
    }
}
