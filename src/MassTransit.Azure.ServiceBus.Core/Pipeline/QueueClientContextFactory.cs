namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using System;
    using Contexts;
    using GreenPipes;
    using Transport;


    public class QueueClientContextFactory :
        ClientContextFactory
    {
        readonly ReceiveSettings _settings;

        public QueueClientContextFactory(IMessagingFactoryContextSupervisor messagingFactoryContextSupervisor,
            INamespaceContextSupervisor namespaceContextSupervisor, IPipe<MessagingFactoryContext> messagingFactoryPipe,
            IPipe<NamespaceContext> namespacePipe, ReceiveSettings settings)
            : base(messagingFactoryContextSupervisor, namespaceContextSupervisor, messagingFactoryPipe, namespacePipe, settings)
        {
            _settings = settings;
        }

        protected override ClientContext CreateClientContext(MessagingFactoryContext connectionContext, Uri inputAddress)
        {
            var queueClient = connectionContext.MessagingFactory.CreateQueueClient(_settings.Path);
            queueClient.PrefetchCount = _settings.PrefetchCount;

            return new QueueClientContext(queueClient, inputAddress, _settings);
        }
    }
}
