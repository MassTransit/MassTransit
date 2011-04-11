namespace MassTransit.Tests.Distributor
{
	using MassTransit.Transports;

	public class LoopbackMultipleDistributorSagaTestFixture :
		MultipleDistributorSagaTestFixture<LoopbackTransportFactory>
	{
	}
}