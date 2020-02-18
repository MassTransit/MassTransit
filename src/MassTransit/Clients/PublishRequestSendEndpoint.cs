namespace MassTransit.Clients
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Initializers;


    public class PublishRequestSendEndpoint<TRequest> :
        IRequestSendEndpoint<TRequest>
        where TRequest : class
    {
        readonly IPublishEndpointProvider _provider;
        readonly ConsumeContext _consumeContext;

        public PublishRequestSendEndpoint(IPublishEndpointProvider provider, ConsumeContext consumeContext)
        {
            _provider = provider;
            _consumeContext = consumeContext;
        }

        public async Task<TRequest> Send(Guid requestId, object values, IPipe<SendContext<TRequest>> pipe, CancellationToken cancellationToken)
        {
            var endpoint = await _provider.GetPublishSendEndpoint<TRequest>().ConfigureAwait(false);

            var initializer = MessageInitializerCache<TRequest>.GetInitializer(values.GetType());

            if (_consumeContext != null)
            {
                var initializeContext = initializer.Create(_consumeContext);

                return await initializer.Send(endpoint, initializeContext, values, new ConsumeSendEndpointPipe<TRequest>(_consumeContext, pipe, requestId))
                    .ConfigureAwait(false);
            }

            return await initializer.Send(endpoint, values, pipe, cancellationToken).ConfigureAwait(false);
        }

        public async Task Send(Guid requestId, TRequest message, IPipe<SendContext<TRequest>> pipe, CancellationToken cancellationToken)
        {
            var endpoint = await _provider.GetPublishSendEndpoint<TRequest>().ConfigureAwait(false);

            IPipe<SendContext<TRequest>> consumePipe = _consumeContext != null
                ? new ConsumeSendEndpointPipe<TRequest>(_consumeContext, pipe, requestId)
                : pipe;

            await endpoint.Send(message, consumePipe, cancellationToken).ConfigureAwait(false);
        }
    }
}
