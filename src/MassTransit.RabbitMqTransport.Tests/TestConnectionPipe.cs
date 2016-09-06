namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Pipeline;
    using Monitoring.Introspection;
    using RabbitMqTransport;


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

        async void IProbeSite.Probe(ProbeContext context)
        {
        }

        public async Task Send(ConnectionContext context)
        {
            _called.TrySetResult(context);
        }
    }
}