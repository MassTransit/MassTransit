namespace MassTransit.AzureServiceBusTransport
{
    using System.Threading;
    using System.Threading.Tasks;
    using Agents;
    using Internals;


    public class SendEndpointContextFactory :
        IPipeContextFactory<SendEndpointContext>
    {
        readonly IPipe<SendEndpointContext> _pipe;
        readonly SendSettings _settings;
        readonly IConnectionContextSupervisor _supervisor;

        public SendEndpointContextFactory(IConnectionContextSupervisor supervisor, IPipe<SendEndpointContext> pipe, SendSettings settings)
        {
            _supervisor = supervisor;
            _pipe = pipe;
            _settings = settings;
        }

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
                var messageSender = context.CreateMessageSender(_settings.EntityPath);

                var sendEndpointContext = new MessageSendEndpointContext(context, messageSender);

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
