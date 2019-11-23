namespace MassTransit.AzureServiceBusTransport.Transport
{
    using System;
    using Configuration;
    using GreenPipes;
    using Pipeline;
    using Topology;
    using Transports;


    public class ServiceBusHostProxy :
        BaseHostProxy,
        IServiceBusHost
    {
        readonly IServiceBusHostConfiguration _configuration;
        IServiceBusHost _host;

        public ServiceBusHostProxy(IServiceBusHostConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SetComplete(IServiceBusHost host)
        {
            _host = host;

            base.SetComplete(host);
        }

        public override Uri Address => _host?.Address ?? _configuration.HostAddress;

        IServiceBusHostTopology IServiceBusHost.Topology => _host?.Topology ?? throw new InvalidOperationException("The host is not ready.");

        IMessagingFactoryContextSupervisor IServiceBusHost.MessagingFactoryContextSupervisor =>
            _host?.MessagingFactoryContextSupervisor ?? throw new InvalidOperationException("The host is not ready.");

        INamespaceContextSupervisor IServiceBusHost.NamespaceContextSupervisor =>
            _host?.NamespaceContextSupervisor ?? throw new InvalidOperationException("The host is not ready.");

        IRetryPolicy IServiceBusHost.RetryPolicy => _host?.RetryPolicy ?? throw new InvalidOperationException("The host is not ready.");

        ServiceBusHostSettings IServiceBusHost.Settings => _configuration.Settings;

        public HostReceiveEndpointHandle ConnectSubscriptionEndpoint<T>(string subscriptionName,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure = null)
            where T : class
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.ConnectSubscriptionEndpoint<T>(subscriptionName, configure);
        }

        public HostReceiveEndpointHandle ConnectSubscriptionEndpoint(string subscriptionName, string topicName,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure = null)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.ConnectSubscriptionEndpoint(subscriptionName, topicName, configure);
        }

        HostReceiveEndpointHandle IReceiveConnector<IServiceBusReceiveEndpointConfigurator>.ConnectReceiveEndpoint(IEndpointDefinition definition,
            IEndpointNameFormatter endpointNameFormatter, Action<IServiceBusReceiveEndpointConfigurator> configureEndpoint)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.ConnectReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        HostReceiveEndpointHandle IReceiveConnector<IServiceBusReceiveEndpointConfigurator>.ConnectReceiveEndpoint(string queueName,
            Action<IServiceBusReceiveEndpointConfigurator> configure)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.ConnectReceiveEndpoint(queueName, configure);
        }

    }
}
