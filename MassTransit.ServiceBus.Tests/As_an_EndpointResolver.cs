namespace MassTransit.ServiceBus.Tests
{
	using System;
	using Internal;
	using NUnit.Framework;

	[TestFixture]
	public class As_an_EndpointResolver : 
        Specification
	{
		[Test]
		public void Be_able_to_resolve_endpoints()
		{
			EndpointResolver res = new EndpointResolver();
			res.Initialize();
			IEndpoint ep = res.Resolve(new Uri("msmq://localhost/test"));
			Assert.IsNotNull(ep);
		}

		[Test]
		public void Be_intializable()
		{
			EndpointResolver res = new EndpointResolver();
			res.Initialize();
		}
	}
}