namespace MassTransit.ActiveMqTransport
{
    using System;
    using System.Threading.Tasks;
    using Apache.NMS;


    public class ActiveMqMoveTransport
    {
        readonly string _destination;
        readonly IFilter<SessionContext> _topologyFilter;

        protected ActiveMqMoveTransport(string destination, IFilter<SessionContext> topologyFilter)
        {
            _topologyFilter = topologyFilter;
            _destination = destination;
        }

        protected async Task Move(ReceiveContext context, Action<IMessage, SendHeaders> preSend)
        {
            if (!context.TryGetPayload(out SessionContext sessionContext))
                throw new ArgumentException("The ReceiveContext must contain a SessionContext", nameof(context));

            await _topologyFilter.Send(sessionContext, Pipe.Empty<SessionContext>()).ConfigureAwait(false);

            var queue = await sessionContext.GetQueue(_destination).ConfigureAwait(false);

            var producer = await sessionContext.CreateMessageProducer(queue).ConfigureAwait(false);
            var body = context.GetBody();

            var message = producer.CreateBytesMessage();

            if (context.TryGetPayload(out ActiveMqMessageContext messageContext))
            {
                foreach (string key in messageContext.Properties.Keys)
                    message.Properties[key] = messageContext.Properties[key];
            }

            SendHeaders headers = new PrimitiveMapHeaders(message.Properties);

            headers.SetHostHeaders();

            preSend(message, headers);

            message.Content = body;

            var task = Task.Run(() => producer.Send(message));
            context.AddReceiveTask(task);
        }
    }
}
