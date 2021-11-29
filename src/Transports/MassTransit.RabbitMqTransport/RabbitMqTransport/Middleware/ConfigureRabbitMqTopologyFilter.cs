namespace MassTransit.RabbitMqTransport.Middleware
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Topology;


    /// <summary>
    /// Configures the broker with the supplied topology once the model is created, to ensure
    /// that the exchanges, queues, and bindings for the model are properly configured in RabbitMQ.
    /// </summary>
    public class ConfigureRabbitMqTopologyFilter<TSettings> :
        IFilter<ModelContext>
        where TSettings : class
    {
        readonly BrokerTopology _brokerTopology;
        readonly TSettings _settings;

        public ConfigureRabbitMqTopologyFilter(TSettings settings, BrokerTopology brokerTopology)
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
            await Task.WhenAll(_brokerTopology.Queues.Select(queue => Declare(context, (Queue)queue))).ConfigureAwait(false);

            await Task.WhenAll(_brokerTopology.Exchanges.Select(exchange => Declare(context, exchange))).ConfigureAwait(false);

            await Task.WhenAll(_brokerTopology.QueueBindings.Select(binding => Bind(context, binding))).ConfigureAwait(false);

            await Task.WhenAll(_brokerTopology.ExchangeBindings.Select(binding => Bind(context, binding))).ConfigureAwait(false);
        }

        static Task Declare(ModelContext context, Exchange exchange)
        {
            RabbitMqLogMessages.DeclareExchange(exchange);

            return context.ExchangeDeclare(exchange.ExchangeName, exchange.ExchangeType, exchange.Durable, exchange.AutoDelete, exchange.ExchangeArguments);
        }

        static async Task Declare(ModelContext context, Queue queue)
        {
            try
            {
                var ok = await context.QueueDeclare(queue.QueueName, queue.Durable, queue.Exclusive, queue.AutoDelete, queue.QueueArguments)
                    .ConfigureAwait(false);

                RabbitMqLogMessages.DeclareQueue(queue, ok.ConsumerCount, ok.MessageCount);

                await Task.Delay(10).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                LogContext.Error?.Log(exception, "Declare queue faulted: {Queue}", queue);

                throw;
            }
        }

        static async Task Bind(ModelContext context, ExchangeToExchangeBinding binding)
        {
            RabbitMqLogMessages.BindToExchange(binding);

            await context.ExchangeBind(binding.Destination.ExchangeName, binding.Source.ExchangeName, binding.RoutingKey, binding.Arguments)
                .ConfigureAwait(false);

            await Task.Delay(10).ConfigureAwait(false);
        }

        static async Task Bind(ModelContext context, ExchangeToQueueBinding binding)
        {
            RabbitMqLogMessages.BindToQueue(binding);

            await context.QueueBind(binding.Destination.QueueName, binding.Source.ExchangeName, binding.RoutingKey, binding.Arguments).ConfigureAwait(false);

            await Task.Delay(10).ConfigureAwait(false);
        }


        class Context :
            ConfigureTopologyContext<TSettings>
        {
        }
    }
}
