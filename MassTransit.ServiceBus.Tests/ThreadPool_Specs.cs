namespace MassTransit.ServiceBus.Tests
{
	using System;
	using System.Threading;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Threading;

	[TestFixture]
	public class When_a_item_is_scheduled_in_the_thread_pool :
        Specification
	{
		[Test]
		public void It_should_be_processed_asynchronously()
		{
			using (ManagedThreadPool<string> pool = new ManagedThreadPool<string>(ThreadProc))
			{
				_checkValue = string.Empty;

				pool.Enqueue("current");

				Assert.That(_done.WaitOne(TimeSpan.FromSeconds(30), true), Is.True);

				Assert.That(_checkValue, Is.EqualTo("current"));
			}
		}

		private string _checkValue;
		private readonly AutoResetEvent _done = new AutoResetEvent(false);

		private void ThreadProc(string value)
		{
			_checkValue = value;

			_done.Set();
		}
	}

	[TestFixture]
	public class When_multiple_long_running_jobs_are_queued : 
        Specification
	{
		private int _counter = 0;
	    private readonly int _numberOfWorkItemsToEnqueue = 100;
	    private readonly int _maxNumberOfThreads = 4;
	    private readonly int _minNumberOfThreads = 1;

		[Test]
		public void The_pool_should_not_exceed_the_maximum_thread_count()
		{
		    int expectedPendingCount = _numberOfWorkItemsToEnqueue - _maxNumberOfThreads;
		    int expectedAmountOnCounter = _maxNumberOfThreads;

			using (ManagedThreadPool<string> pool = new ManagedThreadPool<string>(TestThreadDelegate, _minNumberOfThreads, _maxNumberOfThreads))
			{
                for (int i = 0; i < _numberOfWorkItemsToEnqueue; i++)
				{
					pool.Enqueue("hello");
				}

                Assert.That(pool.CurrentThreadCount, Is.EqualTo(_maxNumberOfThreads), string.Format("Current Thread Count is {0} instead of {1}", pool.CurrentThreadCount, _maxNumberOfThreads));

                Assert.That(pool.PendingCount, Is.EqualTo(expectedPendingCount), "Pending work items was {0} instead of {1}", pool.PendingCount, expectedPendingCount);

                Assert.That(_counter, Is.EqualTo(expectedAmountOnCounter), "Counter was {0} instead of {1}", _counter, expectedAmountOnCounter);
			}
		}

		private void TestThreadDelegate(string value)
		{
			Thread.Sleep(2000);
            _counter++;
		}
	}
}