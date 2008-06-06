namespace MassTransit.Patterns.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using Batching;
	using log4net;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;
	using ServiceBus;

	[TestFixture]
	public class As_a_BatchController
	{
		[SetUp]
		public void Setup()
		{
			_log.Info("Here we go!");

			_mocks = new MockRepository();

			_endpoint = _mocks.DynamicMock<IEndpoint>();

			SetupResult.For(_endpoint.Uri).Return(new Uri("msmq://localhost/test_queue"));

			_mocks.ReplayAll();
			_bus = new ServiceBus(_endpoint);
		}

		[Test]
		public void A_missing_message_should_leave_the_batch_incomplete()
		{
			bool wasCalled = false;
			bool isComplete = false;

			BatchController<StringBatchMessage, Guid> c = new BatchController<StringBatchMessage, Guid>(
				delegate(IBatchContext<StringBatchMessage, Guid> cxt)
					{
						foreach (StringBatchMessage msg in cxt)
						{
							wasCalled = true;
							isComplete = cxt.IsComplete;
						}
					}, TimeSpan.FromSeconds(3));

			Guid batchId = Guid.NewGuid();
			int batchLength = 2;

			StringBatchMessage msg1 = new StringBatchMessage(batchId, batchLength, "hello");

			_bus.Subscribe<StringBatchMessage>(c.HandleMessage);

			_bus.Dispatch(msg1);

			Assert.That(wasCalled, Is.True, "Not Called");
			Assert.That(isComplete, Is.False, "Not Complete");
		}

		[Test]
		public void A_timeout_should_leave_the_batch_incomplete() //should it do more?
		{
			bool wasCalled = false;
			bool isComplete = false;

			BatchController<StringBatchMessage, Guid> c = new BatchController<StringBatchMessage, Guid>(
				delegate(IBatchContext<StringBatchMessage, Guid> cxt)
					{
						foreach (StringBatchMessage msg in cxt)
						{
							wasCalled = true;
							isComplete = cxt.IsComplete;
						}
					}, TimeSpan.FromSeconds(3));

			Guid batchId = Guid.NewGuid();
			int batchLength = 2;

			StringBatchMessage msg1 = new StringBatchMessage(batchId, batchLength, "hello");

			_bus.Subscribe<StringBatchMessage>(c.HandleMessage);

			_bus.Dispatch(msg1);
			Thread.Sleep(3005);
			_bus.Dispatch(msg1);

			Assert.That(wasCalled, Is.True, "Not Called");
			Assert.That(isComplete, Is.False, "Not Complete");
		}

		[Test]
		public void BM_Equals()
		{
			MessageToBatch m = new MessageToBatch();
			BatchMessage<MessageToBatch, Guid> bm = new BatchMessage<MessageToBatch, Guid>(Guid.NewGuid(), 2, m);
			BatchMessage<MessageToBatch, Guid> bm2 = new BatchMessage<MessageToBatch, Guid>(Guid.NewGuid(), 2, m);

			Assert.AreEqual(bm, bm2);
		}

		[Test]
		public void Multiple_messages_should_be_complete_when_the_last_message_is_received()
		{
			bool wasCalled = false;
			bool isComplete = false;
			int numberCalled = 0;

			BatchController<StringBatchMessage, Guid> c = new BatchController<StringBatchMessage, Guid>(
				delegate(IBatchContext<StringBatchMessage, Guid> cxt)
					{
						numberCalled = 0;

						foreach (StringBatchMessage msg in cxt)
						{
							wasCalled = true;
							isComplete = cxt.IsComplete;
							numberCalled++;
						}
					}, TimeSpan.FromSeconds(3));

			Guid batchId = Guid.NewGuid();
			int batchLength = 4;

			StringBatchMessage msg1 = new StringBatchMessage(batchId, batchLength, "hello");
			StringBatchMessage msg2 = new StringBatchMessage(batchId, batchLength, "hello");
			StringBatchMessage msg3 = new StringBatchMessage(batchId, batchLength, "hello");
			StringBatchMessage msg4 = new StringBatchMessage(batchId, batchLength, "hello");

			_bus.Subscribe<StringBatchMessage>(c.HandleMessage);

			ManualResetEvent started = new ManualResetEvent(false);
			ManualResetEvent done = new ManualResetEvent(false);

			ThreadPool.QueueUserWorkItem(delegate
			                             	{
			                             		started.Set();
			                             		_bus.Dispatch(msg1);
			                             		done.Set();
			                             	});

			started.WaitOne(TimeSpan.FromSeconds(3), true);

			_bus.Dispatch(msg2);
			_bus.Dispatch(msg3);
			_bus.Dispatch(msg4);

			done.WaitOne(TimeSpan.FromSeconds(10), true);

			Assert.That(wasCalled, Is.True, "Not Called");
			Assert.That(numberCalled, Is.EqualTo(4));
			Assert.That(isComplete, Is.True, "Not Complete");
		}

		[Test, Ignore("Just an idea")]
		public void Send_as_batch()
		{
			IList<string> msgs = new List<string>();
			msgs.Add("chris");
			msgs.Add("dru");

			StringBatchMessage.SendAsBatch(_bus, new Uri("msmq://localhost/test_queue"), msgs);
			StringBatchMessage.PublishAsBatch(_bus, msgs);
		}

		[Test]
		public void The_batch_should_be_complete_when_the_last_message_is_received()
		{
			bool wasCalled = false;
			bool isComplete = false;

			BatchController<StringBatchMessage, Guid> c = new BatchController<StringBatchMessage, Guid>(
				delegate(IBatchContext<StringBatchMessage, Guid> cxt)
					{
						foreach (StringBatchMessage msg in cxt)
						{
							wasCalled = true;
							isComplete = cxt.IsComplete;
						}
					}, TimeSpan.FromSeconds(3));


			Guid batchId = Guid.NewGuid();
			int batchLength = 1;

			StringBatchMessage msg1 = new StringBatchMessage(batchId, batchLength, "hello");

			_bus.Subscribe<StringBatchMessage>(c.HandleMessage);

			_bus.Dispatch(msg1);

			Assert.That(wasCalled, Is.True, "Not Called");
			Assert.That(isComplete, Is.True, "Not Complete");
		}

		private static readonly ILog _log = LogManager.GetLogger(typeof (As_a_BatchController));
		private MockRepository _mocks;
		private IEndpoint _endpoint;
		private ServiceBus _bus;
	}

    public class StringBatchMessage : BatchMessage<string, Guid>
    {
        public StringBatchMessage(Guid batchId, int batchLength, string body) : base(batchId, batchLength, body)
        {
        }
    }
}