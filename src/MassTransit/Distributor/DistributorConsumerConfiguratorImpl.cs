namespace MassTransit
{
    using System;
    using Distributor;
    using Distributor.Configuration;
    using SubscriptionConfigurators;

    public class DistributorConsumerConfiguratorImpl<TConsumer> :
        DistributorConsumerConfigurator<TConsumer>,
        DistributorBuilderConfigurator
        where TConsumer : class
    {
        readonly ConsumerSubscriptionConfigurator<TConsumer> _consumerConfigurator;

        public DistributorConsumerConfiguratorImpl(ConsumerSubscriptionConfigurator<TConsumer> consumerConfigurator)
        {
            _consumerConfigurator = consumerConfigurator;
        }

        public DistributorConsumerConfigurator<TConsumer> UseWorkerSelector(Func<IWorkerSelectionStrategy<TConsumer>> selector)
        {
            throw new NotImplementedException();
        }
    }
}