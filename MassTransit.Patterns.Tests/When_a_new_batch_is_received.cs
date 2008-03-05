namespace MassTransit.Patterns.Tests
{
	using System;
	using System.Collections.Generic;
	using MassTransit.Patterns.Batching;
	using MassTransit.ServiceBus;
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
		private BatchController<StringBatchMessage, Guid> _controller;

		public void HandleBatch(IBatchContext<StringBatchMessage, Guid> context)
		{
			Guid batchId = context.BatchId;

			foreach (StringBatchMessage message in context)
			{
			}

			List<StringBatchMessage> messages = new List<StringBatchMessage>(context);
		}

		[Test]
		public void Notify_the_subscriber_with_a_batch_message()
		{
			_controller = new BatchController<StringBatchMessage, Guid>(HandleBatch);

			_bus.Subscribe<StringBatchMessage>(_controller.HandleMessage);
		}
	}
}