namespace MassTransit.AzureServiceBusTransport.Transport
{
    using System;
    using EndpointConfigurators;
    using GreenPipes;
    using MassTransit.Pipeline;
    using MassTransit.Topology;
    using Pipeline;
    using Topology;
    using Transports;


    public class ServiceBusHostProxy :
        IServiceBusHost
    {
        IServiceBusHost _host;

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.ConnectSendObserver(observer);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.ConnectPublishObserver(observer);
        }

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.ConnectConsumeMessageObserver(observer);
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.ConnectConsumeObserver(observer);
        }

        ConnectHandle IReceiveObserverConnector.ConnectReceiveObserver(IReceiveObserver observer)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.ConnectReceiveObserver(observer);
        }

        ConnectHandle IReceiveEndpointObserverConnector.ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.ConnectReceiveEndpointObserver(observer);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }

        Uri IHost.Address
        {
            get
            {
                if (_host == null)
                    throw new InvalidOperationException("The host is not ready.");

                return _host.Address;
            }
        }

        IServiceBusHostTopology IServiceBusHost.Topology
        {
            get
            {
                if (_host == null)
                    throw new InvalidOperationException("The host is not ready.");

                return _host.Topology;
            }
        }

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

        IMessagingFactoryContextSupervisor IServiceBusHost.MessagingFactoryContextSupervisor
        {
            get
            {
                if (_host == null)
                    throw new InvalidOperationException("The host is not ready.");

                return _host.MessagingFactoryContextSupervisor;
            }
        }

        INamespaceContextSupervisor IServiceBusHost.NamespaceContextSupervisor
        {
            get
            {
                if (_host == null)
                    throw new InvalidOperationException("The host is not ready.");

                return _host.NamespaceContextSupervisor;
            }
        }

        IRetryPolicy IServiceBusHost.RetryPolicy
        {
            get
            {
                if (_host == null)
                    throw new InvalidOperationException("The host is not ready.");

                return _host.RetryPolicy;
            }
        }

        ServiceBusHostSettings IServiceBusHost.Settings
        {
            get
            {
                if (_host == null)
                    throw new InvalidOperationException("The host is not ready.");

                return _host.Settings;
            }
        }

        IHostTopology IHost.Topology
        {
            get
            {
                if (_host == null)
                    throw new InvalidOperationException("The host is not ready.");

                return _host.Topology;
            }
        }

        HostReceiveEndpointHandle IReceiveConnector.ConnectReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            return _host.ConnectReceiveEndpoint(queueName, configureEndpoint);
        }

        HostReceiveEndpointHandle IReceiveConnector.ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            return _host.ConnectReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        ConnectHandle IEndpointConfigurationObserverConnector.ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
        {
            return _host.ConnectEndpointConfigurationObserver(observer);
        }

        public void SetComplete(IServiceBusHost host)
        {
            _host = host;
        }
    }
}
