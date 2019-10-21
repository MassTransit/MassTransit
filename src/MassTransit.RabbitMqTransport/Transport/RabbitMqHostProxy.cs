namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using Configuration;
    using GreenPipes;
    using Integration;
    using Topology;
    using Transports;


    public class RabbitMqHostProxy :
        BaseHostProxy,
        IRabbitMqHost
    {
        readonly IRabbitMqHostConfiguration _configuration;
        IRabbitMqHost _host;

        public RabbitMqHostProxy(IRabbitMqHostConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SetComplete(IRabbitMqHost host)
        {
            _host = host;

            base.SetComplete(host);
        }

        public override Uri Address => _host?.Address ?? _configuration.HostAddress;

        IRabbitMqHostTopology IRabbitMqHost.Topology => _host?.Topology ?? throw new InvalidOperationException("The host is not ready.");

        IConnectionContextSupervisor IRabbitMqHost.ConnectionContextSupervisor =>
            _host?.ConnectionContextSupervisor ?? throw new InvalidOperationException("The host is not ready.");

        IRetryPolicy IRabbitMqHost.ConnectionRetryPolicy => _host?.ConnectionRetryPolicy ?? throw new InvalidOperationException("The host is not ready.");

        RabbitMqHostSettings IRabbitMqHost.Settings => _configuration.Settings;

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
    }
}
