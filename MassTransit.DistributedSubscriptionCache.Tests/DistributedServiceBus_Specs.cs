namespace MassTransit.DistributedSubscriptionCache.Tests
{
	using System;
	using System.Threading;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using ServiceBus;

	[TestFixture]
	public class When_multiple_service_bus_instances_share_a_distributed_subscription_cache
	{
		[Test]
		public void Correlated_messages_should_be_delivered()
		{
			using (DistributedQueueContext dqc = new DistributedQueueContext())
			{
				Guid correlationId = Guid.NewGuid();

				CorrelatedConsumer consumer = new CorrelatedConsumer(correlationId);

				dqc.RemoteServiceBus.Subscribe(consumer);

				CorrelatedMessage dm = new CorrelatedMessage(correlationId);

				dqc.ServiceBus.Publish(dm);

				Assert.That(consumer.Received.WaitOne(TimeSpan.FromSeconds(6), true), Is.True,
				            "Timeout expired waiting for message");
			}
		}

		[Test]
		public void Correlated_messages_should_be_delivered_to_both_correlated_and_regular_consumers()
		{
			using (DistributedQueueContext dqc = new DistributedQueueContext())
			{
				ManualResetEvent _received = new ManualResetEvent(false);

				dqc.RemoteServiceBus.Subscribe<CorrelatedMessage>(
					delegate { _received.Set(); });

				Guid correlationId = Guid.NewGuid();

				CorrelatedConsumer consumer = new CorrelatedConsumer(correlationId);

				dqc.RemoteServiceBus.Subscribe(consumer);

				CorrelatedMessage dm = new CorrelatedMessage(correlationId);

				dqc.ServiceBus.Publish(dm);

				Assert.That(consumer.Received.WaitOne(TimeSpan.FromSeconds(6), true), Is.True,
				            "Timeout expired waiting for message");

				Assert.That(_received.WaitOne(TimeSpan.FromSeconds(6), true), Is.True,
							"Timeout expired waiting for message");
			}
		}

		[Test]
		public void Regular_messages_should_be_delivered()
		{
			using (DistributedQueueContext dqc = new DistributedQueueContext())
			{
				ManualResetEvent _received = new ManualResetEvent(false);

				dqc.RemoteServiceBus.Subscribe<PublishedMessage>(
					delegate { _received.Set(); });

				PublishedMessage dm = new PublishedMessage();

				dqc.ServiceBus.Publish(dm);

				Assert.That(_received.WaitOne(TimeSpan.FromSeconds(6), true), Is.True,
				            "Timeout expired waiting for message");
			}
		}

		[Serializable]
		internal class PublishedMessage : IMessage
		{
		}

		[Serializable]
		internal class CorrelatedMessage : CorrelatedBy<Guid>, IMessage
		{
			private readonly Guid _correlationId;

			public CorrelatedMessage(Guid correlationId)
			{
				_correlationId = correlationId;
			}

			public Guid CorrelationId
			{
				get { return _correlationId; }
			}
		}

		internal class CorrelatedConsumer : Consumes<CorrelatedMessage>.For<Guid>
		{
			private readonly Guid _correlationId;
			private readonly ManualResetEvent _received = new ManualResetEvent(false);

			public CorrelatedConsumer(Guid correlationId)
			{
				_correlationId = correlationId;
			}

			public ManualResetEvent Received
			{
				get { return _received; }
			}

			public void Consume(CorrelatedMessage message)
			{
				_received.Set();
			}

			public Guid CorrelationId
			{
				get { return _correlationId; }
			}
		}
	}
}