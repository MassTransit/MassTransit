namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using RabbitMqTransport;
    using Util;


    class TestConnectionPipe :
        IPipe<ConnectionContext>
    {
        readonly TaskCompletionSource<ConnectionContext> _called;

        public TestConnectionPipe(CancellationToken testCancellationToken)
        {
            _called = TaskUtil.GetTask<ConnectionContext>();
            testCancellationToken.Register(() => _called.TrySetCanceled());
        }


        public Task<ConnectionContext> Called
        {
            get { return _called.Task; }
        }

        async void IProbeSite.Probe(ProbeContext context)
        {
        }

        public async Task Send(ConnectionContext context)
        {
            _called.TrySetResult(context);
        }
    }
}
