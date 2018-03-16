namespace MassTransit
{
    using System;
    using Context;


    public class SendFaultMessageEvent<T> :
        MessageEvent<T>
        where T : class
    {
        readonly Exception _exception;

        public SendFaultMessageEvent(SendContext<T> context, Exception exception)
            : base(new MessageSendContextAdapter(context), context.Message)
        {
            _exception = exception;
        }
    }
}