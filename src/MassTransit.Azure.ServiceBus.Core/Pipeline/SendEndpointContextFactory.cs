namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Internals.Extensions;


    public abstract class SendEndpointContextFactory :
        IPipeContextFactory<SendEndpointContext>
    {
        readonly IConnectionContextSupervisor _supervisor;
        readonly IPipe<SendEndpointContext> _pipe;

        protected SendEndpointContextFactory(IConnectionContextSupervisor supervisor, IPipe<SendEndpointContext> pipe)
        {
            _supervisor = supervisor;
            _pipe = pipe;
        }

        protected abstract SendEndpointContext CreateSendEndpointContext(ConnectionContext context);

        public IPipeContextAgent<SendEndpointContext> CreateContext(ISupervisor supervisor)
        {
            IAsyncPipeContextAgent<SendEndpointContext> asyncContext = supervisor.AddAsyncContext<SendEndpointContext>();

            CreateSendEndpointContext(asyncContext, supervisor.Stopped);

            return asyncContext;
        }

        public IActivePipeContextAgent<SendEndpointContext> CreateActiveContext(ISupervisor supervisor, PipeContextHandle<SendEndpointContext> context,
            CancellationToken cancellationToken)
        {
            return supervisor.AddActiveContext(context, CreateSharedContext(context.Context, cancellationToken));
        }

        void CreateSendEndpointContext(IAsyncPipeContextAgent<SendEndpointContext> asyncContext, CancellationToken cancellationToken)
        {
            async Task<SendEndpointContext> Create(ConnectionContext context, CancellationToken createCancellationToken)
            {
                var sendEndpointContext = CreateSendEndpointContext(context);

                if (_pipe.IsNotEmpty())
                    await _pipe.Send(sendEndpointContext).ConfigureAwait(false);

                return sendEndpointContext;
            }

            _supervisor.CreateAgent(asyncContext, Create, cancellationToken);
        }

        static async Task<SendEndpointContext> CreateSharedContext(Task<SendEndpointContext> context, CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new SharedSendEndpointContext(context.Result, cancellationToken)
                : new SharedSendEndpointContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }
    }
}
