namespace MassTransit.AmazonSqsTransport
{
    using System.Threading;
    using System.Threading.Tasks;
    using Agents;
    using Internals;


    public class ClientContextFactory :
        IPipeContextFactory<ClientContext>
    {
        readonly IConnectionContextSupervisor _connectionContextSupervisor;

        public ClientContextFactory(IConnectionContextSupervisor connectionContextSupervisor)
        {
            _connectionContextSupervisor = connectionContextSupervisor;
        }

        IPipeContextAgent<ClientContext> IPipeContextFactory<ClientContext>.CreateContext(ISupervisor supervisor)
        {
            IAsyncPipeContextAgent<ClientContext> asyncContext = supervisor.AddAsyncContext<ClientContext>();

            CreateClientContext(asyncContext, supervisor.Stopped);

            return asyncContext;
        }

        IActivePipeContextAgent<ClientContext> IPipeContextFactory<ClientContext>.CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<ClientContext> context, CancellationToken cancellationToken)
        {
            return supervisor.AddActiveContext(context, CreateSharedClientContext(context.Context, cancellationToken));
        }

        static async Task<ClientContext> CreateSharedClientContext(Task<ClientContext> context, CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new ScopeClientContext(context.Result, cancellationToken)
                : new ScopeClientContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }

        void CreateClientContext(IAsyncPipeContextAgent<ClientContext> asyncContext, CancellationToken cancellationToken)
        {
            static Task<ClientContext> Create(ConnectionContext connectionContext, CancellationToken createCancellationToken)
            {
                return Task.FromResult(connectionContext.CreateClientContext(createCancellationToken));
            }

            _connectionContextSupervisor.CreateAgent(asyncContext, Create, cancellationToken);
        }
    }
}
