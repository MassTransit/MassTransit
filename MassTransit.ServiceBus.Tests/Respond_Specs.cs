namespace MassTransit.ServiceBus.Tests
{
	using System;
	using Internal;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;

	[TestFixture]
	public class When_responding_to_a_message
	{
		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();

			mockReceiver = mocks.DynamicMock<IMessageReceiver>();

			mockServiceBusEndPoint = mocks.DynamicMock<IEndpoint>();

			mockSender = mocks.DynamicMock<IMessageSender>();

			_serviceBus = new ServiceBus(mockServiceBusEndPoint);
		}

		[TearDown]
		public void TearDown()
		{
			mocks = null;
			_serviceBus = null;
			mockServiceBusEndPoint = null;
		}

		private MockRepository mocks;
		private ServiceBus _serviceBus;
		private IEndpoint mockServiceBusEndPoint;
		private IMessageReceiver mockReceiver;
		private IMessageSender mockSender;

		[Test]
		public void The_current_information_should_allow_a_response_to_be_sent()
		{
			using (mocks.Record())
			{
				Expect.Call(mockServiceBusEndPoint.Uri).Return(new Uri("msmq://localhost/test_servicebus")).Repeat.Any(); //stupid log4net
				Expect.Call(mockServiceBusEndPoint.Receiver).Return(mockReceiver);
				Expect.Call(mockServiceBusEndPoint.Sender).Return(mockSender);
				Expect.Call(delegate { mockReceiver.Subscribe(null); }).IgnoreArguments().Repeat.Any();
			}

			using (mocks.Playback())
			{
				bool called = false;

				_serviceBus.Subscribe<PingMessage>(
					delegate
						{
							Respond.With(new PongMessage());
							 called = true;
						});

				IEnvelope envelope = new Envelope(mockServiceBusEndPoint, new PingMessage());
				_serviceBus.Deliver(envelope);

				Assert.That(called, Is.True);
			}
		}
	}

	//    [Test]
	//public void An_Event_Handler_Should_Be_Called()
	//{
	//    using (mocks.Record())
	//    {
	//        Expect.Call(mockServiceBusEndPoint.Uri).Return(new Uri("msmq://localhost/test_servicebus")).Repeat.Any(); //stupid log4net
	//        Expect.Call(mockServiceBusEndPoint.Receiver).Return(mockReceiver);
	//        Expect.Call(delegate { mockReceiver.Subscribe(null); }).IgnoreArguments().Repeat.Any();
	//    }
	//    using (mocks.Playback())
	//    {
	//        bool _received = false;

	//        MessageReceivedCallback<PingMessage> handler = delegate { _received = true; };

	//        _serviceBus.Subscribe(handler);

	//        IEnvelope envelope = new Envelope(mockServiceBusEndPoint, new PingMessage());
	//        _serviceBus.Deliver(envelope);

	//        Assert.That(_received, Is.True);
	//    }
	//}
}