namespace MassTransit.ActiveMqTransport
{
    using System;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Middleware;
    using Topology;


    public class ActiveMqMoveTransport<TSettings>
        where TSettings : class
    {
        readonly Queue _destination;
        readonly ConfigureActiveMqTopologyFilter<TSettings> _topologyFilter;

        protected ActiveMqMoveTransport(Queue destination, ConfigureActiveMqTopologyFilter<TSettings> topologyFilter)
        {
            _topologyFilter = topologyFilter;
            _destination = destination;
        }

        protected async Task Move(ReceiveContext context, Action<IMessage, SendHeaders> preSend)
        {
            if (!context.TryGetPayload(out SessionContext sessionContext))
                throw new ArgumentException("The ReceiveContext must contain a SessionContext", nameof(context));

            if (!context.TryGetPayload(out ActiveMqMessageContext messageContext))
                throw new ArgumentException("The ActiveMqMessageContext was not present", nameof(context));

            OneTimeContext<ConfigureTopologyContext<TSettings>> oneTimeContext = await _topologyFilter.Configure(sessionContext).ConfigureAwait(false);

            var queue = await sessionContext.GetQueue(_destination).ConfigureAwait(false);

            var message = messageContext.TransportMessage switch
            {
                // ReSharper disable MethodHasAsyncOverload
                IBytesMessage _ => sessionContext.CreateBytesMessage(context.Body.GetBytes()),
                ITextMessage _ => sessionContext.CreateTextMessage(context.Body.GetString()),
                _ => sessionContext.CreateMessage(),
            };

            CloneMessage(message, messageContext.TransportMessage, preSend);

            try
            {
                await sessionContext.SendAsync(queue, message, context.CancellationToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
                oneTimeContext.Evict();
                throw;
            }
        }

        static void CloneMessage(IMessage message, IMessage source, Action<IMessage, SendHeaders> preSend)
        {
            message.NMSReplyTo = source.NMSReplyTo;
            message.NMSDeliveryMode = source.NMSDeliveryMode;
            message.NMSCorrelationID = source.NMSCorrelationID;
            message.NMSPriority = source.NMSPriority;

            foreach (string key in source.Properties.Keys)
                message.Properties[key] = source.Properties[key];

            SendHeaders headers = new PrimitiveMapHeaders(message.Properties);

            headers.SetHostHeaders();

            preSend(message, headers);
        }
    }
}
