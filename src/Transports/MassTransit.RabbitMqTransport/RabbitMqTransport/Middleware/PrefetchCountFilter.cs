namespace MassTransit.RabbitMqTransport.Middleware
{
    using System.Threading.Tasks;


    /// <summary>
    /// Prepares a queue for receiving messages using the ReceiveSettings specified.
    /// </summary>
    public class PrefetchCountFilter :
        IFilter<ChannelContext>
    {
        ushort _prefetchCount;

        public PrefetchCountFilter(ushort prefetchCount)
        {
            _prefetchCount = prefetchCount;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("prefetchCount");
            scope.Add("prefetchCount", _prefetchCount);
        }

        async Task IFilter<ChannelContext>.Send(ChannelContext context, IPipe<ChannelContext> next)
        {
            await context.BasicQos(0, _prefetchCount, false).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);
        }

        public Task SetPrefetchCount(ushort prefetchCount)
        {
            _prefetchCount = prefetchCount;

            return Task.CompletedTask;
        }
    }
}
