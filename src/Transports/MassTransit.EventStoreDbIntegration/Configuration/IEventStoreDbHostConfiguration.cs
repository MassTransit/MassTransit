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
        IConnectionContextSupervisor ConnectionContextSupervisor { get; }

        IEventStoreDbSubscriptionSpecification CreateCatchupSubscriptionSpecification(StreamCategory streamCategory, string subscriptionName,
            Action<IEventStoreDbCatchupSubscriptionConfigurator> configure);

        IEventStoreDbRider Build(IRiderRegistrationContext context, IBusInstance busInstance);
    }
}
