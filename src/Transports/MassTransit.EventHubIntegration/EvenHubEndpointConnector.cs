namespace MassTransit.EventHubIntegration
{
    using System;
    using MassTransit.Registration;
    using Transports;


    public class EvenHubEndpointConnector :
        IEvenHubEndpointConnector
    {
        readonly IBusInstance _busInstance;
        readonly IRiderRegistrationContext _context;
        readonly IReceiveEndpointCollection _endpoints;
        readonly IEventHubEndpointSpecificationCreator _specificationCreator;

        public EvenHubEndpointConnector(IRiderRegistrationContext context, IBusInstance busInstance, IReceiveEndpointCollection endpoints,
            IEventHubEndpointSpecificationCreator specificationCreator)
        {
            _context = context;
            _busInstance = busInstance;
            _endpoints = endpoints;
            _specificationCreator = specificationCreator;
        }

        public HostReceiveEndpointHandle ConnectEventHubEndpoint(string eventHubName, string consumerGroup,
            Action<IRiderRegistrationContext, IEventHubReceiveEndpointConfigurator> configure)
        {
            var configurator = new Configurator(_context, configure);
            var specification = _specificationCreator.CreateSpecification(eventHubName, consumerGroup, configurator.Configure);
            _endpoints.Add(specification.EndpointName, specification.CreateReceiveEndpoint(_busInstance));
            return _endpoints.Start(specification.EndpointName);
        }


        class Configurator
        {
            readonly Action<IRiderRegistrationContext, IEventHubReceiveEndpointConfigurator> _configure;
            readonly IRiderRegistrationContext _context;

            public Configurator(IRiderRegistrationContext context, Action<IRiderRegistrationContext, IEventHubReceiveEndpointConfigurator> configure)
            {
                _context = context;
                _configure = configure;
            }

            public void Configure(IEventHubReceiveEndpointConfigurator configurator)
            {
                _configure?.Invoke(_context, configurator);
            }
        }
    }
}
