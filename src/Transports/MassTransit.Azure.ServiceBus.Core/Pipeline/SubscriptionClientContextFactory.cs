namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using System;
    using Contexts;
    using Transport;


    public class SubscriptionClientContextFactory :
        ClientContextFactory
    {
        readonly SubscriptionSettings _settings;

        public SubscriptionClientContextFactory(IConnectionContextSupervisor supervisor, SubscriptionSettings settings)
            : base(supervisor, settings)
        {
            _settings = settings;
        }

        protected override ClientContext CreateClientContext(ConnectionContext connectionContext, Uri inputAddress)
        {
            var subscriptionClient = connectionContext.CreateSubscriptionClient(_settings.TopicDescription.Path,
                _settings.SubscriptionDescription.SubscriptionName);

            subscriptionClient.PrefetchCount = _settings.PrefetchCount;

            return new SubscriptionClientContext(connectionContext, subscriptionClient, inputAddress, _settings);
        }
    }
}
