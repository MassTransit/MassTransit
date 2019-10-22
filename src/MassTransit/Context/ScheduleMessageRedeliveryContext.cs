namespace MassTransit.Context
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// Used to schedule message redelivery using the message scheduler
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class ScheduleMessageRedeliveryContext<TMessage> :
        MessageRedeliveryContext
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _context;

        public ScheduleMessageRedeliveryContext(ConsumeContext<TMessage> context)
        {
            _context = context;
        }

        Task MessageRedeliveryContext.ScheduleRedelivery(TimeSpan delay, Action<ConsumeContext, SendContext> callback)
        {
            var schedulerContext = _context.GetPayload<MessageSchedulerContext>();

            Action<ConsumeContext, SendContext> combinedAction = AddMessageHeaderAction + callback;
            return schedulerContext.ScheduleSend(delay, _context.Message, _context.CreateCopyContextPipe(combinedAction));
        }

        void AddMessageHeaderAction(ConsumeContext consumeContext, SendContext sendContext)
        {
            foreach (KeyValuePair<string, object> additionalHeader in GetScheduledMessageHeaders(consumeContext))
                sendContext.Headers.Set(additionalHeader.Key, additionalHeader.Value);
        }

        static IEnumerable<KeyValuePair<string, object>> GetScheduledMessageHeaders(ConsumeContext context)
        {
            Uri inputAddress = context.ReceiveContext.InputAddress ?? context.DestinationAddress;
            if (inputAddress != null)
                yield return new KeyValuePair<string, object>(MessageHeaders.DeliveredAddress, inputAddress.ToString());

            yield return new KeyValuePair<string, object>(MessageHeaders.RedeliveryCount, context.GetRedeliveryCount() + 1);
        }
    }
}
