namespace MassTransit.ServiceBus.MSMQ.Tests
{
	using System;
	using System.Threading;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class When_an_accept_method_throws_an_exception
	{
		[Test]
		public void The_exception_should_not_disrupt_the_flow_of_messages()
		{
			MsmqEndpoint endpoint = "msmq://localhost/test_servicebus";
		    IObjectBuilder obj = null;
			ServiceBus bus = new ServiceBus(endpoint, obj);

			CrashingService service = new CrashingService();

			bus.Subscribe(service);

			endpoint.Send(new BogusMessage());

			Assert.That(CrashingService.Received.WaitOne(TimeSpan.FromSeconds(5), true), Is.True, "No message received");

			CrashingService.Received.Reset();

			endpoint.Send(new LegitMessage());

			Assert.That(CrashingService.LegitReceived.WaitOne(TimeSpan.FromSeconds(5), true), Is.True, "No message received");
		}

		internal class CrashingService : Consumes<BogusMessage>.All, Consumes<LegitMessage>.All
		{
			public static ManualResetEvent Received
			{
				get { return _received; }
			}

			private static readonly ManualResetEvent _received = new ManualResetEvent(false);

			public static ManualResetEvent LegitReceived
			{
				get { return _legitReceived; }
			}

			private static readonly ManualResetEvent _legitReceived = new ManualResetEvent(false);

			public void Consume(BogusMessage message)
			{
				_received.Set();

				throw new NotImplementedException();
			}

			public void Consume(LegitMessage message)
			{
				_legitReceived.Set();
			}
		}

		[Serializable]
		internal class BogusMessage
		{
		}	

		[Serializable]
		internal class LegitMessage
		{
		}
	}
}