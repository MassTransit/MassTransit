namespace MassTransit.Transports.InMemory
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Builders;
    using Configuration;
    using Context;
    using GreenPipes.Agents;
    using Topology.Builders;


    /// <summary>
    /// Caches InMemory transport instances so that they are only created and used once
    /// </summary>
    public class InMemoryHostProxy :
        BaseHostProxy,
        IInMemoryHostControl
    {
        readonly IInMemoryHostConfiguration _configuration;
        IInMemoryHostControl _host;

        public InMemoryHostProxy(IInMemoryHostConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SetComplete(IInMemoryHostControl host)
        {
            _host = host;

            base.SetComplete(host);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IInMemoryReceiveEndpointConfigurator> configureEndpoint = null)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.ConnectReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IInMemoryReceiveEndpointConfigurator> configureEndpoint)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.ConnectReceiveEndpoint(queueName, configureEndpoint);
        }

        public Task Stop(StopContext context)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.Stop(context);
        }

        public Task Ready
        {
            get
            {
                if (_host == null)
                    throw new InvalidOperationException("The host is not ready.");

                return _host.Ready;
            }
        }

        public Task Completed
        {
            get
            {
                if (_host == null)
                    throw new InvalidOperationException("The host is not ready.");

                return _host.Completed;
            }
        }

        public CancellationToken Stopping
        {
            get
            {
                if (_host == null)
                    throw new InvalidOperationException("The host is not ready.");

                return _host.Stopping;
            }
        }

        public CancellationToken Stopped
        {
            get
            {
                if (_host == null)
                    throw new InvalidOperationException("The host is not ready.");

                return _host.Stopped;
            }
        }

        public void Add(IAgent agent)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            _host.Add(agent);
        }

        public int PeakActiveCount
        {
            get
            {
                if (_host == null)
                    throw new InvalidOperationException("The host is not ready.");

                return _host.PeakActiveCount;
            }
        }

        public long TotalCount
        {
            get
            {
                if (_host == null)
                    throw new InvalidOperationException("The host is not ready.");

                return _host.TotalCount;
            }
        }

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

        public Task<ISendTransport> GetSendTransport(Uri address)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.GetSendTransport(address);
        }

        public Uri NormalizeAddress(Uri address)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.NormalizeAddress(address);
        }

        public IReceiveTransport GetReceiveTransport(string queueName, ReceiveEndpointContext receiveEndpointContext)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.GetReceiveTransport(queueName, receiveEndpointContext);
        }

        public IInMemoryPublishTopologyBuilder CreatePublishTopologyBuilder(
            PublishEndpointTopologyBuilder.Options options = PublishEndpointTopologyBuilder.Options.MaintainHierarchy)
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.CreatePublishTopologyBuilder(options);
        }

        public IInMemoryConsumeTopologyBuilder CreateConsumeTopologyBuilder()
        {
            if (_host == null)
                throw new InvalidOperationException("The host is not ready.");

            return _host.CreateConsumeTopologyBuilder();
        }
    }
}
