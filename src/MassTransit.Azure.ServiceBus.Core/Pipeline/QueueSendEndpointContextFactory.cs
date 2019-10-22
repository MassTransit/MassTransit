namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes;
    using Transport;


    public class QueueSendEndpointContextFactory :
        JoinContextFactory<NamespaceContext, MessagingFactoryContext, SendEndpointContext>
    {
        readonly SendSettings _settings;

        public QueueSendEndpointContextFactory(IMessagingFactoryContextSupervisor messagingFactoryContextSupervisor,
            INamespaceContextSupervisor namespaceContextSupervisor, IPipe<MessagingFactoryContext> messagingFactoryPipe,
            IPipe<NamespaceContext> namespacePipe, SendSettings settings)
            : base(namespaceContextSupervisor, namespacePipe, messagingFactoryContextSupervisor, messagingFactoryPipe)
        {
            _settings = settings;
        }

        protected override SendEndpointContext CreateClientContext(NamespaceContext leftContext, MessagingFactoryContext rightContext)
        {
            LogContext.Debug?.Log("Creating Queue Client: {Queue}", _settings.EntityPath);

            var messageSender = rightContext.MessagingFactory.CreateMessageSender(_settings.EntityPath);

            return new QueueSendEndpointContext(messageSender);
        }

        protected override async Task<SendEndpointContext> CreateSharedContext(Task<SendEndpointContext> context, CancellationToken cancellationToken)
        {
            var sendEndpointContext = await context.ConfigureAwait(false);

            return new SharedSendEndpointContext(sendEndpointContext, cancellationToken);
        }
    }
}
