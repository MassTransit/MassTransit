namespace MassTransit.ActiveMqTransport
{
    using System.Threading;
    using System.Threading.Tasks;
    using Agents;
    using Internals;


    public class ScopeSessionContextFactory :
        IPipeContextFactory<SessionContext>
    {
        readonly ISessionContextSupervisor _supervisor;

        public ScopeSessionContextFactory(ISessionContextSupervisor supervisor)
        {
            _supervisor = supervisor;
        }

        IPipeContextAgent<SessionContext> IPipeContextFactory<SessionContext>.CreateContext(ISupervisor supervisor)
        {
            IAsyncPipeContextAgent<SessionContext> asyncContext = supervisor.AddAsyncContext<SessionContext>();

            CreateSession(asyncContext, supervisor.Stopped);

            return asyncContext;
        }

        IActivePipeContextAgent<SessionContext> IPipeContextFactory<SessionContext>.CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<SessionContext> context, CancellationToken cancellationToken)
        {
            return supervisor.AddActiveContext(context, CreateSharedSession(context.Context, cancellationToken));
        }

        static async Task<SessionContext> CreateSharedSession(Task<SessionContext> context, CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new SharedSessionContext(context.Result, cancellationToken)
                : new SharedSessionContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }

        void CreateSession(IAsyncPipeContextAgent<SessionContext> asyncContext, CancellationToken cancellationToken)
        {
            static Task<SessionContext> CreateSessionContext(SessionContext context, CancellationToken createCancellationToken)
            {
                return Task.FromResult<SessionContext>(new SharedSessionContext(context, createCancellationToken));
            }

            _supervisor.CreateAgent(asyncContext, CreateSessionContext, cancellationToken);
        }
    }
}
