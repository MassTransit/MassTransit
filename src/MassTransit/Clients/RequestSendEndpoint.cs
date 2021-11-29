namespace MassTransit.Clients
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Initializers;
    using Transports;


    public abstract class RequestSendEndpoint<TRequest> :
        IRequestSendEndpoint<TRequest>
        where TRequest : class
    {
        readonly ConsumeContext _consumeContext;

        protected RequestSendEndpoint(ConsumeContext consumeContext)
        {
            _consumeContext = consumeContext;
        }

        public async Task<TRequest> Send(Guid requestId, object values, IPipe<SendContext<TRequest>> pipe, CancellationToken cancellationToken)
        {
            var endpoint = await GetSendEndpoint().ConfigureAwait(false);

            (var message, IPipe<SendContext<TRequest>> sendPipe) = _consumeContext != null
                ? await MessageInitializerCache<TRequest>.InitializeMessage(_consumeContext, values,
                    new ConsumeSendPipeAdapter<TRequest>(_consumeContext, pipe, requestId)).ConfigureAwait(false)
                : await MessageInitializerCache<TRequest>.InitializeMessage(values, pipe, cancellationToken).ConfigureAwait(false);

            await endpoint.Send(message, sendPipe, cancellationToken).ConfigureAwait(false);

            return message;
        }

        public async Task Send(Guid requestId, TRequest message, IPipe<SendContext<TRequest>> pipe, CancellationToken cancellationToken)
        {
            var endpoint = await GetSendEndpoint().ConfigureAwait(false);

            IPipe<SendContext<TRequest>> consumePipe = _consumeContext != null
                ? new ConsumeSendPipeAdapter<TRequest>(_consumeContext, pipe, requestId)
                : pipe;

            await endpoint.Send(message, consumePipe, cancellationToken).ConfigureAwait(false);
        }

        protected abstract Task<ISendEndpoint> GetSendEndpoint();
    }
}
