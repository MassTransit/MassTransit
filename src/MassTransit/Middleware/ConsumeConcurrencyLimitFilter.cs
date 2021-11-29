namespace MassTransit.Middleware
{
    using System.Threading.Tasks;


    /// <summary>
    /// A concurrency limit filter that is shared by multiple message types, so that a consumer
    /// accepting those various types can be limited to a specific number of consumer instances.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class ConsumeConcurrencyLimitFilter<TMessage> :
        IFilter<ConsumeContext<TMessage>>
        where TMessage : class
    {
        readonly IConcurrencyLimiter _limiter;

        public ConsumeConcurrencyLimitFilter(IConcurrencyLimiter limiter)
        {
            _limiter = limiter;
        }

        public async Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            await _limiter.Wait(context.CancellationToken).ConfigureAwait(false);

            try
            {
                await next.Send(context).ConfigureAwait(false);
            }
            finally
            {
                _limiter.Release();
            }
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("concurrencyLimit");
            scope.Set(new
            {
                _limiter.Limit,
                _limiter.Available
            });
        }
    }
}
