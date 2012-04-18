namespace MassTransit.Distributor.Configuration
{
    using System.Collections.Generic;
    using SubscriptionConfigurators;

    /// <summary>
    /// Decorates subscriptions added through this interface to the subscription
    /// bus service with the distributor components
    /// </summary>
    public class WorkerConfiguratorImpl :
        WorkerConfigurator
    {
        readonly SubscriptionBusServiceConfigurator _configurator;
        readonly IList<SubscriptionBusServiceBuilderConfigurator> _configurators;

        public WorkerConfiguratorImpl(SubscriptionBusServiceConfigurator configurator)
        {
            _configurator = configurator;
            _configurators = new List<SubscriptionBusServiceBuilderConfigurator>();
        }

        public void AddConfigurator(WorkerBuilderConfigurator configurator)
        {
            throw new System.NotImplementedException();
        }
    }
}