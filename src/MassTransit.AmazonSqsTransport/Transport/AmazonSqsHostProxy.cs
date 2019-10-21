namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using Configuration;
    using Configuration.Configuration;
    using GreenPipes;
    using Topology;
    using Transports;


    public class AmazonSqsHostProxy :
        BaseHostProxy,
        IAmazonSqsHost
    {
        readonly IAmazonSqsHostConfiguration _configuration;
        IAmazonSqsHost _host;

        public AmazonSqsHostProxy(IAmazonSqsHostConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SetComplete(IAmazonSqsHost host)
        {
            _host = host;

            base.SetComplete(host);
        }

        public override Uri Address => _host?.Address ?? _configuration.HostAddress;

        IAmazonSqsHostTopology IAmazonSqsHost.Topology => _host?.Topology ?? throw new InvalidOperationException("The host is not ready.");

        IConnectionContextSupervisor IAmazonSqsHost.ConnectionContextSupervisor =>
            _host?.ConnectionContextSupervisor ?? throw new InvalidOperationException("The host is not ready.");

        IRetryPolicy IAmazonSqsHost.ConnectionRetryPolicy => _host?.ConnectionRetryPolicy ?? throw new InvalidOperationException("The host is not ready.");

        AmazonSqsHostSettings IAmazonSqsHost.Settings => _configuration.Settings;

        HostReceiveEndpointHandle IReceiveConnector<IAmazonSqsReceiveEndpointConfigurator>.ConnectReceiveEndpoint(IEndpointDefinition definition,
            IEndpointNameFormatter endpointNameFormatter, Action<IAmazonSqsReceiveEndpointConfigurator> configureEndpoint)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.ConnectReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        HostReceiveEndpointHandle IReceiveConnector<IAmazonSqsReceiveEndpointConfigurator>.ConnectReceiveEndpoint(string queueName,
            Action<IAmazonSqsReceiveEndpointConfigurator> configure)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.ConnectReceiveEndpoint(queueName, configure);
        }
    }
}
