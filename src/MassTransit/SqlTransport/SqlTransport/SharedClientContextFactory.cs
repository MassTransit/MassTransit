namespace MassTransit.SqlTransport
{
    using System.Threading;
    using System.Threading.Tasks;
    using Agents;
    using Internals;


    public class SharedClientContextFactory :
        IPipeContextFactory<ClientContext>
    {
        readonly IClientContextSupervisor _supervisor;

        public SharedClientContextFactory(IClientContextSupervisor supervisor)
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
            return supervisor.AddActiveContext(context, CreateScopeContext(context.Context, cancellationToken));
        }

        static async Task<ClientContext> CreateScopeContext(Task<ClientContext> context, CancellationToken cancellationToken)
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

            #pragma warning disable CS4014
            _supervisor.CreateAgent(asyncContext, Create, cancellationToken);
            #pragma warning restore CS4014
        }
    }
}
