namespace MassTransit.Futures.Endpoints
{
    using System.Threading.Tasks;
    using Pipeline;


    public class FactoryRequestEndpoint<TCommand, TRequest> :
        IRequestEndpoint<TCommand, TRequest>
        where TCommand : class
        where TRequest : class
    {
        readonly AsyncFutureMessageFactory<TCommand, TRequest> _factory;
        RequestAddressProvider<TCommand> _addressProvider;
        PendingIdProvider<TRequest> _pendingIdProvider;

        public FactoryRequestEndpoint(RequestAddressProvider<TCommand> addressProvider, PendingIdProvider<TRequest> pendingIdProvider,
            AsyncFutureMessageFactory<TCommand, TRequest> factory)
        {
            _addressProvider = addressProvider;
            _pendingIdProvider = pendingIdProvider;
            _factory = factory;
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
            var command = await _factory(context).ConfigureAwait(false);

            var destinationAddress = _addressProvider(context);

            var pipe = new FutureRequestPipe<TRequest>(context.ReceiveContext.InputAddress, context.Instance.CorrelationId);

            if (destinationAddress != null)
            {
                var endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

                await endpoint.Send(command, pipe, context.CancellationToken).ConfigureAwait(false);
            }
            else
                await context.Publish(command, pipe, context.CancellationToken).ConfigureAwait(false);

            if (_pendingIdProvider != null)
            {
                var pendingId = _pendingIdProvider(command);
                context.Instance.Pending.Add(pendingId);
            }
        }
    }
}
