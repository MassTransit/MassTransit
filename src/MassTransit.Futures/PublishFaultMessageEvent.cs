namespace MassTransit
{
    using System;
    using Context;


    public class PublishFaultMessageEvent<T> :
        MessageEvent<T>
        where T : class
    {
        readonly Exception _exception;

        public PublishFaultMessageEvent(PublishContext<T> context, Exception exception)
            : base(new MessageSendContextAdapter(context), context.Message)
        {
            _exception = exception;
        }
    }
}