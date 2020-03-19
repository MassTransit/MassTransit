namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using Transport;


    public abstract class ClientContextFactory :
        JoinContextFactory<NamespaceContext, MessagingFactoryContext, ClientContext>
    {
        readonly ClientSettings _settings;

        protected ClientContextFactory(IMessagingFactoryContextSupervisor messagingFactoryContextSupervisor,
            INamespaceContextSupervisor namespaceContextSupervisor, IPipe<MessagingFactoryContext> messagingFactoryPipe,
            IPipe<NamespaceContext> namespacePipe, ClientSettings settings)
            : base(namespaceContextSupervisor, namespacePipe, messagingFactoryContextSupervisor, messagingFactoryPipe)
        {
            _settings = settings;
        }

        protected override ClientContext CreateClientContext(NamespaceContext leftContext, MessagingFactoryContext rightContext)
        {
            var inputAddress = _settings.GetInputAddress(rightContext.ServiceAddress, _settings.Path);

            return CreateClientContext(rightContext, inputAddress);
        }

        protected abstract ClientContext CreateClientContext(MessagingFactoryContext connectionContext, Uri inputAddress);

        protected override async Task<ClientContext> CreateSharedContext(Task<ClientContext> context, CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new SharedClientContext(context.Result, cancellationToken)
                : new SharedClientContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }
    }
}
