namespace MassTransit.ActiveMqTransport.Transport
{
    using System;
    using Configuration;
    using GreenPipes;
    using Topology;
    using Transports;


    public class ActiveMqHostProxy :
        BaseHostProxy,
        IActiveMqHost
    {
        readonly IActiveMqHostConfiguration _configuration;
        IActiveMqHost _host;

        public ActiveMqHostProxy(IActiveMqHostConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SetComplete(IActiveMqHost host)
        {
            _host = host;

            base.SetComplete(host);
        }

        public override Uri Address => _host?.Address ?? _configuration.HostAddress;

        IActiveMqHostTopology IActiveMqHost.Topology => _host?.Topology ?? throw new InvalidOperationException("The host is not ready.");

        IConnectionContextSupervisor IActiveMqHost.ConnectionContextSupervisor =>
            _host?.ConnectionContextSupervisor ?? throw new InvalidOperationException("The host is not ready.");

        IRetryPolicy IActiveMqHost.ConnectionRetryPolicy => _host?.ConnectionRetryPolicy ?? throw new InvalidOperationException("The host is not ready.");

        ActiveMqHostSettings IActiveMqHost.Settings => _configuration.Settings;

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
    }
}
