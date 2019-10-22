namespace MassTransit.ActiveMqTransport.Transport
{
    using System;
    using EndpointConfigurators;
    using GreenPipes;
    using MassTransit.Pipeline;
    using MassTransit.Topology;
    using Topology;
    using Transports;


    public class ActiveMqHostProxy :
        IActiveMqHost
    {
        IActiveMqHost _host;

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

        IActiveMqHostTopology IActiveMqHost.Topology
        {
            get
            {
                if (_host == null)
                    throw new InvalidOperationException("The host is not ready.");

                return _host.Topology;
            }
        }

        HostReceiveEndpointHandle IReceiveConnector<IActiveMqReceiveEndpointConfigurator>.ConnectReceiveEndpoint(IEndpointDefinition definition,
            IEndpointNameFormatter endpointNameFormatter, Action<IActiveMqReceiveEndpointConfigurator> configureEndpoint)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.ConnectReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        HostReceiveEndpointHandle IReceiveConnector<IActiveMqReceiveEndpointConfigurator>.ConnectReceiveEndpoint(string queueName,
            Action<IActiveMqReceiveEndpointConfigurator> configure)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.ConnectReceiveEndpoint(queueName, configure);
        }

        IConnectionContextSupervisor IActiveMqHost.ConnectionContextSupervisor
        {
            get
            {
                if (_host == null)
                    throw new InvalidOperationException("The host is not ready.");

                return _host.ConnectionContextSupervisor;
            }
        }

        IRetryPolicy IActiveMqHost.ConnectionRetryPolicy
        {
            get
            {
                if (_host == null)
                    throw new InvalidOperationException("The host is not ready.");

                return _host.ConnectionRetryPolicy;
            }
        }

        ActiveMqHostSettings IActiveMqHost.Settings
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

        public void SetComplete(IActiveMqHost host)
        {
            _host = host;
        }
    }
}
