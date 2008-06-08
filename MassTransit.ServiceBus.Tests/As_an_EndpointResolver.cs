namespace MassTransit.ServiceBus.Tests
{
	using System;
	using System.Collections;
	using Internal;
	using NUnit.Framework;
	using Rhino.Mocks;

    [TestFixture]
	public class As_an_EndpointResolver : 
        Specification
	{
		[Test]
		public void Be_able_to_resolve_endpoints()
		{
            //TODO: need a better test
		    IObjectBuilder obj = StrictMock<IObjectBuilder>();
			EndpointResolver res = new EndpointResolver(obj);
			res.Initialize();
		    IEndpoint mock = DynamicMock<IEndpoint>();
            using(Record())
            {
                Expect.Call(obj.Build<IEndpoint>(new Hashtable())).Return(mock).IgnoreArguments();
            }
            using(Playback())
            {
                
			IEndpoint ep = res.Resolve(new Uri("msmq://localhost/test"));
			Assert.IsNotNull(ep);
            }
		}

		[Test]
		public void Be_intializable()
		{
		    IObjectBuilder obj = StrictMock<IObjectBuilder>();
			EndpointResolver res = new EndpointResolver(obj);
			res.Initialize();
		}
	}
}