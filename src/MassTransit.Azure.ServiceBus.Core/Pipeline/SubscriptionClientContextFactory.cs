namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using System;
    using Contexts;
    using GreenPipes;
    using Transport;


    public class SubscriptionClientContextFactory :
        ClientContextFactory
    {
        readonly SubscriptionSettings _settings;

        public SubscriptionClientContextFactory(IMessagingFactoryContextSupervisor messagingFactoryContextSupervisor,
            INamespaceContextSupervisor namespaceContextSupervisor, IPipe<MessagingFactoryContext> messagingFactoryPipe,
            IPipe<NamespaceContext> namespacePipe, SubscriptionSettings settings)
            : base(messagingFactoryContextSupervisor, namespaceContextSupervisor, messagingFactoryPipe, namespacePipe, settings)
        {
            _settings = settings;
        }

        protected override ClientContext CreateClientContext(MessagingFactoryContext connectionContext, Uri inputAddress)
        {
            var subscriptionClient = connectionContext.MessagingFactory.CreateSubscriptionClient(_settings.TopicDescription.Path,
                _settings.SubscriptionDescription.SubscriptionName);

            subscriptionClient.PrefetchCount = _settings.PrefetchCount;

            return new SubscriptionClientContext(subscriptionClient, inputAddress, _settings);
        }
    }
}
