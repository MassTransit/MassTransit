namespace MassTransit.AmazonSqsTransport.Middleware
{
    using System.Threading.Tasks;


    /// <summary>
    /// Purges the queue on startup, only once per filter instance
    /// </summary>
    public class PurgeOnStartupFilter :
        IFilter<ClientContext>
    {
        readonly string _queueName;
        bool _queueAlreadyPurged;

        public PurgeOnStartupFilter(string queueName)
        {
            _queueName = queueName;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("purgeOnStartup");
        }

        async Task IFilter<ClientContext>.Send(ClientContext context, IPipe<ClientContext> next)
        {
            await PurgeIfRequested(context, _queueName).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);
        }

        async Task PurgeIfRequested(ClientContext context, string queueName)
        {
            if (!_queueAlreadyPurged)
            {
                await context.PurgeQueue(queueName, context.CancellationToken).ConfigureAwait(false);

                LogContext.Debug?.Log("Purged queue {QueueName}", queueName);

                _queueAlreadyPurged = true;
            }
            else
                LogContext.Debug?.Log("Queue {QueueName} was purged at startup, skipping", queueName);
        }
    }
}
