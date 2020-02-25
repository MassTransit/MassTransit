namespace MassTransit.Transports.Tests.Observers
{
    using System;
    using System.Threading.Tasks;
    using Testing;


    public class PublishObserver :
        IPublishObserver
    {
        readonly TaskCompletionSource<PublishContext> _postSend;
        readonly TaskCompletionSource<PublishContext> _preSend;
        readonly TaskCompletionSource<PublishContext> _sendFaulted;

        public PublishObserver(AsyncTestHarness fixture)
        {
            _postSend = fixture.GetTask<PublishContext>();
            _preSend = fixture.GetTask<PublishContext>();
            _sendFaulted = fixture.GetTask<PublishContext>();
        }

        public Task<PublishContext> PrePublished => _preSend.Task;
        public Task<PublishContext> PostPublished => _postSend.Task;
        public Task<PublishContext> PublishFaulted => _sendFaulted.Task;

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
