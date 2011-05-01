namespace MassTransit.Tests.Configuration
{
	using Examples.Sagas;
	using Examples.Sagas.Messages;
	using Magnum.TestFramework;
	using MassTransit.Saga;
	using MassTransit.Saga.Configuration;
	using Rhino.Mocks;
	using TestFramework;

	[Scenario]
	public class When_subscribing_a_state_machine_saga_to_the_bus
	{
		IServiceBus _bus;

		[When]
		public void Subscribing_a_consumer_to_the_bus()
		{
			_bus = ServiceBusFactory.New(x =>
				{
					x.ReceiveFrom("loopback://localhost/mt_test");

					x.Subscribe(s => s.Saga(MockRepository.GenerateMock<ISagaRepository<SimpleStateMachineSaga>>()));
				});
		}

		[Finally]
		public void Finally()
		{
			_bus.Dispose();
		}

		[Then]
		public void Should_have_subscribed_start_message()
		{
			_bus.ShouldHaveRemoteSubscriptionFor<StartSimpleSaga>();
		}

		[Then]
		public void Should_have_subscribed_approve_message()
		{
			_bus.ShouldHaveRemoteSubscriptionFor<ApproveSimpleCustomer>();
		}

		[Then]
		public void Should_have_subscribed_finish_message()
		{
			_bus.ShouldHaveRemoteSubscriptionFor<FinishSimpleSaga>();
		}
	}
}