namespace MassTransit.ActiveMqTransport.Contexts
{
    using System;
    using System.Threading.Tasks;


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
        readonly IMessageScheduler _scheduler;

        public ActiveMqMessageRedeliveryContext(ConsumeContext<TMessage> context, IMessageScheduler scheduler)
        {
            _context = context;
            _scheduler = scheduler;
        }

        Task MessageRedeliveryContext.ScheduleRedelivery(TimeSpan delay, Action<ConsumeContext, SendContext> callback)
        {
            var combinedCallback = UpdateDeliveryContext + callback;

            return _scheduler.ScheduleSend(_context.ReceiveContext.InputAddress, delay, _context.Message, _context.CreateCopyContextPipe(combinedCallback));
        }

        static void UpdateDeliveryContext(ConsumeContext context, SendContext sendContext)
        {
            sendContext.Headers.Set(MessageHeaders.RedeliveryCount, context.GetRedeliveryCount() + 1);
        }
    }
}
