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
		[Test]
		public void The_pool_should_not_exceed_the_maximum_thread_count()
		{
			using (ManagedThreadPool<string> pool = new ManagedThreadPool<string>(ThreadProc, 1, 4))
			{
				for (int i = 0; i < 5; i++)
				{
					pool.Enqueue("hello");
				}

				Assert.That(pool.CurrentThreadCount, Is.EqualTo(4));

				Assert.That(pool.PendingCount, Is.EqualTo(1));
			}

			Assert.That(_counter, Is.EqualTo(4));
		}

		private void ThreadProc(string value)
		{
			Thread.Sleep(5000);
			_counter++;
		}
	}
}