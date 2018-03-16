namespace MassTransit
{
    public class ConsumeMessageEvent<T> :
        MessageEvent<T>
        where T : class
    {
        public ConsumeMessageEvent(ConsumeContext<T> context)
            : base(context, context.Message)
        {
        }
    }
}