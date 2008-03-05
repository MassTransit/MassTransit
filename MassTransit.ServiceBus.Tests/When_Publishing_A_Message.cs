namespace MassTransit.ServiceBus.Tests
{
	using System;
	using Internal;
	using MassTransit.ServiceBus.Subscriptions;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class When_Publishing_A_Message
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			mockBusEndpoint = mocks.CreateMock<IEndpoint>();
			mockSubscriptionStorage = mocks.CreateMock<ISubscriptionStorage>();
			mockSender = mocks.CreateMock<IMessageSender>();
			mockReceiver = mocks.CreateMock<IMessageReceiver>();
		}

		[TearDown]
		public void TearDown()
		{
			mocks = null;
			mockBusEndpoint = null;
			mockSubscriptionStorage = null;
			_serviceBus = null;
			mockReceiver = null;
		}

		#endregion

		private ServiceBus _serviceBus;
		private MockRepository mocks;
		private IEndpoint mockBusEndpoint;
		private ISubscriptionStorage mockSubscriptionStorage;
		private IMessageSender mockSender;
		private IMessageReceiver mockReceiver;

		private string queueName = @".\private$\test";
		private Uri queueUri = new Uri("msmq://" + Environment.MachineName + "/test");


		[Test]
		public void Poison_Letters_Should_Be_Moved_To_A_Poison_Queue()
		{
			using (mocks.Record())
			{
				Expect.Call(mockBusEndpoint.Receiver).Return(mockReceiver).Repeat.Any();
				Expect.Call(delegate { mockReceiver.Subscribe(null); }).IgnoreArguments().Repeat.Any();
				Expect.Call(mockBusEndpoint.Uri).Return(queueUri).Repeat.Any();

				mockSubscriptionStorage.Add(new Subscription(typeof (PoisonMessage).FullName, queueUri));

				//Expect.Call(mockBusEndpoint.Sender).Return(mockSender);

				//          mockSender.Send(null);
				//        LastCall.IgnoreArguments(); //because we can't control the Envelope from here?
			}
			using (mocks.Playback())
			{
				_serviceBus = new ServiceBus(mockBusEndpoint);
			    _serviceBus.SubscriptionStorage = mockSubscriptionStorage;

				////this ends up in a seperate thread and I am therefore unable to figure out how to test
				_serviceBus.Subscribe<PoisonMessage>(delegate(IMessageContext<PoisonMessage> cxt)
				                                     	{
				                                     		try
				                                     		{
				                                     			cxt.Message.ThrowException();
				                                     			Assert.Fail("No Exception was thrown");
				                                     		}
				                                     		catch (Exception)
				                                     		{
				                                     			cxt.MarkPoison();
				                                     		}
				                                     	}
					);

				_serviceBus.Deliver(new Envelope(new PoisonMessage()));
			}
		}
	}
}