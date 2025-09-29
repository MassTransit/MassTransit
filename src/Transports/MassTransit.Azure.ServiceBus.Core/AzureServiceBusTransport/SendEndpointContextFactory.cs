namespace MassTransit.AzureServiceBusTransport
{
    using System.Threading;
    using System.Threading.Tasks;
    using Agents;
    using Internals;
    using Middleware;


    public class SendEndpointContextFactory :
        IPipeContextFactory<SendEndpointContext>
    {
        readonly ConfigureServiceBusTopologyFilter<SendSettings> _configureTopologyFilter;
        readonly SendSettings _settings;
        readonly IConnectionContextSupervisor _supervisor;

        public SendEndpointContextFactory(IConnectionContextSupervisor supervisor, ConfigureServiceBusTopologyFilter<SendSettings> configureTopologyFilter,
            SendSettings settings)
        {
            _supervisor = supervisor;
            _configureTopologyFilter = configureTopologyFilter;
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

                await _configureTopologyFilter.Configure(sendEndpointContext, createCancellationToken).ConfigureAwait(false);

                return sendEndpointContext;
            }

            #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            _supervisor.CreateAgent(asyncContext, Create, cancellationToken);
            #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        static async Task<SendEndpointContext> CreateSharedContext(Task<SendEndpointContext> context, CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new SharedSendEndpointContext(context.Result, cancellationToken)
                : new SharedSendEndpointContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }
    }
}
