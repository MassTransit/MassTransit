namespace MassTransit.ServiceBus.Tests
{
	using System;
	using System.Collections;
	using System.Threading;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;

	[TestFixture]
	public class When_a_batch_message_arrives :
		Specification
	{
        private IObjectBuilder _builder;
        private IEndpoint _endpoint;
        private ServiceBus _bus;

        protected override void Before_each()
        {
            _builder = DynamicMock<IObjectBuilder>();
            SetupResult.For(_builder.Build<BatchConsumer>(new Hashtable())).IgnoreArguments().Return(new BatchConsumer());

            _endpoint = DynamicMock<IEndpoint>();
            SetupResult.For(_endpoint.Uri).Return(new Uri("msmq://localhost/queue"));

            _bus = new ServiceBus(_endpoint, _builder);
        }

        protected override void After_each()
        {
            _bus.Dispose();
            _bus = null;

            _endpoint.Dispose();
            _endpoint = null;
        }

		[Test]
		public void A_large_batch_should_work_properly()
		{
			TestBatch(712);
		}

		[Test]
		public void The_consumer_method_should_be_called()
		{
			TestBatch(1);
		}

		[Test]
		public void When_a_message_fails_to_arrive_the_batch_should_timeout()
		{
			ReplayAll();

			BatchConsumer consumer = new BatchConsumer();

			_bus.Subscribe(consumer);

			Guid batchId = Guid.NewGuid();
			IndividualMessage message = new IndividualMessage(batchId, 2);

			_bus.Subscribe<BatchTimeout<IndividualMessage, Guid>>(delegate { });

			Assert.That(_bus.Accept(message), Is.True, "Bus did not accept the message");

			_bus.Dispatch(message, DispatchMode.Asynchronous);

            Assert.That(BatchConsumer.Received.WaitOne(TimeSpan.FromSeconds(5), true), Is.False, "Batch should not have completed");
            Assert.That(BatchConsumer.BatchId, Is.EqualTo(batchId));
            Assert.That(BatchConsumer.ReceivedCount, Is.EqualTo(1));
		}

        [Test]
        public void Verify_that_container_based_consumers_are_operational()
        {
            ReplayAll();

            _bus.AddComponent<BatchConsumer>();

            Guid batchId = Guid.NewGuid();
            int length = 123;
            for (int i = 0; i < length; i++)
            {
                IndividualMessage message = new IndividualMessage(batchId, length);

                Assert.That(_bus.Accept(message), Is.True, "Bus did not accept the message");

                _bus.Dispatch(message, DispatchMode.Asynchronous);
            }

            Assert.That(BatchConsumer.Received.WaitOne(TimeSpan.FromSeconds(3), true), Is.True, "Timeout waiting for message");
            Assert.That(BatchConsumer.BatchId, Is.EqualTo(batchId));
            Assert.That(BatchConsumer.ReceivedCount, Is.EqualTo(length));
        }

		

		private void TestBatch(int length)
		{
			ReplayAll();

			BatchConsumer consumer = new BatchConsumer();

			_bus.Subscribe(consumer);

			Guid batchId = Guid.NewGuid();
			for (int i = 0; i < length; i++)
			{
				IndividualMessage message = new IndividualMessage(batchId, length);

				Assert.That(_bus.Accept(message), Is.True, "Bus did not accept the message");

				_bus.Dispatch(message, DispatchMode.Asynchronous);
			}

            Assert.That(BatchConsumer.Received.WaitOne(TimeSpan.FromSeconds(3), true), Is.True, "Timeout waiting for message");
            Assert.That(BatchConsumer.BatchId, Is.EqualTo(batchId));
            Assert.That(BatchConsumer.ReceivedCount, Is.EqualTo(length));
		}


		internal class BatchConsumer : 
            Consumes<Batch<IndividualMessage, Guid>>.Selected
		{
			private static ManualResetEvent _received;
			private static Guid _batchId;
			private static int _receivedCount;

            public BatchConsumer()
            {
                _received = new ManualResetEvent(false);
                _batchId = Guid.Empty;
                _receivedCount = 0;
            }
			public static int ReceivedCount
			{
				get { return _receivedCount; }
			}

			public static ManualResetEvent Received
			{
				get { return _received; }
			}

			public static Guid BatchId
			{
				get { return _batchId; }
			}

			public void Consume(Batch<IndividualMessage, Guid> batch)
			{
				_batchId = batch.BatchId;

				foreach (IndividualMessage message in batch)
				{
					_receivedCount++;
				}

				if (_receivedCount == batch.BatchLength)
					_received.Set();
			}

			public bool Accept(Batch<IndividualMessage, Guid> message)
			{
				return true;
			}
		}
	}


	[Timeout(Seconds = 3)]
	internal class IndividualMessage : BatchedBy<Guid>
	{
		private readonly Guid _batchId;
		private readonly int _batchLength;

		public IndividualMessage(Guid batchId, int batchLength)
		{
			_batchId = batchId;
			_batchLength = batchLength;
		}

		public Guid BatchId
		{
			get { return _batchId; }
		}

		public int BatchLength
		{
			get { return _batchLength; }
		}
	}
}