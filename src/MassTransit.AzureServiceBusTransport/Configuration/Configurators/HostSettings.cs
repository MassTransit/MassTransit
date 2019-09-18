namespace MassTransit.AzureServiceBusTransport.Configurators
{
    using System;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.ServiceBus.Messaging.Amqp;


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
            AmqpTransportSettings = new AmqpTransportSettings();
            NetMessagingTransportSettings = new NetMessagingTransportSettings();
        }

        public Uri ServiceUri { get; set; }
        public TokenProvider TokenProvider { get; set; }
        public TimeSpan OperationTimeout { get; set; }
        public TimeSpan RetryMinBackoff { get; set; }
        public TimeSpan RetryMaxBackoff { get; set; }
        public int RetryLimit { get; set; }
        public TransportType TransportType { get; set; }
        public AmqpTransportSettings AmqpTransportSettings { get; set; }
        public NetMessagingTransportSettings NetMessagingTransportSettings { get; set; }
    }
}
