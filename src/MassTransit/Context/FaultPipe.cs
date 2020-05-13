namespace MassTransit.Context
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Util;


    public class FaultPipe<T> :
        IPipe<SendContext<Fault<T>>>
        where T : class
    {
        readonly ConsumeContext<T> _context;

        public FaultPipe(ConsumeContext<T> context)
        {
            _context = context;
        }

        public Task Send(SendContext<Fault<T>> context)
        {
            context.TransferConsumeContextHeaders(_context);

            context.CorrelationId = _context.CorrelationId;
            context.RequestId = _context.RequestId;

            if (_context.TryGetPayload(out ConsumeRetryContext consumeRetryContext) && consumeRetryContext.RetryCount > 0)
            {
                context.Headers.Set(MessageHeaders.FaultRetryCount, consumeRetryContext.RetryCount);
            }
            else if (_context.TryGetPayload(out RetryContext retryContext) && retryContext.RetryCount > 0)
            {
                context.Headers.Set(MessageHeaders.FaultRetryCount, retryContext.RetryCount);
            }

            return TaskUtil.Completed;
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
