namespace MassTransit.Tests.Pipeline
{
	using System;
	using Magnum.Common.DateTimeExtensions;
	using MassTransit.Pipeline;
	using Messages;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class BatchConsumer_Specs
	{
		#region Setup/Teardown

		[SetUp]
		public void Setup()
		{
			_builder = MockRepository.GenerateMock<IObjectBuilder>();
		}

		#endregion

		private IObjectBuilder _builder;

		[Test]
		public void A_batch_consumer_should_be_delivered_messages()
		{
			TestBatchConsumer<IndividualBatchMessage, Guid> batchConsumer = new TestBatchConsumer<IndividualBatchMessage, Guid>();

			MessagePipeline pipeline = MessagePipelineConfigurator.CreateDefault(_builder);

			pipeline.Subscribe(batchConsumer);

			PublishBatch(pipeline);

			TimeSpan _timeout = 5.Seconds();

			batchConsumer.ShouldHaveReceivedBatch(_timeout);
		}

		private static void PublishBatch(MessagePipeline pipeline)
		{
			Guid batchId = Guid.NewGuid();
			const int _batchSize = 1;
			for (int i = 0; i < _batchSize; i++)
			{
				IndividualBatchMessage message = new IndividualBatchMessage(batchId, _batchSize);

				pipeline.Dispatch(message);
			}
		}
	}
}