namespace MassTransit.ActiveMqTransport.Middleware;

using System;
using System.Linq;
using System.Threading.Tasks;
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
    readonly ActiveMqReceiveEndpointContext _context;
    readonly TSettings _settings;

    public ConfigureActiveMqTopologyFilter(TSettings settings, BrokerTopology brokerTopology, ActiveMqReceiveEndpointContext context)
    {
        _settings = settings;
        _brokerTopology = brokerTopology;
        _context = context;
    }

    public async Task Send(SessionContext context, IPipe<SessionContext> next)
    {
        OneTimeContext<ConfigureTopologyContext<TSettings>> oneTimeContext = await Configure(context);

        try
        {
            await next.Send(context).ConfigureAwait(false);

            if (_settings is ReceiveSettings)
                _context.AddSendAgent(new RemoveAutoDeleteAgent(context, _brokerTopology));
        }
        catch (Exception)
        {
            oneTimeContext.Evict();

            throw;
        }
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
}
