namespace MassTransit.ActiveMqTransport.Configuration
{
    using System.Collections.Generic;

    public interface IAddressFormatter
    {
        public Dictionary<string, string> TransportOptions { get; }
    }
}
