namespace MassTransit.RabbitMqTransport.Contexts
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Scheduling;
    using Scheduling;
    using Topology;


    /// <summary>
    /// Context for delaying message redelivery using a delayed RabbitMQ exchange. Requires the plug-in
    /// https://github.com/rabbitmq/rabbitmq-delayed-message-exchange
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class DelayedExchangeMessageRedeliveryContext<TMessage> :
        MessageRedeliveryContext
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _context;

        public DelayedExchangeMessageRedeliveryContext(ConsumeContext<TMessage> context)
        {
            _context = context;
        }

        Task MessageRedeliveryContext.ScheduleRedelivery(TimeSpan delay, Action<ConsumeContext, SendContext> callback)
        {
            Action<ConsumeContext, SendContext> combinedCallback = UpdateDeliveryContext + callback;

            var scheduler = CreateMessageScheduler();

            return scheduler.ScheduleSend(_context.ReceiveContext.InputAddress, delay, _context.Message, _context.CreateCopyContextPipe(combinedCallback));
        }

        static void UpdateDeliveryContext(ConsumeContext context, SendContext sendContext)
        {
            sendContext.Headers.Set(MessageHeaders.RedeliveryCount, context.GetRedeliveryCount() + 1);
        }

        IMessageScheduler CreateMessageScheduler()
        {
            if (_context.TryGetPayload<IMessageScheduler>(out var scheduler))
                return scheduler;

            var hostTopology = _context.GetPayload<IRabbitMqHostTopology>();

            var provider = new DelayedExchangeScheduleMessageProvider(_context, hostTopology);

            return new MessageScheduler(provider, hostTopology);
        }
    }
}
