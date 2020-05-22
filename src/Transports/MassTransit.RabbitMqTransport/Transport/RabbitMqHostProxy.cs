namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using GreenPipes;
    using GreenPipes.Agents;
    using Integration;
    using Topology;
    using Transports;


    public class RabbitMqHostProxy :
        BaseHostProxy,
        IRabbitMqHostControl
    {
        readonly IRabbitMqHostConfiguration _configuration;
        IRabbitMqHostControl _host;

        public RabbitMqHostProxy(IRabbitMqHostConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SetComplete(IRabbitMqHostControl host)
        {
            _host = host;

            base.SetComplete(host);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IRabbitMqReceiveEndpointConfigurator> configureEndpoint = null)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.ConnectReceiveEndpoint(definition, endpointNameFormatter);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IRabbitMqReceiveEndpointConfigurator> configureEndpoint)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.ConnectReceiveEndpoint(queueName, configureEndpoint);
        }

        public IConnectionContextSupervisor ConnectionContextSupervisor =>
            _host?.ConnectionContextSupervisor ?? throw new InvalidOperationException("The host is not ready.");

        public IRetryPolicy ConnectionRetryPolicy => _host?.ConnectionRetryPolicy ?? throw new InvalidOperationException("The host is not ready.");

        public RabbitMqHostSettings Settings => _configuration.Settings;

        public new IRabbitMqHostTopology Topology => _host?.Topology ?? throw new InvalidOperationException("The host is not ready.");

        public Task Stop(StopContext context)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.Stop(context);
        }

        public Task Ready => _host?.Ready ?? throw new InvalidOperationException("The host is not ready.");

        public Task Completed => _host?.Completed ?? throw new InvalidOperationException("The host is not ready.");

        public CancellationToken Stopping => _host?.Stopping ?? throw new InvalidOperationException("The host is not ready.");

        public CancellationToken Stopped => _host?.Stopped ?? throw new InvalidOperationException("The host is not ready.");

        public void Add(IAgent agent)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            _host.Add(agent);
        }

        public int PeakActiveCount => _host?.PeakActiveCount ?? throw new InvalidOperationException("The host is not ready.");

        public long TotalCount => _host?.TotalCount ?? throw new InvalidOperationException("The host is not ready.");

        public Task<HostHandle> Start(CancellationToken cancellationToken)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.Start(cancellationToken);
        }

        public void AddReceiveEndpoint(string endpointName, IReceiveEndpointControl receiveEndpoint)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            _host.AddReceiveEndpoint(endpointName, receiveEndpoint);
        }

        public Task<ISendTransport> CreateSendTransport(RabbitMqEndpointAddress address, IModelContextSupervisor modelContextSupervisor)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.CreateSendTransport(address, modelContextSupervisor);
        }

        public Task<ISendTransport> CreatePublishTransport<T>(IModelContextSupervisor modelContextSupervisor)
            where T : class
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.CreatePublishTransport<T>(modelContextSupervisor);
        }
    }
}
