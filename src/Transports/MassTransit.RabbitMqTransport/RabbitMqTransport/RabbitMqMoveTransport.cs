namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Middleware;
    using RabbitMQ.Client;


    public class RabbitMqMoveTransport<TSettings>
        where TSettings : class
    {
        readonly string _exchange;
        readonly ConfigureRabbitMqTopologyFilter<TSettings> _topologyFilter;

        protected RabbitMqMoveTransport(string exchange, ConfigureRabbitMqTopologyFilter<TSettings> topologyFilter)
        {
            _topologyFilter = topologyFilter;
            _exchange = exchange;
        }

        protected async Task Move(ReceiveContext context, Action<IBasicProperties, SendHeaders> preSend)
        {
            if (!context.TryGetPayload(out ModelContext modelContext))
                throw new ArgumentException("The ReceiveContext must contain a ModelContext", nameof(context));

            OneTimeContext<ConfigureTopologyContext<TSettings>> oneTimeContext = await _topologyFilter.Configure(modelContext).ConfigureAwait(false);

            IBasicProperties properties;
            var routingKey = "";
            byte[] body;

            if (context.TryGetPayload(out RabbitMqBasicConsumeContext basicConsumeContext))
            {
                properties = basicConsumeContext.Properties;
                routingKey = basicConsumeContext.RoutingKey;
                body = context.GetBody();
            }
            else
            {
                properties = modelContext.Model.CreateBasicProperties();
                properties.Headers = new Dictionary<string, object>();

                body = context.GetBody();
            }

            SendHeaders headers = new MoveTransportHeaders(properties);

            headers.SetHostHeaders();

            preSend(properties, headers);

            try
            {
                await modelContext.BasicPublishAsync(_exchange, routingKey, true, properties, body, true).ConfigureAwait(false);
            }
            catch (Exception)
            {
                oneTimeContext.Evict();
                throw;
            }
        }
    }
}
