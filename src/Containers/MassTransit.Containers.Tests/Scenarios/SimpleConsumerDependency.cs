namespace MassTransit.Containers.Tests.Scenarios
{
    using System;
    using System.Threading.Tasks;
    using Util;


    public class SimpleConsumerDependency :
        ISimpleConsumerDependency,
        IDisposable
    {
        readonly TaskCompletionSource<bool> _disposed;

        public SimpleConsumerDependency()
        {
            _disposed = TaskUtil.GetTask<bool>();
        }

        public void Dispose()
        {
            Console.WriteLine("Disposing Simple Dependency");

            _disposed.TrySetResult(true);
        }

        public Task<bool> WasDisposed
        {
            get { return _disposed.Task; }
        }

        public void DoSomething()
        {
            if (_disposed.Task.IsCompleted)
                throw new ObjectDisposedException("Should not have disposed of me just yet");

            SomethingDone = true;
        }

        public bool SomethingDone { get; private set; }
    }
}
