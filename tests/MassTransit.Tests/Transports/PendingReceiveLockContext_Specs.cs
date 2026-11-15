namespace MassTransit.Tests.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals;
    using MassTransit.Transports;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class PendingReceiveLockContext_Specs
    {
        /// <summary>
        /// How long the test waits for a release of the receive lock that must not happen while a cancel is running. Without
        /// the lock around the read and clear of the lock context, the release happens within microseconds of being started.
        /// </summary>
        static readonly TimeSpan _releaseWindow = TimeSpan.FromMilliseconds(500);

        [Test]
        public async Task Should_complete_the_receive_lock_after_a_concurrent_cancel()
        {
            await ReleaseWhileCanceling(lockContext => lockContext.Complete());
        }

        [Test]
        public async Task Should_fault_the_receive_lock_after_a_concurrent_cancel()
        {
            await ReleaseWhileCanceling(lockContext => lockContext.Faulted(new IntentionalTestException()));
        }

        [Test]
        public async Task Should_not_cancel_the_receive_context_once_the_receive_lock_is_complete()
        {
            var receiveContext = new TestReceiveContext(TestConsumeContext.GetContext());

            var lockContext = new PendingReceiveLockContext();
            Assert.That(lockContext.Enqueue(receiveContext, new TrackingReceiveLockContext()), Is.True);

            await lockContext.Complete();

            receiveContext.Dispose();

            Assert.That(() => lockContext.Cancel(), Throws.Nothing);
            Assert.That(lockContext.IsEmpty, Is.True);
        }

        static async Task ReleaseWhileCanceling(Func<PendingReceiveLockContext, Task> release)
        {
            var receiveContext = new TestReceiveContext(TestConsumeContext.GetContext());
            var receiveLock = new TrackingReceiveLockContext();

            var lockContext = new PendingReceiveLockContext();
            Assert.That(lockContext.Enqueue(receiveContext, receiveLock), Is.True);

            var cancelStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var finishCancel = new ManualResetEventSlim();

            // The cancellation callbacks of the receive context run inside PendingReceiveLockContext.Cancel, which holds its
            // lock until they return, so this callback holds the cancel after it has read the pending receive context
            receiveContext.CancellationToken.Register(() =>
            {
                cancelStarted.TrySetResult(true);
                finishCancel.Wait(TimeSpan.FromSeconds(30));
            });

            var cancelTask = Task.Factory.StartNew(() => lockContext.Cancel(), CancellationToken.None, TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            await cancelStarted.Task.OrTimeout(s: 30);

            // as the transports do: release the receive lock, then dispose of the receive context
            var releaseTask = Task.Factory.StartNew(async () =>
            {
                await release(lockContext).ConfigureAwait(false);

                receiveContext.Dispose();
            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();

            await Task.WhenAny(receiveLock.Released, Task.Delay(_releaseWindow));

            var releasedWhileCanceling = receiveLock.Released.IsCompleted;

            finishCancel.Set();

            await Task.WhenAll(cancelTask, releaseTask).OrTimeout(s: 30);

            Assert.That(releasedWhileCanceling, Is.False, "The receive lock was released while the receive context was being canceled");
            Assert.That(receiveLock.Released.IsCompleted, Is.True);
            Assert.That(lockContext.IsEmpty, Is.True);
        }


        class TrackingReceiveLockContext :
            ReceiveLockContext
        {
            readonly TaskCompletionSource<bool> _released = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            public Task Released => _released.Task;

            public Task Complete()
            {
                _released.TrySetResult(true);

                return Task.CompletedTask;
            }

            public Task Faulted(Exception exception)
            {
                _released.TrySetResult(true);

                return Task.CompletedTask;
            }

            public Task ValidateLockStatus()
            {
                return Task.CompletedTask;
            }
        }
    }
}
