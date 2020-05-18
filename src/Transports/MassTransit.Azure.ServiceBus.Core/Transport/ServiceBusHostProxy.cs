namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using GreenPipes;
    using GreenPipes.Agents;
    using Pipeline;
    using Topology;
    using Transports;


    public class ServiceBusHostProxy :
        BaseHostProxy,
        IServiceBusHostControl
    {
        readonly IServiceBusHostConfiguration _configuration;
        IServiceBusHostControl _host;

        public ServiceBusHostProxy(IServiceBusHostConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SetComplete(IServiceBusHostControl host)
        {
            _host = host;

            base.SetComplete(host);
        }

        public override Uri Address => _host?.Address ?? _configuration.HostAddress;

        IServiceBusHostTopology IServiceBusHost.Topology => _host?.Topology ?? throw new InvalidOperationException("The host is not ready.");

        IConnectionContextSupervisor IServiceBusHost.ConnectionContextSupervisor =>
            _host?.ConnectionContextSupervisor ?? throw new InvalidOperationException("The host is not ready.");

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

        Task IAgent.Stop(StopContext context)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.Stop(context);
        }

        Task IAgent.Ready
        {
            get
            {
                if (_host == null)
                    throw new InvalidOperationException("The host is not ready.");

                return _host.Ready;
            }
        }

        Task IAgent.Completed
        {
            get
            {
                if (_host == null)
                    throw new InvalidOperationException("The host is not ready.");

                return _host.Completed;
            }
        }

        CancellationToken IAgent.Stopping
        {
            get
            {
                if (_host == null)
                    throw new InvalidOperationException("The host is not ready.");

                return _host.Stopping;
            }
        }

        CancellationToken IAgent.Stopped
        {
            get
            {
                if (_host == null)
                    throw new InvalidOperationException("The host is not ready.");

                return _host.Stopped;
            }
        }

        void ISupervisor.Add(IAgent agent)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            _host.Add(agent);
        }

        int ISupervisor.PeakActiveCount
        {
            get
            {
                if (_host == null)
                    throw new InvalidOperationException("The host is not ready.");

                return _host.PeakActiveCount;
            }
        }

        long ISupervisor.TotalCount
        {
            get
            {
                if (_host == null)
                    throw new InvalidOperationException("The host is not ready.");

                return _host.TotalCount;
            }
        }

        Task<HostHandle> IBusHostControl.Start(CancellationToken cancellationToken)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.Start(cancellationToken);
        }

        void IBusHostControl.AddReceiveEndpoint(string endpointName, IReceiveEndpointControl receiveEndpoint)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            _host.AddReceiveEndpoint(endpointName, receiveEndpoint);
        }

        Task<ISendTransport> IServiceBusHostControl.CreateSendTransport(ServiceBusEndpointAddress address)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.CreateSendTransport(address);
        }

        Task<ISendTransport> IServiceBusHostControl.CreatePublishTransport<T>()
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.CreatePublishTransport<T>();
        }
    }
}
