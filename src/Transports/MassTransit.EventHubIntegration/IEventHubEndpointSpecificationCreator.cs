namespace MassTransit.EventHubIntegration
{
    using System;
    using Specifications;


    public interface IEventHubEndpointSpecificationCreator
    {
        IEventHubReceiveEndpointSpecification CreateSpecification(string eventHubName, string consumerGroup,
            Action<IEventHubReceiveEndpointConfigurator> configure);
    }
}
