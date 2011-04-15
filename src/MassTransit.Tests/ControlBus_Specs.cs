namespace MassTransit.Tests
{
	using Configuration;
	using MassTransit.Internal;
	using MassTransit.Services.Subscriptions;
	using MassTransit.Transports;
	using NUnit.Framework;
	using Rhino.Mocks;
	using TextFixtures;

	[TestFixture]
	public class When_creating_a_bus_with_a_separate_control_bus :
		EndpointTestFixture<LoopbackTransportFactory>
	{
		public ISubscriptionService SubscriptionService { get; private set; }
		public IServiceBus LocalBus { get; private set; }
		public IControlBus LocalControlBus { get; private set; }
		public IServiceBus RemoteBus { get; private set; }
		public IControlBus RemoteControlBus { get; private set; }

		protected override void EstablishContext()
		{
			base.EstablishContext();

			SetupSubscriptionService();

			LocalControlBus = ControlBusConfigurator.New(x =>
			{
				x.ReceiveFrom("loopback://localhost/mt_client_control");

				x.PurgeBeforeStarting();
			});

			RemoteControlBus = ControlBusConfigurator.New(x =>
			{
				x.ReceiveFrom("loopback://localhost/mt_server_control");

				x.PurgeBeforeStarting();
			});

			LocalBus = ServiceBusConfigurator.New(x =>
			{
				x.AddService<SubscriptionPublisher>();
				x.AddService<SubscriptionConsumer>();
				x.ReceiveFrom("loopback://localhost/mt_client");
				x.UseControlBus(LocalControlBus);
			});

			RemoteBus = ServiceBusConfigurator.New(x =>
			{
				x.AddService<SubscriptionPublisher>();
				x.AddService<SubscriptionConsumer>();
				x.ReceiveFrom("loopback://localhost/mt_server");
				x.UseControlBus(RemoteControlBus);
			});
		}

		protected override void TeardownContext()
		{
			RemoteBus.Dispose();
			RemoteBus = null;

			RemoteControlBus.Dispose();
			RemoteControlBus = null;

			LocalBus.Dispose();
			LocalBus = null;

			LocalControlBus.Dispose();
			LocalControlBus = null;

			SubscriptionService = null;

			base.TeardownContext();
		}

		private void SetupSubscriptionService()
		{
			SubscriptionService = new LocalSubscriptionService();
			ObjectBuilder.Stub(x => x.GetInstance<IEndpointSubscriptionEvent>())
				.Return(SubscriptionService);

			ObjectBuilder.Stub(x => x.GetInstance<SubscriptionPublisher>())
				.Return(null)
				.WhenCalled(invocation =>
				{
					// Return a unique instance of this class
					invocation.ReturnValue = new SubscriptionPublisher(SubscriptionService);
				});

			ObjectBuilder.Stub(x => x.GetInstance<SubscriptionConsumer>())
				.Return(null)
				.WhenCalled(invocation =>
				{
					// Return a unique instance of this class
					invocation.ReturnValue = new SubscriptionConsumer(SubscriptionService, EndpointResolver);
				});
		}

		[Test]
		public void Should_purge_messages_on_startup_if_specified()
		{
			
		}
	}
}