namespace MassTransit.Futures.Pipeline
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit;
    using Util;


    class FutureResultPipe<T> :
        IPipe<SendContext<T>>
        where T : class
    {
        readonly Guid _requestId;

        public FutureResultPipe(Guid requestId)
        {
            _requestId = requestId;
        }

        public Task Send(SendContext<T> context)
        {
            context.RequestId = _requestId;

            return TaskUtil.Completed;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope(nameof(FutureResultPipe<T>));
        }
    }
}
