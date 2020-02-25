namespace MassTransit.TestFramework
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Testing;
    using Testing.Observers;


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
        /// Timeout for the test, used for any delay timers
        /// </summary>
        protected TimeSpan TestTimeout
        {
            get { return AsyncTestHarness.TestTimeout; }
            set { AsyncTestHarness.TestTimeout = value; }
        }

        /// <summary>
        /// Forces the test to be cancelled, aborting any awaiting tasks
        /// </summary>
        protected void CancelTest()
        {
            AsyncTestHarness.CancelTest();
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

        protected TestObserver<T> GetObserver<T>()
            where T : class
        {
            return AsyncTestHarness.GetObserver<T>();
        }

        protected TestSendObserver GetSendObserver()
        {
            return AsyncTestHarness.GetSendObserver();
        }

        /// <summary>
        /// Await a task in a test method that is not asynchronous, such as a test fixture setup
        /// </summary>
        /// <param name="taskFactory"></param>
        protected void Await(Func<Task> taskFactory)
        {
            AsyncTestHarness.Await(taskFactory);
        }

        /// <summary>
        /// Await a task in a test method that is not asynchronous, such as a test fixture setup
        /// </summary>
        /// <param name="taskFactory"></param>
        /// <param name="cancellationToken"></param>
        protected void Await(Func<Task> taskFactory, CancellationToken cancellationToken)
        {
            AsyncTestHarness.Await(taskFactory, cancellationToken);
        }

        /// <summary>
        /// Await a task in a test method that is not asynchronous, such as a test fixture setup
        /// </summary>
        /// <param name="taskFactory"></param>
        protected T Await<T>(Func<Task<T>> taskFactory)
        {
            return AsyncTestHarness.Await(taskFactory);
        }

        /// <summary>
        /// Await a task in a test method that is not asynchronous, such as a test fixture setup
        /// </summary>
        /// <param name="taskFactory"></param>
        /// <param name="cancellationToken"></param>
        protected T Await<T>(Func<Task<T>> taskFactory, CancellationToken cancellationToken)
        {
            return AsyncTestHarness.Await(taskFactory, cancellationToken);
        }
    }
}
