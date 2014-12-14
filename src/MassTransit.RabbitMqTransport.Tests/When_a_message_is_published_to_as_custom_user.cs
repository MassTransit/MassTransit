namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Diagnostics;
    using Magnum.Extensions;
    using NUnit.Framework;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;


    [Explicit("because it requires you to run a few commands first")]
	public class When_a_message_is_published_to_as_custom_user
		: Given_a_rabbitmq_bus_with_vhost_mt_and_credentials
	{
		Future<A> _received;
		Future<B> _receivedB;

		protected override void ConfigureServiceBus(System.Uri uri, BusConfigurators.ServiceBusConfigurator configurator)
		{
			base.ConfigureServiceBus(uri, configurator);

			_received = new Future<A>();
			_receivedB = new Future<B>();

//			configurator.Subscribe(s =>
//				{
////					s.Handler<A>(async message => _received.Complete(message.Message));
//	//				s.Handler<B>(async message => _receivedB.Complete(message.Message));
//				});
		}

		[SetUp]
		public void A_message_is_published()
		{
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

		[Test]
		public void Should_be_received_by_the_queue()
		{
			_received.WaitUntilCompleted(8.Seconds()).ShouldBe(true);
			_received.Value.StringA.ShouldBe("ValueA");
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