namespace MassTransit.AzureServiceBusTransport.Middleware
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Middleware;
    using Topology;


    public sealed class RemoveServiceBusTopologyAgent :
        Agent
    {
        readonly BrokerTopology _brokerTopology;
        readonly ConnectionContext _context;

        public RemoveServiceBusTopologyAgent(ConnectionContext context, BrokerTopology brokerTopology)
        {
            _brokerTopology = brokerTopology;
            _context = context;

            SetReady();
        }

        protected override async Task StopAgent(StopContext context)
        {
            try
            {
                await RemoveSubscriptions(_context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogContext.Warning?.Log(ex, "Failed to remove one or more subscriptions from the endpoint.");
            }

            await base.StopAgent(context);
        }

        async Task RemoveSubscriptions(ConnectionContext context)
        {
            await Task.WhenAll(_brokerTopology.QueueSubscriptions.Select(subscription => Delete(context, subscription))).ConfigureAwait(false);
        }

        Task Delete(ConnectionContext context, QueueSubscription subscription)
        {
            return context.DeleteTopicSubscription(subscription.Subscription.CreateSubscriptionOptions);
        }
    }
}
