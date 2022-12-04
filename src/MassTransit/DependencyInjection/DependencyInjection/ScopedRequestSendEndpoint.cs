namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public class ScopedRequestSendEndpoint<TRequest> :
        IRequestSendEndpoint<TRequest>
        where TRequest : class
    {
        readonly IRequestSendEndpoint<TRequest> _endpoint;
        readonly IServiceProvider _serviceProvider;

        public ScopedRequestSendEndpoint(IRequestSendEndpoint<TRequest> endpoint, IServiceProvider serviceProvider)
        {
            _endpoint = endpoint;
            _serviceProvider = serviceProvider;
        }

        public Task<TRequest> Send(Guid requestId, object values, IPipe<SendContext<TRequest>> pipe, CancellationToken cancellationToken)
        {
            return _endpoint.Send(requestId, values, new ScopedSendPipeAdapter<TRequest>(_serviceProvider, pipe), cancellationToken);
        }

        public Task Send(Guid requestId, TRequest message, IPipe<SendContext<TRequest>> pipe, CancellationToken cancellationToken)
        {
            return _endpoint.Send(requestId, message, new ScopedSendPipeAdapter<TRequest>(_serviceProvider, pipe), cancellationToken);
        }
    }
}
