namespace MassTransit.Transports.Tests.Observers
{
    using System;
    using System.Threading.Tasks;
    using Testing;


    public class SendObserver :
        ISendObserver
    {
        readonly TaskCompletionSource<SendContext> _postSend;
        readonly TaskCompletionSource<SendContext> _preSend;
        readonly TaskCompletionSource<SendContext> _sendFaulted;

        public SendObserver(AsyncTestHarness fixture)
        {
            _postSend = fixture.GetTask<SendContext>();
            _preSend = fixture.GetTask<SendContext>();
            _sendFaulted = fixture.GetTask<SendContext>();
        }

        public Task<SendContext> PreSent => _preSend.Task;
        public Task<SendContext> PostSent => _postSend.Task;
        public Task<SendContext> SendFaulted => _sendFaulted.Task;

        public async Task PreSend<T>(SendContext<T> context)
            where T : class
        {
            _preSend.TrySetResult(context);
        }

        public async Task PostSend<T>(SendContext<T> context)
            where T : class
        {
            _postSend.TrySetResult(context);
        }

        public async Task SendFault<T>(SendContext<T> context, Exception exception)
            where T : class
        {
            _sendFaulted.TrySetResult(context);
        }
    }
}
