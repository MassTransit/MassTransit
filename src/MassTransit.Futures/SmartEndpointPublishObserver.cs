namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using Util;


    public class SmartEndpointPublishObserver :
        IPublishObserver
    {
        readonly IMessageBuffer _messageBuffer;

        public SmartEndpointPublishObserver(IMessageBuffer messageBuffer)
        {
            _messageBuffer = messageBuffer;
        }

        public Task PrePublish<T>(PublishContext<T> context)
            where T : class
        {
            return TaskUtil.Completed;
        }

        public Task PostPublish<T>(PublishContext<T> context)
            where T : class
        {
            return _messageBuffer.Add(new PublishMessageEvent<T>(context));
        }

        public Task PublishFault<T>(PublishContext<T> context, Exception exception)
            where T : class
        {
            return _messageBuffer.Add(new PublishFaultMessageEvent<T>(context, exception));
        }
    }
}