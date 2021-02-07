namespace MassTransit.Futures.Pipeline
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Util;


    class FutureRequestPipe<T> :
        IPipe<SendContext<T>>
        where T : class
    {
        readonly Guid _requestId;
        readonly Uri _responseAddress;

        public FutureRequestPipe(Uri responseAddress, Guid requestId)
        {
            _responseAddress = responseAddress;
            _requestId = requestId;
        }

        public Task Send(SendContext<T> context)
        {
            context.ResponseAddress = _responseAddress;
            context.RequestId = _requestId;

            return TaskUtil.Completed;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope(nameof(FutureRequestPipe<T>));
        }
    }
}
