namespace MassTransit.Scoping
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Clients;
    using GreenPipes;
    using Pipeline;
    using Util;


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
            return _endpoint.Send(requestId, values, new PayloadPipe<TRequest>(_scope, pipe), cancellationToken);
        }

        public Task Send(Guid requestId, TRequest message, IPipe<SendContext<TRequest>> pipe, CancellationToken cancellationToken)
        {
            return _endpoint.Send(requestId, message, new PayloadPipe<TRequest>(_scope, pipe), cancellationToken);
        }

        class PayloadPipe<TMessage> :
            IPipe<SendContext<TMessage>>,
            ISendContextPipe
            where TMessage : class
        {
            readonly TScope _payload;
            readonly IPipe<SendContext<TMessage>> _pipe;

            public PayloadPipe(TScope payload, IPipe<SendContext<TMessage>> pipe = default)
            {
                _payload = payload;
                _pipe = pipe;
            }

            Task ISendContextPipe.Send<T>(SendContext<T> context)
                where T : class
            {
                context.GetOrAddPayload(() => _payload);

                return _pipe is ISendContextPipe sendContextPipe
                    ? sendContextPipe.Send(context)
                    : TaskUtil.Completed;
            }

            public Task Send(SendContext<TMessage> context)
            {
                return _pipe.IsNotEmpty() ? _pipe.Send(context) : Task.CompletedTask;
            }

            public void Probe(ProbeContext context)
            {
                _pipe?.Probe(context);
            }
        }
    }
}
