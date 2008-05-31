namespace MassTransit.ServiceBus.Tests
{
	using System;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;

	[TestFixture]
	public class MessageQueueEndpoint_MeetsCriteria
	{
		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			mockServiceBusEndPoint = mocks.CreateMock<IEndpoint>();
		}

		[TearDown]
		public void TearDown()
		{
			mocks = null;
			mockServiceBusEndPoint = null;
			_serviceBus = null;
		}

		[Test]
		public void dru_test()
		{
			bool workDid = false;

			using (mocks.Record())
			{
				Expect.Call(mockServiceBusEndPoint.Uri).Return(new Uri("msmq://localhost/test_servicebus")).Repeat.Any(); //stupid log4net
			}
			using (mocks.Playback())
			{
				_serviceBus = new ServiceBus(mockServiceBusEndPoint);

				_serviceBus.Subscribe<PingMessage>(
					delegate { workDid = true; },
					delegate { return true; });

				Assert.That(_serviceBus.Accept(new PingMessage()), Is.True);

				_serviceBus.Dispatch(new PingMessage());
				Assert.That(workDid, Is.True, "Lazy Test!");
			}
		}

		//[Test]
		//public void NAME()
		//{
		//    string input = @"FormatName:DIRECT=OS:" + Environment.MachineName + @"\private$\test_servicebus";
		//    string output = ServiceBusSetupFixture.GetQueueName(input);
		//    Assert.AreEqual(@".\private$\test_servicebus", output);
		//}

		[Test]
		public void The_Service_Bus_Should_Return_False_If_The_Message_Will_Be_Handled_By_none_Of_the_handlers()
		{
			using (mocks.Record())
			{
				Expect.Call(mockServiceBusEndPoint.Uri).Return(new Uri("msmq://localhost/test_servicebus")).Repeat.Any(); //stupid log4net
			}
			using (mocks.Playback())
			{
				_serviceBus = new ServiceBus(mockServiceBusEndPoint);

				_serviceBus.Subscribe<PingMessage>(
					delegate { },
					delegate { return false; });

				_serviceBus.Subscribe<PingMessage>(
					delegate { },
					delegate { return false; });

				object message = new PingMessage();

				Assert.That(_serviceBus.Accept(message), Is.False);
			}
		}

		[Test]
		public void The_Service_Bus_Should_Return_False_If_The_Message_Will_Not_Be_Handled()
		{
			using (mocks.Record())
			{
				Expect.Call(mockServiceBusEndPoint.Uri).Return(new Uri("msmq://localhost/test_servicebus")).Repeat.Any(); //stupid log4net
			}
			using (mocks.Playback())
			{
				_serviceBus = new ServiceBus(mockServiceBusEndPoint);
				_serviceBus.Subscribe<PingMessage>(
					delegate { },
					delegate { return false; });

				object message = new PingMessage();

				Assert.That(_serviceBus.Accept(message), Is.False);
			}
		}

		[Test]
		public void The_Service_Bus_Should_Return_True_If_The_Message_Will_Be_Handled()
		{
			using (mocks.Record())
			{
				Expect.Call(mockServiceBusEndPoint.Uri).Return(new Uri("msmq://localhost/test_servicebus")).Repeat.Any(); //stupid log4net
			}
			using (mocks.Playback())
			{
				_serviceBus = new ServiceBus(mockServiceBusEndPoint);

				_serviceBus.Subscribe<PingMessage>(
					delegate { },
					delegate { return true; });

				object message = new PingMessage();

				Assert.That(_serviceBus.Accept(message), Is.True);
			}
		}

		[Test]
		public void The_Service_Bus_Should_Return_True_If_The_Message_Will_Be_Handled_By_One_Of_multiple_handlers()
		{
			using (mocks.Record())
			{
				Expect.Call(mockServiceBusEndPoint.Uri).Return(new Uri("msmq://localhost/test_servicebus")).Repeat.Any(); //stupid log4net
			}
			using (mocks.Playback())
			{
				_serviceBus = new ServiceBus(mockServiceBusEndPoint);

				_serviceBus.Subscribe<PingMessage>(
					delegate { },
					delegate { return false; });

				_serviceBus.Subscribe<PingMessage>(
					delegate { },
					delegate { return true; });

				object message = new PingMessage();

				Assert.That(_serviceBus.Accept(message), Is.True);
			}
		}

		private MockRepository mocks;
		private ServiceBus _serviceBus;
		private IEndpoint mockServiceBusEndPoint;
	}
}