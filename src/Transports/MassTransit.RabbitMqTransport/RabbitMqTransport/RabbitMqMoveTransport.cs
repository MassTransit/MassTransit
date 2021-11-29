namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using RabbitMQ.Client;


    public class RabbitMqMoveTransport
    {
        readonly string _exchange;
        readonly IFilter<ModelContext> _topologyFilter;

        protected RabbitMqMoveTransport(string exchange, IFilter<ModelContext> topologyFilter)
        {
            _topologyFilter = topologyFilter;
            _exchange = exchange;
        }

        protected async Task Move(ReceiveContext context, Action<IBasicProperties, SendHeaders> preSend)
        {
            if (!context.TryGetPayload(out ModelContext modelContext))
                throw new ArgumentException("The ReceiveContext must contain a ModelContext", nameof(context));

            await _topologyFilter.Send(modelContext, Pipe.Empty<ModelContext>()).ConfigureAwait(false);

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

            var task = modelContext.BasicPublishAsync(_exchange, routingKey, true, properties, body, true);
            context.AddReceiveTask(task);
        }
    }
}
