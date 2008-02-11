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
        private BatchController<BatchMessage, Guid> _controller;

        public void HandleBatch(BatchContext<BatchMessage, Guid> context)
        {
            Guid batchId = context.BatchId;

            foreach (BatchMessage<Guid> message in context)
            {
            }

            List<BatchMessage> messages = new List<BatchMessage>(context);
        }

        [Test]
        public void Notify_the_subscriber_with_a_batch_message()
        {
            _controller = new BatchController<BatchMessage, Guid>(HandleBatch);

            _bus.Subscribe<BatchMessage>(_controller.HandleMessage);
        }
    }

    [Serializable]
    public class BatchMessage :
        BatchMessage<Guid>
    {
        public BatchMessage(Guid batchId, int batchLength) : base(batchId, batchLength)
        {
        }
    }
}