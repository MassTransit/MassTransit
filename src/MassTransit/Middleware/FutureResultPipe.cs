namespace MassTransit.Middleware
{
    using System;
    using System.Threading.Tasks;


    public class FutureResultPipe<T> :
        IPipe<SendContext<T>>
        where T : class
    {
        readonly IPipe<SendContext<T>> _pipe;
        readonly Guid _requestId;

        public FutureResultPipe(IPipe<SendContext<T>> pipe, Guid requestId)
        {
            _pipe = pipe;
            _requestId = requestId;
        }

        public Task Send(SendContext<T> context)
        {
            context.RequestId = _requestId;

            return _pipe.IsNotEmpty() ? _pipe.Send(context) : Task.CompletedTask;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope(nameof(FutureResultPipe<T>));
        }
    }
}
