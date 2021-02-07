namespace MassTransit.Futures.Endpoints
{
    using System.Threading.Tasks;
    using Context;
    using Initializers;
    using Pipeline;


    public class InitializerRequestEndpoint<TCommand, TRequest> :
        IRequestEndpoint<TCommand, TRequest>
        where TCommand : class
        where TRequest : class
    {
        readonly InitializerValueProvider<TCommand> _provider;
        RequestAddressProvider<TCommand> _addressProvider;
        PendingIdProvider<TRequest> _pendingIdProvider;

        public InitializerRequestEndpoint(RequestAddressProvider<TCommand> addressProvider, PendingIdProvider<TRequest> pendingIdProvider,
            InitializerValueProvider<TCommand> provider)
        {
            _addressProvider = addressProvider;
            _pendingIdProvider = pendingIdProvider;
            _provider = provider;
        }

        public RequestAddressProvider<TCommand> RequestAddressProvider
        {
            set => _addressProvider = value;
        }

        public PendingIdProvider<TRequest> PendingIdProvider
        {
            set => _pendingIdProvider = value;
        }

        public async Task SendCommand(FutureConsumeContext<TCommand> context)
        {
            InitializeContext<TRequest> initializeContext = await MessageInitializerCache<TRequest>.Initialize(context.Message, context.CancellationToken);

            var destinationAddress = _addressProvider(context);

            var endpoint = destinationAddress != null
                ? await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false)
                : new ConsumeSendEndpoint(await context.ReceiveContext.PublishEndpointProvider.GetPublishSendEndpoint<TRequest>().ConfigureAwait(false),
                    context, default);

            var pipe = new FutureRequestPipe<TRequest>(context.ReceiveContext.InputAddress, context.Instance.CorrelationId);

            var values = _provider(context);

            IMessageInitializer<TRequest> messageInitializer = MessageInitializerCache<TRequest>.GetInitializer(values.GetType());

            var command = await messageInitializer.Send(endpoint, initializeContext, values, pipe).ConfigureAwait(false);

            if (_pendingIdProvider != null)
            {
                var pendingId = _pendingIdProvider(command);
                context.Instance.Pending.Add(pendingId);
            }
        }
    }
}
