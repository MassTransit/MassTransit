using System;
using GreenPipes;
using MassTransit.EventStoreDbIntegration.Contexts;
using MassTransit.EventStoreDbIntegration.Specifications;
using MassTransit.Registration;

namespace MassTransit.EventStoreDbIntegration
{
    public interface IEventStoreDbHostConfiguration :
        ISpecification
    {
        IClientContextSupervisor ConnectionContextSupervisor { get; }

        IEventStoreDbReceiveEndpointSpecification CreateSpecification(StreamCategory streamCategory, string subscriptionName,
            Action<IEventStoreDbReceiveEndpointConfigurator> configure);

        IEventStoreDbRider Build(IRiderRegistrationContext context, IBusInstance busInstance);
    }
}
