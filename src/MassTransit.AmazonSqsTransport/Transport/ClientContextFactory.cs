namespace MassTransit.AmazonSqsTransport.Transport
{
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Internals.Extensions;


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
                ? new SharedClientContext(context.Result, cancellationToken)
                : new SharedClientContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }

        void CreateClientContext(IAsyncPipeContextAgent<ClientContext> asyncContext, CancellationToken cancellationToken)
        {
            static Task<ClientContext> CreateClientContext(ConnectionContext connectionContext, CancellationToken createCancellationToken)
            {
                return Task.FromResult(connectionContext.CreateClientContext(createCancellationToken));
            }

            _connectionContextSupervisor.CreateAgent(asyncContext, CreateClientContext, cancellationToken);
        }
    }
}
