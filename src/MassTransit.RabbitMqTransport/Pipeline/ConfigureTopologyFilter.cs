namespace MassTransit.RabbitMqTransport.Pipeline
{
    using System.Linq;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Topology.Builders;
    using Topology.Entities;


    /// <summary>
    /// Configures the broker with the supplied topology once the model is created, to ensure
    /// that the exchanges, queues, and bindings for the model are properly configured in RabbitMQ.
    /// </summary>
    public class ConfigureTopologyFilter<TSettings> :
        IFilter<ModelContext>
        where TSettings : class
    {
        readonly TSettings _settings;
        readonly BrokerTopology _brokerTopology;

        public ConfigureTopologyFilter(TSettings settings, BrokerTopology brokerTopology)
        {
            _settings = settings;
            _brokerTopology = brokerTopology;
        }

        async Task IFilter<ModelContext>.Send(ModelContext context, IPipe<ModelContext> next)
        {
            await context.OneTimeSetup<ConfigureTopologyContext<TSettings>>(async payload =>
            {
                await ConfigureTopology(context).ConfigureAwait(false);

                context.GetOrAddPayload(() => _settings);
            }, () => new Context()).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("configureTopology");

            _brokerTopology.Probe(scope);
        }

        async Task ConfigureTopology(ModelContext context)
        {
            await Task.WhenAll(_brokerTopology.Exchanges.Select(exchange => Declare(context, exchange))).ConfigureAwait(false);

            await Task.WhenAll(_brokerTopology.ExchangeBindings.Select(binding => Bind(context, binding))).ConfigureAwait(false);

            await Task.WhenAll(_brokerTopology.Queues.Select(queue => Declare(context, queue))).ConfigureAwait(false);

            await Task.WhenAll(_brokerTopology.QueueBindings.Select(binding => Bind(context, binding))).ConfigureAwait(false);
        }

        Task Declare(ModelContext context, Exchange exchange)
        {
            LogContext.Debug?.Log("Declare exchange {Exchange}", exchange);

            return context.ExchangeDeclare(exchange.ExchangeName, exchange.ExchangeType, exchange.Durable, exchange.AutoDelete, exchange.ExchangeArguments);
        }

        Task Declare(ModelContext context, Queue queue)
        {
            LogContext.Debug?.Log("Declare queue {Queue}", queue);

            return context.QueueDeclare(queue.QueueName, queue.Durable, queue.Exclusive, queue.AutoDelete, queue.QueueArguments);
        }

        Task Bind(ModelContext context, ExchangeToExchangeBinding binding)
        {
            LogContext.Debug?.Log("Bind exchange to exchange {Binding}", binding);

            return context.ExchangeBind(binding.Destination.ExchangeName, binding.Source.ExchangeName, binding.RoutingKey, binding.Arguments);
        }

        Task Bind(ModelContext context, ExchangeToQueueBinding binding)
        {
            LogContext.Debug?.Log("Bind exchange to queue {Binding}", binding);

            return context.QueueBind(binding.Destination.QueueName, binding.Source.ExchangeName, binding.RoutingKey, binding.Arguments);
        }


        class Context :
            ConfigureTopologyContext<TSettings>
        {
        }
    }
}
