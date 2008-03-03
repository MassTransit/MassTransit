using System.Threading;
using log4net;
using MassTransit.ServiceBus.Internal;

namespace MassTransit.Patterns.Tests
{
	using System;
	using System.Collections.Generic;
	using Batching;
	using MassTransit.ServiceBus.Subscriptions;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;
	using ServiceBus;

	[TestFixture]
	public class As_a_BatchController
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (As_a_BatchController));
		private MockRepository _mocks;
		private IEndpoint _endpoint;
		private IMessageReceiver _receiver;
		private ServiceBus _bus;

		[SetUp]
		public void Setup()
		{
			_log.Info("Here we go!");

			_mocks = new MockRepository();

			_endpoint = _mocks.DynamicMock<IEndpoint>();
			_receiver = _mocks.DynamicMock<IMessageReceiver>();

			SetupResult.For(_endpoint.Uri).Return(new Uri("msmq://localhost/test_queue"));
			SetupResult.For(_endpoint.Receiver).Return(_receiver);

			_mocks.ReplayAll();
			_bus = new ServiceBus(_endpoint, new LocalSubscriptionCache());
		}


	    [Test, Ignore("Just an idea")]
	    public void Send_as_batch()
	    {
	        IList<string> msgs = new List<string>();
            msgs.Add("chris");
            msgs.Add("dru");

            BatchMessage<string, Guid>.SendAsBatch(_bus, new Uri("msmq://localhost/test_queue"), msgs);
            BatchMessage<string, Guid>.PublishAsBatch(_bus,  msgs);
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
		public void The_batch_should_be_complete_when_the_last_message_is_received()
		{
			bool wasCalled = false;
			bool isComplete = false;

			BatchController<BatchMessage<string, Guid>, Guid> c = new BatchController<BatchMessage<string, Guid>, Guid>(
				delegate(IBatchContext<BatchMessage<string, Guid>, Guid> cxt)
					{
						foreach (BatchMessage<string, Guid> msg in cxt)
						{
							wasCalled = true;
							isComplete = cxt.IsComplete;
						}
					}, TimeSpan.FromSeconds(3));


			Guid batchId = Guid.NewGuid();
			int batchLength = 1;

			BatchMessage<string, Guid> msg1 = new BatchMessage<string, Guid>(batchId, batchLength, "hello");

			IEnvelope env1 = new Envelope(msg1);

			_bus.Subscribe<BatchMessage<string, Guid>>(c.HandleMessage);

			_bus.Deliver(env1);

			Assert.That(wasCalled, Is.True, "Not Called");
			Assert.That(isComplete, Is.True, "Not Complete");
		}

		[Test]
		public void A_missing_message_should_leave_the_batch_incomplete()
		{
			bool wasCalled = false;
			bool isComplete = false;

			BatchController<BatchMessage<string, Guid>, Guid> c = new BatchController<BatchMessage<string, Guid>, Guid>(
				delegate(IBatchContext<BatchMessage<string, Guid>, Guid> cxt)
					{
						foreach (BatchMessage<string, Guid> msg in cxt)
						{
							wasCalled = true;
							isComplete = cxt.IsComplete;
						}
					}, TimeSpan.FromSeconds(3));

			Guid batchId = Guid.NewGuid();
			int batchLength = 2;

			BatchMessage<string, Guid> msg1 = new BatchMessage<string, Guid>(batchId, batchLength, "hello");

			IEnvelope env1 = new Envelope(msg1);

			_bus.Subscribe<BatchMessage<string, Guid>>(c.HandleMessage);

			_bus.Deliver(env1);

			Assert.That(wasCalled, Is.True, "Not Called");
			Assert.That(isComplete, Is.False, "Not Complete");
		}

		[Test]
		public void Multiple_messages_should_be_complete_when_the_last_message_is_received()
		{
			bool wasCalled = false;
			bool isComplete = false;
			int numberCalled = 0;

			BatchController<BatchMessage<string, Guid>, Guid> c = new BatchController<BatchMessage<string, Guid>, Guid>(
				delegate(IBatchContext<BatchMessage<string, Guid>, Guid> cxt)
					{
						numberCalled = 0;

						foreach (BatchMessage<string, Guid> msg in cxt)
						{
							wasCalled = true;
							isComplete = cxt.IsComplete;
							numberCalled++;
						}
					}, TimeSpan.FromSeconds(3));

			Guid batchId = Guid.NewGuid();
			int batchLength = 4;

			BatchMessage<string, Guid> msg1 = new BatchMessage<string, Guid>(batchId, batchLength, "hello");
			BatchMessage<string, Guid> msg2 = new BatchMessage<string, Guid>(batchId, batchLength, "hello");
			BatchMessage<string, Guid> msg3 = new BatchMessage<string, Guid>(batchId, batchLength, "hello");
			BatchMessage<string, Guid> msg4 = new BatchMessage<string, Guid>(batchId, batchLength, "hello");

			IEnvelope env1 = new Envelope(msg1);
			IEnvelope env2 = new Envelope(msg2);
			IEnvelope env3 = new Envelope(msg3);
			IEnvelope env4 = new Envelope(msg4);

			_bus.Subscribe<BatchMessage<string, Guid>>(c.HandleMessage);

			ManualResetEvent started = new ManualResetEvent(false);
			ManualResetEvent done = new ManualResetEvent(false);

			ThreadPool.QueueUserWorkItem(delegate
			                             	{
			                             		started.Set();
												_bus.Deliver(env1);
			                             		done.Set();
			                             	});

			started.WaitOne(TimeSpan.FromSeconds(3), true);

			_bus.Deliver(env2);
			_bus.Deliver(env3);
			_bus.Deliver(env4);

			done.WaitOne(TimeSpan.FromSeconds(10), true);

			Assert.That(wasCalled, Is.True, "Not Called");
			Assert.That(numberCalled, Is.EqualTo(4));
			Assert.That(isComplete, Is.True, "Not Complete");
		}

        [Test]
        public void A_timeout_should_leave_the_batch_incomplete() //should it do more?
        {
            bool wasCalled = false;
            bool isComplete = false;

            BatchController<BatchMessage<string, Guid>, Guid> c = new BatchController<BatchMessage<string, Guid>, Guid>(
                delegate(IBatchContext<BatchMessage<string, Guid>, Guid> cxt)
                {
                    foreach (BatchMessage<string, Guid> msg in cxt)
                    {
                        wasCalled = true;
                        isComplete = cxt.IsComplete;
                    }
                }, TimeSpan.FromSeconds(3));

            Guid batchId = Guid.NewGuid();
            int batchLength = 2;

            BatchMessage<string, Guid> msg1 = new BatchMessage<string, Guid>(batchId, batchLength, "hello");

            IEnvelope env1 = new Envelope(msg1);

            _bus.Subscribe<BatchMessage<string, Guid>>(c.HandleMessage);

            _bus.Deliver(env1);
            Thread.Sleep(3005);
            _bus.Deliver(env1);

            Assert.That(wasCalled, Is.True, "Not Called");
            Assert.That(isComplete, Is.False, "Not Complete");
        }
	}
}