namespace MassTransit.Transports.RabbitMq.Tests
{
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Pipeline;


    class TestConnectionPipe :
        IPipe<ConnectionContext>
    {
        readonly TaskCompletionSource<ConnectionContext> _called;

        public TestConnectionPipe(CancellationToken testCancellationToken)
        {
            _called = new TaskCompletionSource<ConnectionContext>();
            testCancellationToken.Register(() => _called.TrySetCanceled());
        }


        public Task<ConnectionContext> Called
        {
            get { return _called.Task; }
        }

        public async Task Send(ConnectionContext context)
        {
            _called.TrySetResult(context);
        }

        public bool Inspect(IPipeInspector inspector)
        {
            return inspector.Inspect(this);
        }
    }
}