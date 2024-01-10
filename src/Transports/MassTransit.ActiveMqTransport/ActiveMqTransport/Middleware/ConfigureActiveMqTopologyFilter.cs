namespace MassTransit.ActiveMqTransport.Middleware;

using System;
using System.Linq;
using System.Threading.Tasks;
using Apache.NMS.ActiveMQ;
using Topology;


/// <summary>
/// Configures the broker with the supplied topology once the model is created, to ensure
/// that the exchanges, queues, and bindings for the model are properly configured in ActiveMQ.
/// </summary>
public class ConfigureActiveMqTopologyFilter<TSettings> :
    IFilter<SessionContext>
    where TSettings : class
{
    readonly BrokerTopology _brokerTopology;
    readonly TSettings _settings;

    public ConfigureActiveMqTopologyFilter(TSettings settings, BrokerTopology brokerTopology)
    {
        _settings = settings;
        _brokerTopology = brokerTopology;
    }

    public async Task Send(SessionContext context, IPipe<SessionContext> next)
    {
        OneTimeContext<ConfigureTopologyContext<TSettings>> oneTimeContext = await Configure(context);

        try
        {
            await next.Send(context).ConfigureAwait(false);
        }
        catch (Exception)
        {
            oneTimeContext.Evict();

            throw;
        }

        if (_settings is ReceiveSettings)
            await DeleteAutoDelete(context).ConfigureAwait(false);
    }

    public void Probe(ProbeContext context)
    {
        var scope = context.CreateFilterScope("configureTopology");

        _brokerTopology.Probe(scope);
    }

    public async Task<OneTimeContext<ConfigureTopologyContext<TSettings>>> Configure(SessionContext context)
    {
        return await context.OneTimeSetup<ConfigureTopologyContext<TSettings>>(() =>
        {
            context.GetOrAddPayload(() => _settings);

            return ConfigureTopology(context);
        }).ConfigureAwait(false);
    }

    async Task ConfigureTopology(SessionContext context)
    {
        await Task.WhenAll(_brokerTopology.Topics.Select(topic => Declare(context, topic))).ConfigureAwait(false);

        await Task.WhenAll(_brokerTopology.Queues.Select(queue => Declare(context, queue))).ConfigureAwait(false);
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

    Task Declare(SessionContext context, Topic topic)
    {
        LogContext.Debug?.Log("Get topic {Topic}", topic);

        return context.GetTopic(topic);
    }

    Task Declare(SessionContext context, Queue queue)
    {
        LogContext.Debug?.Log("Get queue {Queue}", queue);

        return context.GetQueue(queue);
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
