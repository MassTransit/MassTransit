namespace MassTransit.ServiceBus.Tests
{
	using System;
	using System.Collections.Generic;
	using MassTransit.Patterns.Batching;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class When_a_new_batch_is_received
	{
		#region Setup/Teardown

		[SetUp]
		public void Setup()
		{
			_mocks = new MockRepository();
			_bus = _mocks.CreateMock<IServiceBus>();
		}

		#endregion

		private MockRepository _mocks;
		private IServiceBus _bus;
		private BatchController<BatchMessage<string, Guid>, Guid> _controller;

		public void HandleBatch(BatchContext<BatchMessage<string, Guid>, Guid> context)
		{
			Guid batchId = context.BatchId;

			foreach (IBatchMessage message in context)
			{
			}

			List<BatchMessage<string, Guid>> messages = new List<BatchMessage<string, Guid>>(context);
		}

		[Test]
		public void Notify_the_subscriber_with_a_batch_message()
		{
			_controller = new BatchController<BatchMessage<string, Guid>, Guid>(HandleBatch);

			_bus.Subscribe<BatchMessage<string, Guid>>(_controller.HandleMessage);
		}
	}
}