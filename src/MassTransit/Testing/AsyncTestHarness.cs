namespace MassTransit.Testing
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Implementations;
    using Util;


    public abstract class AsyncTestHarness :
        IDisposable
    {
        readonly Lazy<AsyncInactivityObserver> _inactivityObserver;
        CancellationToken _cancellationToken;
        CancellationTokenSource _cancellationTokenSource;
        Task<bool> _cancelledTask;

        protected AsyncTestHarness()
        {
            TestTimeout = Debugger.IsAttached ? TimeSpan.FromMinutes(50) : TimeSpan.FromSeconds(30);
            TestInactivityTimeout = TimeSpan.FromSeconds(6);

            _inactivityObserver = new Lazy<AsyncInactivityObserver>(() => new AsyncInactivityObserver(TestInactivityTimeout, TestCancellationToken));
        }

        /// <summary>
        /// Task that is canceled when the test is aborted, for continueWith usage
        /// </summary>
        public Task TestCancelledTask
        {
            get
            {
                var token = TestCancellationToken;
                return _cancelledTask;
            }
        }

        /// <summary>
        /// CancellationToken that is canceled when the test is being aborted
        /// </summary>
        public CancellationToken TestCancellationToken
        {
            get
            {
                if (_cancellationToken == CancellationToken.None)
                {
                    _cancellationTokenSource = new CancellationTokenSource((int)TestTimeout.TotalMilliseconds);
                    _cancellationToken = _cancellationTokenSource.Token;

                    TaskCompletionSource<bool> source = TaskUtil.GetTask<bool>();
                    _cancelledTask = source.Task;

                    _cancellationToken.Register(() => source.TrySetCanceled());
                }

                return _cancellationToken;
            }
        }

        /// <summary>
        /// Task that is completed when the bus inactivity timeout has elapsed with no bus activity
        /// </summary>
        public Task InactivityTask => _inactivityObserver.Value.InactivityTask;

        /// <summary>
        /// CancellationToken that is cancelled when the test inactivity timeout has elapsed with no bus activity
        /// </summary>
        public CancellationToken InactivityToken => _inactivityObserver.Value.InactivityToken;

        public IInactivityObserver InactivityObserver => _inactivityObserver.Value;

        /// <summary>
        /// Timeout for the test, used for any delay timers
        /// </summary>
        public TimeSpan TestTimeout { get; set; }

        /// <summary>
        /// Timeout specifying the elapsed time with no bus activity after which the test could be completed
        /// </summary>
        public TimeSpan TestInactivityTimeout { get; set; }

        public virtual void Dispose()
        {
            _cancellationTokenSource?.Dispose();
        }

        /// <summary>
        /// Forces the test to be cancelled, aborting any awaiting tasks
        /// </summary>
        public void Cancel()
        {
            _cancellationTokenSource?.Cancel();
        }

        public void ForceInactive()
        {
            _inactivityObserver.Value.ForceInactive();
        }

        /// <summary>
        /// Returns a task completion that is automatically canceled when the test is canceled
        /// </summary>
        /// <typeparam name="T">The task type</typeparam>
        /// <returns></returns>
        public TaskCompletionSource<T> GetTask<T>()
        {
            TaskCompletionSource<T> source = TaskUtil.GetTask<T>();

            TestCancelledTask.ContinueWith(x => source.TrySetCanceled(), TaskContinuationOptions.OnlyOnCanceled);

            return source;
        }

        public TestConsumeMessageObserver<T> GetConsumeObserver<T>()
            where T : class
        {
            return new TestConsumeMessageObserver<T>(GetTask<T>(), GetTask<T>(), GetTask<T>());
        }

        public TestConsumeObserver GetConsumeObserver()
        {
            return new TestConsumeObserver(TestTimeout, InactivityToken);
        }
    }
}
