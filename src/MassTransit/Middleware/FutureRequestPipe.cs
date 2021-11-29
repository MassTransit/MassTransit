namespace MassTransit.Middleware
{
    using System;
    using System.Threading.Tasks;


    public class FutureRequestPipe<T> :
        IPipe<SendContext<T>>
        where T : class
    {
        readonly IPipe<SendContext<T>> _pipe;
        readonly Guid _requestId;
        readonly Uri _responseAddress;

        public FutureRequestPipe(IPipe<SendContext<T>> pipe, Uri responseAddress, Guid requestId)
        {
            _pipe = pipe;
            _responseAddress = responseAddress;
            _requestId = requestId;
        }

        public Task Send(SendContext<T> context)
        {
            context.ResponseAddress = _responseAddress;
            context.RequestId = _requestId;

            return _pipe.IsNotEmpty() ? _pipe.Send(context) : Task.CompletedTask;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope(nameof(FutureRequestPipe<T>));
        }
    }
}
