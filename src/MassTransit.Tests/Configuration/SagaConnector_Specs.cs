namespace MassTransit.Tests.Configuration
{
	using System.Linq;
	using Magnum.TestFramework;
	using MassTransit.Saga;
	using MassTransit.Saga.SubscriptionConnectors;
	using Rhino.Mocks;
	using Saga;
	using SubscriptionConnectors;

	[Scenario]
	public class When_a_saga_is_inspected
	{
		SagaConnector<SimpleSaga> _factory;

		[When]
		public void A_consumer_with_consumes_all_interfaces_is_inspected()
		{
			_factory = new SagaConnector<SimpleSaga>(new InMemorySagaRepository<SimpleSaga>());
		}

		[Then]
		public void Should_create_the_builder()
		{
			_factory.ShouldNotBeNull();
		}

		[Then]
		public void Should_have_three_subscription_types()
		{
			_factory.Connectors.Count().ShouldEqual(3);
		}

		[Then]
		public void Should_have_an_a()
		{
			_factory.Connectors.First().MessageType.ShouldEqual(typeof(InitiateSimpleSaga));
		}

		[Then]
		public void Should_have_a_b()
		{
			_factory.Connectors.Skip(1).First().MessageType.ShouldEqual(typeof(CompleteSimpleSaga));
		}

		[Then]
		public void Should_have_a_c()
		{
			_factory.Connectors.Skip(2).First().MessageType.ShouldEqual(typeof(ObservableSagaMessage));
		}
	}
}
