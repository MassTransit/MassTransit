namespace MassTransit
{
    using Context;


    public class PublishMessageEvent<T> :
        MessageEvent<T>
        where T : class
    {
        public PublishMessageEvent(PublishContext<T> context)
            : base(new MessageSendContextAdapter(context), context.Message)
        {
        }
    }
}