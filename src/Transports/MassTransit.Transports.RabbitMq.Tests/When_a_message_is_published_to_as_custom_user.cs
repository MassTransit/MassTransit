namespace MassTransit.Transports.RabbitMq.Tests
{
	using System.Diagnostics;
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using NUnit.Framework;
	using TestFramework;

	[Scenario, Explicit("because it requires you to run a few commands first")]
	public class When_a_message_is_published_to_as_custom_user
		: Given_a_rabbitmq_bus_with_vhost_mt_and_credentials
	{
		Future<A> _received;
		Future<B> _receivedB;
		UnsubscribeAction _unsubscribe;

		[When]
		public void A_message_is_published()
		{
			_received = new Future<A>();
			_receivedB = new Future<B>();

			var unsub = LocalBus.SubscribeHandler<A>(message => _received.Complete(message));
			_unsubscribe = unsub += LocalBus.SubscribeHandler<B>(message => _receivedB.Complete(message));

			LocalBus.Publish(new A
			{
				StringA = "ValueA",
				StringB = "ValueB",
			});
		}

		[Test, Explicit]
		public void Can_set_up_config()
		{
			var run = new[]{
				"add_vhost mt",
				"add_user -p mt testUser topSecret",
				@"set_permissions -p mt testUser "".*"" "".*"" "".*"""
			};

			foreach (var s in run)
				Process.Start("rabbitmqctl.bat", s);
		}

		[Then]
		public void Should_be_received_by_the_queue()
		{
			_received.WaitUntilCompleted(8.Seconds()).ShouldBeTrue();
			_received.Value.StringA.ShouldEqual("ValueA");
		}

		[Finally]
		public void Final()
		{
			_unsubscribe();
		}

		class A :
			B
		{
			public string StringA { get; set; }
		}

		class B
		{
			public string StringB { get; set; }
		}
		
	}
}