#nullable enable
namespace MassTransit.Context
{
    using System;
    using System.Threading.Tasks;
    using Middleware;


    /// <summary>
    /// Used to schedule message redelivery using the message scheduler
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class ScheduleMessageRedeliveryContext<TMessage> :
        MessageRedeliveryContext
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _context;
        readonly RedeliveryOptions _options;

        public ScheduleMessageRedeliveryContext(ConsumeContext<TMessage> context, RedeliveryOptions options)
        {
            _context = context;
            _options = options;
        }

        public Task ScheduleRedelivery(TimeSpan delay, Action<ConsumeContext, SendContext>? callback)
        {
            var schedulerContext = _context.GetPayload<MessageSchedulerContext>();

            void SendCallback(ConsumeContext consumeContext, SendContext sendContext)
            {
                sendContext.ApplyRedeliveryOptions(consumeContext, _options);

                callback?.Invoke(consumeContext, sendContext);
            }

            return schedulerContext.ScheduleSend(delay, _context.Message, new CopyContextPipe(_context, SendCallback));
        }
    }
}
