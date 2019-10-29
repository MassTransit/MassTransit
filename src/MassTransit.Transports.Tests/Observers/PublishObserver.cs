namespace MassTransit.Transports.Tests.Observers
{
    using System;
    using System.Threading.Tasks;
    using Util;


    public class PublishObserver :
        IPublishObserver
    {
        readonly TaskCompletionSource<PublishContext> _postSend;
        readonly TaskCompletionSource<PublishContext> _preSend;
        readonly TaskCompletionSource<PublishContext> _sendFaulted;

        public PublishObserver()
        {
            _sendFaulted = TaskUtil.GetTask<PublishContext>();
            _preSend = TaskUtil.GetTask<PublishContext>();
            _postSend = TaskUtil.GetTask<PublishContext>();
        }

        public Task<PublishContext> PrePublished
        {
            get { return _preSend.Task; }
        }

        public Task<PublishContext> PostPublished
        {
            get { return _postSend.Task; }
        }

        public Task<PublishContext> PublishFaulted
        {
            get { return _sendFaulted.Task; }
        }

        public async Task PrePublish<T>(PublishContext<T> context)
            where T : class
        {
            _preSend.TrySetResult(context);
        }

        public async Task PostPublish<T>(PublishContext<T> context)
            where T : class
        {
            _postSend.TrySetResult(context);
        }

        public async Task PublishFault<T>(PublishContext<T> context, Exception exception)
            where T : class
        {
            _sendFaulted.TrySetResult(context);
        }
    }
}
