namespace MassTransit
{
    using Context;


    public class SendMessageEvent<T> :
        MessageEvent<T>
        where T : class
    {
        public SendMessageEvent(SendContext<T> context)
            : base(new MessageSendContextAdapter(context), context.Message)
        {
        }
    }
}