namespace MassTransit.RabbitMqTransport.Middleware
{
    using System.Threading.Tasks;
    using RabbitMQ.Client;


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

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("purgeOnStartup");
        }

        async Task IFilter<ChannelContext>.Send(ChannelContext context, IPipe<ChannelContext> next)
        {
            var queueOk = await context.QueueDeclarePassive(_queueName).ConfigureAwait(false);

            if (queueOk.ConsumerCount == 0 && queueOk.MessageCount > 0)
                await PurgeIfRequested(context, queueOk, _queueName).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);
        }

        async Task PurgeIfRequested(ChannelContext context, QueueDeclareOk queueOk, string queueName)
        {
            if (!_queueAlreadyPurged)
            {
                var purgedMessageCount = await context.QueuePurge(queueName).ConfigureAwait(false);

                LogContext.Debug?.Log("Purged {MessageCount} messages from queue {QueueName}", purgedMessageCount, queueName);

                _queueAlreadyPurged = true;
            }
            else
                LogContext.Debug?.Log("Queue {QueueName} was purged at startup, skipping", queueName);
        }
    }
}
