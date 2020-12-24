namespace MassTransit.EventHubIntegration
{
    using System;
    using MassTransit.Registration;
    using Transports;


    public class EvenHubEndpointConnector :
        IEvenHubEndpointConnector
    {
        readonly IBusInstance _busInstance;
        readonly IReceiveEndpointCollection _endpoints;
        readonly IEventHubEndpointSpecificationCreator _specificationCreator;

        public EvenHubEndpointConnector(IBusInstance busInstance, IReceiveEndpointCollection endpoints,
            IEventHubEndpointSpecificationCreator specificationCreator)
        {
            _busInstance = busInstance;
            _endpoints = endpoints;
            _specificationCreator = specificationCreator;
        }

        public HostReceiveEndpointHandle ConnectEventHubEndpoint(string eventHubName, string consumerGroup,
            Action<IEventHubReceiveEndpointConfigurator> configure)
        {
            var specification = _specificationCreator.CreateSpecification(eventHubName, consumerGroup, configure);
            _endpoints.Add(specification.EndpointName, specification.CreateReceiveEndpoint(_busInstance));
            return _endpoints.Start(specification.EndpointName);
        }
    }
}
