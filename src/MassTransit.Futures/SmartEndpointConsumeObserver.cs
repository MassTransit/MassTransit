namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using Util;


    public class SmartEndpointConsumeObserver :
        IConsumeObserver
    {
        readonly IMessageBuffer _messageBuffer;

        public SmartEndpointConsumeObserver(IMessageBuffer messageBuffer)
        {
            _messageBuffer = messageBuffer;
        }

        public Task PreConsume<T>(ConsumeContext<T> context)
            where T : class
        {
            return TaskUtil.Completed;
        }

        public Task PostConsume<T>(ConsumeContext<T> context)
            where T : class
        {
            return _messageBuffer.Add(new ConsumeMessageEvent<T>(context));
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
            where T : class
        {
            return _messageBuffer.Add(new ConsumeFaultMessageEvent<T>(context, exception));
        }
    }
}