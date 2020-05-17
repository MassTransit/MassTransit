namespace MassTransit.Testing.Observers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes.Internals.Extensions;
    using Util;


    public class AsyncInactivityObserver :
        IInactivityObserver
    {
        readonly TaskCompletionSource<bool> _inactivityTaskSource;
        readonly Lazy<Task> _inactivityTask;
        readonly CancellationTokenSource _inactivityTokenSource;

        public AsyncInactivityObserver(TimeSpan timeout, CancellationToken cancellationToken)
        {
            _inactivityTaskSource = TaskUtil.GetTask();
            _inactivityTask = new Lazy<Task>(() => _inactivityTaskSource.Task.OrTimeout(timeout, cancellationToken));

            _inactivityTokenSource = new CancellationTokenSource();
        }

        public Task InactivityTask => _inactivityTask.Value;

        public CancellationToken InactivityToken => _inactivityTokenSource.Token;

        public Task NoActivity()
        {
            _inactivityTaskSource.TrySetResult(true);
            _inactivityTokenSource.Cancel();

            return TaskUtil.Completed;
        }
    }
}
