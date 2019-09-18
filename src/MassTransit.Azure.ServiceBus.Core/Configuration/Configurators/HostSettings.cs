namespace MassTransit.Azure.ServiceBus.Core.Configurators
{
    using System;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Primitives;


    public class HostSettings :
        ServiceBusHostSettings
    {
        public HostSettings()
        {
            OperationTimeout = TimeSpan.FromSeconds(60);

            RetryMinBackoff = TimeSpan.FromMilliseconds(100);
            RetryMaxBackoff = TimeSpan.FromSeconds(30);
            RetryLimit = 10;

            TransportType = TransportType.Amqp;
        }

        public Uri ServiceUri { get; set; }
        public ITokenProvider TokenProvider { get; set; }
        public TimeSpan OperationTimeout { get; set; }
        public TimeSpan RetryMinBackoff { get; set; }
        public TimeSpan RetryMaxBackoff { get; set; }
        public int RetryLimit { get; set; }
        public TransportType TransportType { get; set; }
    }
}
