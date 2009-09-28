namespace MassTransit.Transports.Tibco.Tests
{
	using System.Diagnostics;
	using System.Threading;
	using MassTransit.Tests.Performance;
	using NUnit.Framework;
	using TestFixtures;

	[TestFixture]
	public class PerformanceLoadTest :
		TibcoEndpointTestFixture
	{
		[Test]
		public void A_load_test()
		{
			ApartmentState state = Thread.CurrentThread.GetApartmentState();

			Trace.WriteLine("Apartment State: " + state);

			Thread.CurrentThread.SetApartmentState(ApartmentState.MTA);

			state = Thread.CurrentThread.GetApartmentState();

			Trace.WriteLine("Apartment State: " + state);

			EndpointLoadTest test = new EndpointLoadTest(LocalBus, 2000);

			test.Run();
		}
		
	}
}