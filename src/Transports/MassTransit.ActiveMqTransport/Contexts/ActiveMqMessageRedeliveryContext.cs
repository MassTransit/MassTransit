namespace MassTransit.ActiveMqTransport.Contexts
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Scheduling;
    using MassTransit.Topology;
    using Scheduling;


    /// <summary>
    /// Context for delaying message redelivery using a delayed ActiveMQ messages.
    /// http://activemq.apache.org/delay-and-schedule-message-delivery
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class ActiveMqMessageRedeliveryContext<TMessage> :
        MessageRedeliveryContext
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _context;

        public ActiveMqMessageRedeliveryContext(ConsumeContext<TMessage> context)
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

            var provider = new ActiveMqScheduleMessageProvider(_context);

            return new MessageScheduler(provider, _context.GetPayload<IBusTopology>());
        }
    }
}
