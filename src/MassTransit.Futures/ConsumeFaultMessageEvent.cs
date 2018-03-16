namespace MassTransit
{
    using System;


    public class ConsumeFaultMessageEvent<T> :
        MessageEvent<T>
        where T : class
    {
        readonly Exception _exception;

        public ConsumeFaultMessageEvent(ConsumeContext<T> context, Exception exception)
            : base(context, context.Message)
        {
            _exception = exception;
        }
    }
}