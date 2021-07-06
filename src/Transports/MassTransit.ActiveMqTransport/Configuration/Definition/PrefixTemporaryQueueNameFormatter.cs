using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.ActiveMqTransport.Configuration.Definition
{
    using Topology;


    public class PrefixTemporaryQueueNameFormatter : IActiveMqTemporaryQueueNameFormatter
    {
        private readonly string _prefix;

        public PrefixTemporaryQueueNameFormatter(string prefix)
        {
            _prefix = prefix;
        }

        public string Format(string Tag, string systemGeneratedQueueName)
        {
            return $"{_prefix}{systemGeneratedQueueName}";
        }
    }
}
