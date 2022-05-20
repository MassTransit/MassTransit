namespace MassTransit.AmazonSqsTransport.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Middleware;
    using Topology;


    public sealed class RemoveAmazonSqsTopologyAgent :
        Agent
    {
        readonly BrokerTopology _brokerTopology;
        readonly ClientContext _context;

        public RemoveAmazonSqsTopologyAgent(ClientContext context, BrokerTopology brokerTopology)
        {
            _brokerTopology = brokerTopology;
            _context = context;

            SetReady();
        }

        protected override async Task StopAgent(StopContext context)
        {
            try
            {
                await DeleteAutoDelete(_context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogContext.Warning?.Log(ex, "Failed to remove one or more queues, topics, or subscriptions from the endpoint.");
            }

            await base.StopAgent(context);
        }

        async Task DeleteAutoDelete(ClientContext context)
        {
            IEnumerable<Task> topics = _brokerTopology.Topics.Where(x => x.AutoDelete).Select(topic => Delete(context, topic));

            IEnumerable<Task> queues = _brokerTopology.Queues.Where(x => x.AutoDelete).Select(queue => Delete(context, queue));

            await Task.WhenAll(topics.Concat(queues)).ConfigureAwait(false);
        }

        static Task Delete(ClientContext context, Topic topic)
        {
            LogContext.Debug?.Log("Delete topic {Topic}", topic);

            return context.DeleteTopic(topic);
        }

        static Task Delete(ClientContext context, Queue queue)
        {
            LogContext.Debug?.Log("Delete queue {Queue}", queue);

            return context.DeleteQueue(queue);
        }
    }
}
