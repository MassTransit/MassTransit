namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public class ScopedRequestSendEndpoint<TScope, TRequest> :
        IRequestSendEndpoint<TRequest>
        where TRequest : class
        where TScope : class
    {
        readonly IRequestSendEndpoint<TRequest> _endpoint;
        readonly TScope _scope;

        public ScopedRequestSendEndpoint(IRequestSendEndpoint<TRequest> endpoint, TScope scope)
        {
            _endpoint = endpoint;
            _scope = scope;
        }

        public Task<TRequest> Send(Guid requestId, object values, IPipe<SendContext<TRequest>> pipe, CancellationToken cancellationToken)
        {
            return _endpoint.Send(requestId, values, new ScopedSendPipeAdapter<TScope, TRequest>(_scope, pipe), cancellationToken);
        }

        public Task Send(Guid requestId, TRequest message, IPipe<SendContext<TRequest>> pipe, CancellationToken cancellationToken)
        {
            return _endpoint.Send(requestId, message, new ScopedSendPipeAdapter<TScope, TRequest>(_scope, pipe), cancellationToken);
        }
    }
}
