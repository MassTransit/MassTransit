namespace MassTransit.Patterns.Tests
{
	using System;
	using System.Threading;
	using MassTransit.ServiceBus.Internal;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;
	using ServiceBus;

	[TestFixture]
	public class When_a_batch_message_arrives
	{
		[SetUp]
		public void Setup()
		{
			_mocks = new MockRepository();
			_endpoint = _mocks.DynamicMock<IEndpoint>();
			SetupResult.For(_endpoint.Uri).Return(new Uri("msmq://localhost/queue"));

			_bus = new ServiceBus(_endpoint);

			_mocks.ReplayAll();
		}

		[TearDown]
		public void Teardown()
		{
			_bus.Dispose();
			_bus = null;

			_endpoint.Dispose();
			_endpoint = null;

			_mocks = null;
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

		private void TestBatch(int length)
		{
			BatchConsumer consumer = new BatchConsumer();

			BatchDistributor<IndividualMessage, Guid> distributor = new BatchDistributor<IndividualMessage, Guid>(consumer);

			_bus.Subscribe(distributor);

			Guid batchId = Guid.NewGuid();
			for (int i = 0; i < length; i++)
			{
				IndividualMessage message = new IndividualMessage(batchId, length);

				Assert.That(_bus.Accept(message), Is.True, "Bus did not accept the message");

				_bus.Dispatch(message, DispatchMode.Asynchronous);
			}

			Assert.That(consumer.Received.WaitOne(TimeSpan.FromSeconds(3), true), Is.True, "Timeout waiting for message");
			Assert.That(consumer.BatchId, Is.EqualTo(batchId));
			Assert.That(consumer.ReceivedCount, Is.EqualTo(length));
		}

		private MockRepository _mocks;
		private IEndpoint _endpoint;
		private ServiceBus _bus;


		internal class BatchConsumer : Consumes<Batch<IndividualMessage, Guid>>.Selected
		{
			private readonly ManualResetEvent _received = new ManualResetEvent(false);
			private Guid _batchId;
			private int _receivedCount;

			public int ReceivedCount
			{
				get { return _receivedCount; }
			}

			public ManualResetEvent Received
			{
				get { return _received; }
			}

			public Guid BatchId
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

				_received.Set();
			}

			public bool Accept(Batch<IndividualMessage, Guid> message)
			{
				return true;
			}
		}
	}

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

namespace MassTransit.ServiceBus
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Threading;

}