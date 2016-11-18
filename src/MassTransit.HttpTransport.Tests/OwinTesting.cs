namespace MassTransit.HttpTransport.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using HottpTransport.Tests;
    using NUnit.Framework;


    public class OwinTesting
    {
        [Test]
        public async Task WholeBus()
        {
            var bus = Bus.Factory.CreateUsingHttp(cfg =>
            {
                var h = cfg.Host(new Uri("http://localhost:8080"));
                //                var slack = cfg.Host("http", "slack.com", 80);
//                var request = cfg.Host(new Uri("http://requestb.in"), host =>
//                {
//                    host.UseMethod(HttpMethod.Put);
//                    //TODO: Serializer
//                });

                //http://localhost:8080/
                cfg.ReceiveEndpoint(ep =>
                {
                    ep.Consumer<HttpEater>();
                });
            });
            
            var uu = await bus.StartAsync();

            Thread.Sleep(100000);
            uu.Stop();

        }

        [Test]
        public void OwinHost()
        {
            
        }
    }
}