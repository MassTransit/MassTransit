namespace MassTransit.ActiveMqTransport.Middleware;

using System;
using System.Linq;
using System.Threading.Tasks;
using Apache.NMS.ActiveMQ;
using MassTransit.Middleware;
using Topology;


public sealed class RemoveAutoDeleteAgent :
    Agent
{
    readonly BrokerTopology _brokerTopology;
    readonly SessionContext _context;

    public RemoveAutoDeleteAgent(SessionContext context, BrokerTopology brokerTopology)
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
            LogContext.Warning?.Log(ex, "Failed to remove one or more subscriptions from the endpoint.");
        }

        await base.StopAgent(context);
    }

    async Task DeleteAutoDelete(SessionContext context)
    {
        try
        {
            await Task.WhenAll(_brokerTopology.Consumers.Where(x => x.Destination.AutoDelete).Select(consumer => Delete(context, consumer.Destination)))
                .ConfigureAwait(false);

            await Task.WhenAll(_brokerTopology.Topics.Where(x => x.AutoDelete).Select(topic => Delete(context, topic))).ConfigureAwait(false);

            await Task.WhenAll(_brokerTopology.Queues.Where(x => x.AutoDelete).Select(queue => Delete(context, queue))).ConfigureAwait(false);
        }
        catch (ConnectionClosedException exception)
        {
            LogContext.Debug?.Log(exception, "Connection was closed, auto-delete queues/topics/consumers could not be deleted");
        }
        catch (Exception exception)
        {
            LogContext.Error?.Log(exception, "Failure removing auto-delete queues/topics");
        }
    }

    Task Delete(SessionContext context, Topic topic)
    {
        return context.DeleteTopic(topic.EntityName);
    }

    Task Delete(SessionContext context, Queue queue)
    {
        return context.DeleteQueue(queue.EntityName);
    }
}
