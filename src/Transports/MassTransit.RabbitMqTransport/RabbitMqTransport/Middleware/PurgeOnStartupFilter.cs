namespace MassTransit.RabbitMqTransport.Middleware
{
    using System.Threading.Tasks;


    /// <summary>
    /// Purges the queue on startup, only once per filter instance
    /// </summary>
    public class PurgeOnStartupFilter :
        IFilter<ChannelContext>
    {
        readonly string _queueName;
        bool _queueAlreadyPurged;

        public PurgeOnStartupFilter(string queueName)
        {
            _queueName = queueName;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("purgeOnStartup");
        }

        public async Task Send(ChannelContext context, IPipe<ChannelContext> next)
        {
            var queueOk = await context.QueueDeclarePassive(_queueName, context.CancellationToken).ConfigureAwait(false);

            if (queueOk.ConsumerCount == 0 && queueOk.MessageCount > 0)
                await PurgeIfRequested(context, _queueName).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);
        }

        async Task PurgeIfRequested(ChannelContext context, string queueName)
        {
            if (!_queueAlreadyPurged)
            {
                var purgedMessageCount = await context.QueuePurge(queueName, context.CancellationToken).ConfigureAwait(false);

                LogContext.Debug?.Log("Purged {MessageCount} messages from queue {QueueName}", purgedMessageCount, queueName);

                _queueAlreadyPurged = true;
            }
            else
                LogContext.Debug?.Log("Queue {QueueName} was purged at startup, skipping", queueName);
        }
    }
}
