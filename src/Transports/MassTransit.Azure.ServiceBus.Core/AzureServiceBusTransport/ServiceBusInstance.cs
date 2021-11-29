namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using MassTransit.Configuration;
    using Transports;


    public class ServiceBusInstance :
        TransportBusInstance<IServiceBusReceiveEndpointConfigurator>,
        ISubscriptionEndpointConnector
    {
        readonly IServiceBusHost _host;

        public ServiceBusInstance(IBusControl busControl, IHost<IServiceBusReceiveEndpointConfigurator> host, IHostConfiguration hostConfiguration,
            IBusRegistrationContext busRegistrationContext)
            : base(busControl, host, hostConfiguration, busRegistrationContext)
        {
            _host = host as IServiceBusHost ?? throw new ArgumentException("Host was not an IServiceBusHost", nameof(host));
        }

        public HostReceiveEndpointHandle ConnectSubscriptionEndpoint<T>(string subscriptionName,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure = null)
            where T : class
        {
            return _host.ConnectSubscriptionEndpoint<T>(subscriptionName, configure);
        }

        public HostReceiveEndpointHandle ConnectSubscriptionEndpoint(string subscriptionName, string topicName,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure = null)
        {
            return _host.ConnectSubscriptionEndpoint(subscriptionName, topicName, configure);
        }
    }
}
