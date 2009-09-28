namespace MassTransit.Infrastructure.Tests.Sagas
{
	using MassTransit.Tests.TextFixtures;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class SagaLoadTest :
		LoopbackTestFixture
	{



		[Test]
		public void Put_some_stress_on_the_saga_dispatcher_to_see_how_it_handles_multiple_sagas_at_once()
		{



			
		}
	}
}