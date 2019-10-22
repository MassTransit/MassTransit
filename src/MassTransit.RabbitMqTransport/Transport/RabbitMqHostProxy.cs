namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using EndpointConfigurators;
    using GreenPipes;
    using Integration;
    using MassTransit.Pipeline;
    using MassTransit.Topology;
    using Topology;
    using Transports;


    public class RabbitMqHostProxy :
        IRabbitMqHost
    {
        IRabbitMqHost _host;

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

        IRabbitMqHostTopology IRabbitMqHost.Topology
        {
            get
            {
                if (_host == null)
                    throw new InvalidOperationException("The host is not ready.");

                return _host.Topology;
            }
        }

        HostReceiveEndpointHandle IReceiveConnector<IRabbitMqReceiveEndpointConfigurator>.ConnectReceiveEndpoint(IEndpointDefinition definition,
            IEndpointNameFormatter endpointNameFormatter, Action<IRabbitMqReceiveEndpointConfigurator> configureEndpoint)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.ConnectReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        HostReceiveEndpointHandle IReceiveConnector<IRabbitMqReceiveEndpointConfigurator>.ConnectReceiveEndpoint(string queueName,
            Action<IRabbitMqReceiveEndpointConfigurator> configure)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.ConnectReceiveEndpoint(queueName, configure);
        }

        IConnectionContextSupervisor IRabbitMqHost.ConnectionContextSupervisor
        {
            get
            {
                if (_host == null)
                    throw new InvalidOperationException("The host is not ready.");

                return _host.ConnectionContextSupervisor;
            }
        }

        IRetryPolicy IRabbitMqHost.ConnectionRetryPolicy
        {
            get
            {
                if (_host == null)
                    throw new InvalidOperationException("The host is not ready.");

                return _host.ConnectionRetryPolicy;
            }
        }

        RabbitMqHostSettings IRabbitMqHost.Settings
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

        public void SetComplete(IRabbitMqHost host)
        {
            _host = host;
        }
    }
}
