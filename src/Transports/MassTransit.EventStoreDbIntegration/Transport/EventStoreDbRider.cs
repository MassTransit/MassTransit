using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using GreenPipes.Agents;
using MassTransit;
using MassTransit.EventStoreDbIntegration.Contexts;
using MassTransit.Registration;
using MassTransit.Riders;
using MassTransit.Transports;
using MassTransit.Util;

namespace MassTransit.EventStoreDbIntegration
{
    public class EventStoreDbRider :
        IEventStoreDbRider
    {
        readonly IBusInstance _busInstance;
        readonly IReceiveEndpointCollection _endpoints;
        readonly IEventStoreDbHostConfiguration _hostConfiguration;
        readonly IEventStoreDbProducerProvider _producerProvider;
        readonly IRiderRegistrationContext _registrationContext;

        public EventStoreDbRider(IEventStoreDbHostConfiguration hostConfiguration, IBusInstance busInstance, IReceiveEndpointCollection endpoints,
            IEventStoreDbProducerProvider producerProvider,
            IRiderRegistrationContext registrationContext)
        {
            _hostConfiguration = hostConfiguration;
            _busInstance = busInstance;
            _endpoints = endpoints;
            _producerProvider = producerProvider;
            _registrationContext = registrationContext;
        }

        public IEventStoreDbProducerProvider GetProducerProvider(ConsumeContext consumeContext = null)
        {
            return consumeContext == null
                ? _producerProvider
                : new ConsumeContextEventStoreDbProducerProvider(_producerProvider, consumeContext);
        }

        public HostReceiveEndpointHandle ConnectEventStoreDbEndpoint(StreamCategory streamCategory, string subscriptionName,
            Action<IRiderRegistrationContext, IEventStoreDbReceiveEndpointConfigurator> configure)
        {
            var specification = _hostConfiguration.CreateSpecification(streamCategory, subscriptionName, configurator =>
            {
                configure?.Invoke(_registrationContext, configurator);
            });

            _endpoints.Add(specification.EndpointName, specification.CreateReceiveEndpoint(_busInstance));

            return _endpoints.Start(specification.EndpointName);
        }

        public RiderHandle Start(CancellationToken cancellationToken = default)
        {
            HostReceiveEndpointHandle[] endpointsHandle = _endpoints.StartEndpoints(cancellationToken);

            var ready = endpointsHandle.Length == 0 ? TaskUtil.Completed : _hostConfiguration.ConnectionContextSupervisor.Ready;

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
                return _agent.Stop("EvenStoreDB stopped", cancellationToken);
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
