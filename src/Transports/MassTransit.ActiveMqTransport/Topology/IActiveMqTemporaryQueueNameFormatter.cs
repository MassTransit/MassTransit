using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.ActiveMqTransport.Topology
{
    public interface IActiveMqTemporaryQueueNameFormatter
    {
        public string Format(string Tag, string systemGeneratedQueueName);
    }
}
