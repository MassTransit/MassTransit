namespace MassTransit.Tests.ContainerTests.Scenarios
{
    using System;
    using System.Threading.Tasks;
    using Util;


    public class SimpleConsumerDependency :
        ISimpleConsumerDependency,
        IDisposable
    {
        readonly ConsumeContext _consumeContext;
        readonly TaskCompletionSource<bool> _disposed;
        readonly ISendEndpointProvider _sendEndpointProvider;

        public SimpleConsumerDependency(ISendEndpointProvider sendEndpointProvider, ConsumeContext consumeContext)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _consumeContext = consumeContext;
            _disposed = TaskUtil.GetTask<bool>();
        }

        public void Dispose()
        {
            Console.WriteLine("Disposing Simple Dependency");

            _disposed.TrySetResult(true);
        }

        public Task<bool> WasDisposed => _disposed.Task;

        public void DoSomething()
        {
            if (_disposed.Task.IsCompleted)
                throw new ObjectDisposedException("Should not have disposed of me just yet");

            if (_sendEndpointProvider != _consumeContext)
                throw new InvalidOperationException("The injected types should be the same");

            SomethingDone = true;
        }

        public bool SomethingDone { get; private set; }
    }
}
