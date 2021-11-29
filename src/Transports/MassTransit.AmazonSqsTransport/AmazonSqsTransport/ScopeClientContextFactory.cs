namespace MassTransit.AmazonSqsTransport
{
    using System.Threading;
    using System.Threading.Tasks;
    using Agents;
    using Internals;


    public class ScopeClientContextFactory :
        IPipeContextFactory<ClientContext>
    {
        readonly IClientContextSupervisor _supervisor;

        public ScopeClientContextFactory(IClientContextSupervisor supervisor)
        {
            _supervisor = supervisor;
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
            return supervisor.AddActiveContext(context, CreateSharedModel(context.Context, cancellationToken));
        }

        static async Task<ClientContext> CreateSharedModel(Task<ClientContext> context, CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new SharedClientContext(context.Result, cancellationToken)
                : new SharedClientContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }

        void CreateClientContext(IAsyncPipeContextAgent<ClientContext> asyncContext, CancellationToken cancellationToken)
        {
            static Task<ClientContext> Create(ClientContext context, CancellationToken createCancellationToken)
            {
                return Task.FromResult<ClientContext>(new SharedClientContext(context, createCancellationToken));
            }

            _supervisor.CreateAgent(asyncContext, Create, cancellationToken);
        }
    }
}
