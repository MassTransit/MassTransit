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
	    private readonly int _numberOfWorkItemsToEnqueue = 5;
	    private readonly int _maxNumberOfThreads = 4;
	    private readonly int _minNumberOfThreads = 1;

		[Test]
		public void The_pool_should_not_exceed_the_maximum_thread_count()
		{
		    int expectedPendingCount = _numberOfWorkItemsToEnqueue - _maxNumberOfThreads;
            int expectedAmountOnCounter = _numberOfWorkItemsToEnqueue;

			using (ManagedThreadPool<string> pool = new ManagedThreadPool<string>(TestThreadDelegate, _minNumberOfThreads, _maxNumberOfThreads))
			{
                for (int i = 0; i < _numberOfWorkItemsToEnqueue; i++)
				{
					pool.Enqueue("hello");
				}

                Assert.That(pool.CurrentThreadCount, Is.EqualTo(_maxNumberOfThreads), string.Format("Current Thread Count is {0} instead of {1}", pool.CurrentThreadCount, _maxNumberOfThreads));

                Assert.That(pool.PendingCount, Is.EqualTo(expectedPendingCount), "Pending work items was {0} instead of {1}", pool.PendingCount, expectedPendingCount);

			}

            Assert.That(_counter, Is.EqualTo(expectedAmountOnCounter), "Counter should be {0} instead of {1} after a dispose", expectedAmountOnCounter, _counter);
		}

	    [Test]
	    public void The_pool_should_complete_with_a_high_level_of_level_of_work()
	    {
            using(ManagedThreadPool<string> pool = new ManagedThreadPool<string>(TestThreadDelegate, _minNumberOfThreads, 10))
            {
                //5000 work items takes about 103 seconds
                int numberOfWorkItems = 5000;
                for(int i = 0; i < numberOfWorkItems; i++)
                {
                    pool.Enqueue("hello");
                }

            }
	    }

	    [Test]
	    public void As_a_thread_pool_I_should_be_able_to_run_out_of_work_and_start_back_up()
	    {
            using (ManagedThreadPool<string> pool = new ManagedThreadPool<string>(TestThreadDelegate, _minNumberOfThreads, 10))
            {
                pool.Enqueue("dru");
                Thread.Sleep(300);
                pool.Enqueue("dru");
                Thread.Sleep(300);

                Assert.AreEqual(2, _counter);

            }
	    }


		private void TestThreadDelegate(string value)
		{
			Thread.Sleep(200);
            _counter++;
		}
	}
}