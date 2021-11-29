namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Agents;
    using Internals;


    public abstract class ClientContextFactory :
        IPipeContextFactory<ClientContext>
    {
        readonly ClientSettings _settings;
        readonly IConnectionContextSupervisor _supervisor;

        protected ClientContextFactory(IConnectionContextSupervisor supervisor, ClientSettings settings)
        {
            _supervisor = supervisor;
            _settings = settings;
        }

        public IPipeContextAgent<ClientContext> CreateContext(ISupervisor supervisor)
        {
            IAsyncPipeContextAgent<ClientContext> asyncContext = supervisor.AddAsyncContext<ClientContext>();

            CreateClientContext(asyncContext, supervisor.Stopped);

            return asyncContext;
        }

        public IActivePipeContextAgent<ClientContext> CreateActiveContext(ISupervisor supervisor, PipeContextHandle<ClientContext> context,
            CancellationToken cancellationToken)
        {
            return supervisor.AddActiveContext(context, CreateSharedContext(context.Context, cancellationToken));
        }

        protected abstract ClientContext CreateClientContext(ConnectionContext connectionContext, Uri inputAddress, IAgent agent);

        void CreateClientContext(IAsyncPipeContextAgent<ClientContext> asyncContext, CancellationToken cancellationToken)
        {
            Task<ClientContext> Create(ConnectionContext connectionContext, CancellationToken createCancellationToken)
            {
                var inputAddress = _settings.GetInputAddress(connectionContext.Endpoint, _settings.Path);

                return Task.FromResult(CreateClientContext(connectionContext, inputAddress, asyncContext));
            }

            _supervisor.CreateAgent(asyncContext, Create, cancellationToken);
        }

        static async Task<ClientContext> CreateSharedContext(Task<ClientContext> context, CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new SharedClientContext(context.Result, cancellationToken)
                : new SharedClientContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }
    }
}
