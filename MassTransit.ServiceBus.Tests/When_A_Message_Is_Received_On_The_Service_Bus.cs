namespace MassTransit.ServiceBus.Tests
{
	using System;
	using Internal;
	using MassTransit.ServiceBus.Subscriptions;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;

	[TestFixture]
	public class When_A_Message_Is_Received_On_The_Service_Bus
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			mockServiceBusEndPoint = mocks.CreateMock<IEndpoint>();
			mockReceiver = mocks.CreateMock<IMessageReceiver>();
			_serviceBus = new ServiceBus(mockServiceBusEndPoint);
		}

		[TearDown]
		public void TearDown()
		{
			mocks = null;
			_serviceBus = null;
			mockServiceBusEndPoint = null;
		}

		#endregion

		private MockRepository mocks;
		private ServiceBus _serviceBus;
		private IEndpoint mockServiceBusEndPoint;
		private IMessageReceiver mockReceiver;

		[Test]
		public void An_Event_Handler_Should_Be_Called()
		{
			using (mocks.Record())
			{
				Expect.Call(mockServiceBusEndPoint.Uri).Return(new Uri("msmq://localhost/test_servicebus")).Repeat.Any(); //stupid log4net
				Expect.Call(mockServiceBusEndPoint.Receiver).Return(mockReceiver);
				Expect.Call(delegate { mockReceiver.Subscribe(null); }).IgnoreArguments().Repeat.Any();
			}
			using (mocks.Playback())
			{
				bool _received = false;

				MessageReceivedCallback<PingMessage> handler = delegate { _received = true; };

				_serviceBus.Subscribe(handler);

				IEnvelope envelope = new Envelope(mockServiceBusEndPoint, new PingMessage());
				_serviceBus.Deliver(envelope);

				Assert.That(_received, Is.True);
			}
		}

		[Test]
		public void What_Happens_If_No_Subscriptions()
		{
			using (mocks.Record())
			{
				Expect.Call(mockServiceBusEndPoint.Uri).Return(new Uri("msmq://localhost/test_servicebus")).Repeat.Any(); //stupid log4net
			}
			using (mocks.Playback())
			{
				_serviceBus = new ServiceBus(mockServiceBusEndPoint);

				bool _received = false;

				IEnvelope envelope = new Envelope(mockServiceBusEndPoint, new PingMessage());
				_serviceBus.Deliver(envelope);

				Assert.That(_received, Is.False);
			}
		}
	}
}