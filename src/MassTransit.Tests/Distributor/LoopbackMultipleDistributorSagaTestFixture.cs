namespace MassTransit.Tests.Distributor
{
	using MassTransit.Transports;
	using MassTransit.Transports.Loopback;

	public class LoopbackMultipleDistributorSagaTestFixture :
		MultipleDistributorSagaTestFixture<LoopbackTransportFactory>
	{
	}
}