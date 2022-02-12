#nullable enable
namespace MassTransit.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Serialization;


    public class DelayedMessageRedeliveryContext<TMessage> :
        MessageRedeliveryContext
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _context;
        readonly RedeliveryOptions _options;

        public DelayedMessageRedeliveryContext(ConsumeContext<TMessage> context, RedeliveryOptions options)
        {
            _context = context;
            _options = options;
        }

        public async Task ScheduleRedelivery(TimeSpan delay, Action<ConsumeContext, SendContext>? callback)
        {
            IPipe<SendContext<TMessage>> pipe = Pipe.Execute<SendContext<TMessage>>(sendContext =>
            {
                sendContext.ApplyRedeliveryOptions(_context, _options);

                callback?.Invoke(_context, sendContext);
            });

            IPipe<SendContext<TMessage>> delaySendPipe = new DelaySendPipe<TMessage>(pipe, delay);

            var endpoint = await _context.GetSendEndpoint(_context.ReceiveContext.InputAddress).ConfigureAwait(false);

            var messagePipe = new ForwardMessagePipe<TMessage>(_context, delaySendPipe);

            await endpoint.Send(_context.Message, messagePipe, _context.CancellationToken).ConfigureAwait(false);
        }
    }
}
