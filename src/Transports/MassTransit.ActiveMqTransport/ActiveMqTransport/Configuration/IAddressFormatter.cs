using System.Collections.Generic;

namespace MassTransit.ActiveMqTransport.Configuration
{
    public interface IAddressFormatter
    {
        public Dictionary<string, string> TransportOptions { get; }
    }
}
