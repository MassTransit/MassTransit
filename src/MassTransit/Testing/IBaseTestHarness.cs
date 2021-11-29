namespace MassTransit.Testing
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public interface IBaseTestHarness
    {
        TimeSpan TestTimeout { get; set; }
        TimeSpan TestInactivityTimeout { get; set; }

        /// <summary>
        /// CancellationToken that is canceled when the test is being aborted
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// CancellationToken that is cancelled when the test inactivity timeout has elapsed with no bus activity
        /// </summary>
        CancellationToken InactivityToken { get; }

        /// <summary>
        /// Task that is completed when the bus inactivity timeout has elapsed with no bus activity
        /// </summary>
        public Task InactivityTask { get; }

        IReceivedMessageList Consumed { get; }
        IPublishedMessageList Published { get; }
        ISentMessageList Sent { get; }

        /// <summary>
        /// Sets the <see cref="CancellationToken" />, canceling the test execution
        /// </summary>
        void Cancel();

        /// <summary>
        /// Force the inactivity task to complete
        /// </summary>
        void ForceInactive();
    }
}
