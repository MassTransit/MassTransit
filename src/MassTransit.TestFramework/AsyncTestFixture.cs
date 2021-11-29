namespace MassTransit.TestFramework
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Testing;
    using Testing.Implementations;


    public abstract class AsyncTestFixture
    {
        protected AsyncTestFixture(AsyncTestHarness harness)
        {
            AsyncTestHarness = harness;
        }

        protected AsyncTestHarness AsyncTestHarness { get; }

        /// <summary>
        /// Task that is canceled when the test is aborted, for continueWith usage
        /// </summary>
        protected Task TestCancelledTask => AsyncTestHarness.TestCancelledTask;

        /// <summary>
        /// CancellationToken that is canceled when the test is being aborted
        /// </summary>
        protected CancellationToken TestCancellationToken => AsyncTestHarness.TestCancellationToken;

        /// <summary>
        /// CancellationToken that is cancelled when the test inactivity timeout has elapsed with no bus activity
        /// </summary>
        protected CancellationToken InactivityToken => AsyncTestHarness.InactivityToken;

        /// <summary>
        /// Task that is completed when the bus inactivity timeout has elapsed with no bus activity
        /// </summary>
        protected Task InactivityTask => AsyncTestHarness.InactivityTask;

        /// <summary>
        /// Timeout for the test, used for any delay timers
        /// </summary>
        protected TimeSpan TestTimeout
        {
            get => AsyncTestHarness.TestTimeout;
            set => AsyncTestHarness.TestTimeout = value;
        }

        /// <summary>
        /// Timeout for detecting bus activity
        /// </summary>
        protected TimeSpan TestInactivityTimeout
        {
            get => AsyncTestHarness.TestInactivityTimeout;
            set => AsyncTestHarness.TestInactivityTimeout = value;
        }

        /// <summary>
        /// Forces the test to be cancelled, aborting any awaiting tasks
        /// </summary>
        protected void CancelTest()
        {
            AsyncTestHarness.Cancel();
        }

        /// <summary>
        /// Returns a task completion that is automatically canceled when the test is canceled
        /// </summary>
        /// <typeparam name="T">The task type</typeparam>
        /// <returns></returns>
        public TaskCompletionSource<T> GetTask<T>()
        {
            return AsyncTestHarness.GetTask<T>();
        }

        protected TestConsumeMessageObserver<T> GetConsumeObserver<T>()
            where T : class
        {
            return AsyncTestHarness.GetConsumeObserver<T>();
        }

        protected TestConsumeObserver GetConsumeObserver()
        {
            return AsyncTestHarness.GetConsumeObserver();
        }
    }
}
