namespace MassTransit.EventHubIntegration
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using Riders;
    using Transports;


    public class EventHubRider :
        IEventHubRider
    {
        readonly IReceiveEndpointCollection _endpoints;
        readonly IEventHubHostConfiguration _hostConfiguration;
        readonly IEventHubProducerProvider _producerProvider;

        public EventHubRider(IEventHubHostConfiguration hostConfiguration, IReceiveEndpointCollection endpoints, IEventHubProducerProvider producerProvider)
        {
            _hostConfiguration = hostConfiguration;
            _endpoints = endpoints;
            _producerProvider = producerProvider;
        }

        public IEventHubProducerProvider GetProducerProvider(ConsumeContext consumeContext = default)
        {
            if (consumeContext == null)
                return _producerProvider;
            return new ConsumeContextEventHubProducerProvider(_producerProvider, consumeContext);
        }

        public RiderHandle Start(CancellationToken cancellationToken = default)
        {
            HostReceiveEndpointHandle[] endpointsHandle = _endpoints.StartEndpoints(cancellationToken);
            IAgent agent = new RiderAgent(_hostConfiguration.ConnectionContextSupervisor, _endpoints);
            return new Handle(endpointsHandle, agent);
        }


        class RiderAgent :
            Agent
        {
            readonly IReceiveEndpointCollection _endpoints;
            readonly IConnectionContextSupervisor _supervisor;

            public RiderAgent(IConnectionContextSupervisor supervisor, IReceiveEndpointCollection endpoints)
            {
                _supervisor = supervisor;
                _endpoints = endpoints;

                SetReady(_supervisor.Ready);
                SetCompleted(_supervisor.Completed);
            }

            protected override async Task StopAgent(StopContext context)
            {
                await _endpoints.Stop(context.CancellationToken).ConfigureAwait(false);
                await _supervisor.Stop(context).ConfigureAwait(false);
                await base.StopAgent(context).ConfigureAwait(false);
            }
        }


        class Handle :
            RiderHandle
        {
            readonly IAgent _agent;
            readonly HostReceiveEndpointHandle[] _endpoints;

            public Handle(HostReceiveEndpointHandle[] endpoints, IAgent agent)
            {
                _endpoints = endpoints;
                _agent = agent;
            }

            public Task Ready => ReadyOrNot(_endpoints.Select(x => x.Ready));

            public Task StopAsync(CancellationToken cancellationToken)
            {
                return _agent.Stop("EvenHub stopped", cancellationToken);
            }

            async Task ReadyOrNot(IEnumerable<Task<ReceiveEndpointReady>> endpoints)
            {
                Task<ReceiveEndpointReady>[] readyTasks = endpoints as Task<ReceiveEndpointReady>[] ?? endpoints.ToArray();
                foreach (Task<ReceiveEndpointReady> ready in readyTasks)
                    await ready.ConfigureAwait(false);

                await _agent.Ready.ConfigureAwait(false);

                await Task.WhenAll(readyTasks).ConfigureAwait(false);
            }
        }
    }
}
