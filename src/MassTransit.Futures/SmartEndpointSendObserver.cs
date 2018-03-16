namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using Util;


    public class SmartEndpointSendObserver :
        ISendObserver
    {
        readonly IMessageBuffer _messageBuffer;

        public SmartEndpointSendObserver(IMessageBuffer messageBuffer)
        {
            _messageBuffer = messageBuffer;
        }

        public Task PreSend<T>(SendContext<T> context)
            where T : class
        {
            return TaskUtil.Completed;
        }

        public Task PostSend<T>(SendContext<T> context)
            where T : class
        {
            return _messageBuffer.Add(new SendMessageEvent<T>(context));
        }

        public Task SendFault<T>(SendContext<T> context, Exception exception)
            where T : class
        {
            return _messageBuffer.Add(new SendFaultMessageEvent<T>(context, exception));
        }
    }
}