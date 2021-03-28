namespace MassTransit.Transports.Scheduling
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;


    public class DelayedMessageRedeliveryContext<TMessage> :
        MessageRedeliveryContext
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _context;

        public DelayedMessageRedeliveryContext(ConsumeContext<TMessage> context)
        {
            _context = context;
        }

        async Task MessageRedeliveryContext.ScheduleRedelivery(TimeSpan delay, Action<ConsumeContext, SendContext> callback)
        {
            IPipe<SendContext<TMessage>> pipe = Pipe.Execute<SendContext<TMessage>>(sendContext =>
            {
                sendContext.Headers.Set(MessageHeaders.RedeliveryCount, _context.GetRedeliveryCount() + 1);

                callback?.Invoke(_context, sendContext);
            });

            IPipe<SendContext<TMessage>> delaySendPipe = new DelaySendPipe<TMessage>(pipe, delay);

            var schedulerEndpoint = await _context.GetSendEndpoint(_context.ReceiveContext.InputAddress).ConfigureAwait(false);

            await _context.Forward(schedulerEndpoint, delaySendPipe).ConfigureAwait(false);
        }
    }
}
