using System.Threading;
using log4net;

namespace MassTransit.Patterns.Tests
{
	using System;
	using Batching;
	using MassTransit.ServiceBus.Subscriptions;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using ServiceBus;

	[TestFixture]
	public class As_a_BatchController
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (As_a_BatchController));

		[SetUp]
		public void Setup()
		{
			_log.Info("Here we go!");
		}

		[Test]
		public void The_batch_should_be_complete_when_the_last_message_is_received()
		{
			ServiceBus bus = new ServiceBus(new MessageQueueEndpoint("msmq://localhost/test"), new LocalSubscriptionCache());

			bool wasCalled = false;
			bool isComplete = false;

			BatchController<MessageToBatch, Guid> c = new BatchController<MessageToBatch, Guid>(
				delegate(BatchContext<MessageToBatch, Guid> cxt)
					{
						foreach (MessageToBatch msg in cxt)
						{
							wasCalled = true;
							isComplete = cxt.IsComplete;
						}
					}, TimeSpan.FromSeconds(3));

			Guid batchId = Guid.NewGuid();
			int batchLength = 1;

			MessageToBatch msg1 = new MessageToBatch(batchId, batchLength);

			IEnvelope env1 = new Envelope(msg1);

			bus.Subscribe<MessageToBatch>(c.HandleMessage);

			bus.Deliver(env1);

			Assert.That(wasCalled, Is.True, "Not Called");
			Assert.That(isComplete, Is.True, "Not Complete");
		}

		[Test]
		public void A_missing_message_should_leave_the_batch_incomplete()
		{
			ServiceBus bus = new ServiceBus(new MessageQueueEndpoint("msmq://localhost/test"), new LocalSubscriptionCache());

			bool wasCalled = false;
			bool isComplete = false;

			BatchController<MessageToBatch, Guid> c = new BatchController<MessageToBatch, Guid>(
				delegate(BatchContext<MessageToBatch, Guid> cxt)
					{
						foreach (MessageToBatch msg in cxt)
						{
							wasCalled = true;
							isComplete = cxt.IsComplete;
						}
					}, TimeSpan.FromSeconds(3));

			Guid batchId = Guid.NewGuid();
			int batchLength = 2;

			MessageToBatch msg1 = new MessageToBatch(batchId, batchLength);

			IEnvelope env1 = new Envelope(msg1);

			bus.Subscribe<MessageToBatch>(c.HandleMessage);

			bus.Deliver(env1);

			Assert.That(wasCalled, Is.True, "Not Called");
			Assert.That(isComplete, Is.False, "Not Complete");
		}

		[Test]
		public void Multiple_messages_should_be_complete_when_the_last_message_is_received()
		{
			ServiceBus bus = new ServiceBus(new MessageQueueEndpoint("msmq://localhost/test"), new LocalSubscriptionCache());

			bool wasCalled = false;
			bool isComplete = false;
			int numberCalled = 0;

			BatchController<MessageToBatch, Guid> c = new BatchController<MessageToBatch, Guid>(
				delegate(BatchContext<MessageToBatch, Guid> cxt)
					{
						numberCalled = 0;

						foreach (MessageToBatch msg in cxt)
						{
							wasCalled = true;
							isComplete = cxt.IsComplete;
							numberCalled++;
						}
					}, TimeSpan.FromSeconds(3));

			Guid batchId = Guid.NewGuid();
			int batchLength = 4;

			MessageToBatch msg1 = new MessageToBatch(batchId, batchLength);
			MessageToBatch msg2 = new MessageToBatch(batchId, batchLength);
			MessageToBatch msg3 = new MessageToBatch(batchId, batchLength);
			MessageToBatch msg4 = new MessageToBatch(batchId, batchLength);

			IEnvelope env1 = new Envelope(msg1);
			IEnvelope env2 = new Envelope(msg2);
			IEnvelope env3 = new Envelope(msg3);
			IEnvelope env4 = new Envelope(msg4);

			bus.Subscribe<MessageToBatch>(c.HandleMessage);

			ManualResetEvent done = new ManualResetEvent(false);

			ThreadPool.QueueUserWorkItem(delegate
			                             	{
			                             		bus.Deliver(env1);
			                             		done.Set();
			                             	});

			bus.Deliver(env2);
			bus.Deliver(env3);
			bus.Deliver(env4);

			done.WaitOne(TimeSpan.FromSeconds(10), true);

			Assert.That(numberCalled, Is.EqualTo(4));
			Assert.That(wasCalled, Is.True, "Not Called");
			Assert.That(isComplete, Is.True, "Not Complete");
		}
	}
}