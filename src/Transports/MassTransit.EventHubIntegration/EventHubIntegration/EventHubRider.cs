namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using MassTransit.Middleware;
    using Middleware;
    using Transports;


    public class EventHubRider :
        IEventHubRider
    {
        readonly IBusInstance _busInstance;
        readonly IReceiveEndpointCollection _endpoints;
        readonly IEventHubHostConfiguration _hostConfiguration;
        readonly IEventHubProducerProvider _producerProvider;
        readonly IRiderRegistrationContext _registrationContext;

        public EventHubRider(IEventHubHostConfiguration hostConfiguration, IBusInstance busInstance, IReceiveEndpointCollection endpoints,
            IEventHubProducerProvider producerProvider,
            IRiderRegistrationContext registrationContext)
        {
            _hostConfiguration = hostConfiguration;
            _busInstance = busInstance;
            _endpoints = endpoints;
            _producerProvider = producerProvider;
            _registrationContext = registrationContext;
        }

        public IEventHubProducerProvider GetProducerProvider(ConsumeContext consumeContext = default)
        {
            return consumeContext == null
                ? _producerProvider
                : new ConsumeContextEventHubProducerProvider(_producerProvider, consumeContext);
        }

        public HostReceiveEndpointHandle ConnectEventHubEndpoint(string eventHubName, string consumerGroup,
            Action<IRiderRegistrationContext, IEventHubReceiveEndpointConfigurator> configure)
        {
            var specification = _hostConfiguration.CreateSpecification(eventHubName, consumerGroup, configurator =>
            {
                configure?.Invoke(_registrationContext, configurator);
            });

            _endpoints.Add(specification.EndpointName, specification.CreateReceiveEndpoint(_busInstance));

            return _endpoints.Start(specification.EndpointName);
        }

        public RiderHandle Start(CancellationToken cancellationToken = default)
        {
            HostReceiveEndpointHandle[] endpointsHandle = _endpoints.StartEndpoints(cancellationToken);

            var ready = endpointsHandle.Length == 0 ? Task.CompletedTask : _hostConfiguration.ConnectionContextSupervisor.Ready;

            var agent = new RiderAgent(_hostConfiguration.ConnectionContextSupervisor, _endpoints, ready);

            return new Handle(endpointsHandle, agent);
        }


        class RiderAgent :
            Agent
        {
            readonly IReceiveEndpointCollection _endpoints;
            readonly IConnectionContextSupervisor _supervisor;

            public RiderAgent(IConnectionContextSupervisor supervisor, IReceiveEndpointCollection endpoints, Task ready)
            {
                _supervisor = supervisor;
                _endpoints = endpoints;

                SetReady(ready);
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
