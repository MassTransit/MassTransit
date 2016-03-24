namespace MassTransit.HttpTransport.Tests
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using Configuration.Builders;
    using Hosting;
    using HottpTransport.Tests;
    using NUnit.Framework;


    public class OwinTesting
    {
        [Test]
        public void WholeBus()
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

                //http://localhost:8080/listen_here
                cfg.ReceiveEndpoint("listen_here", ep =>
                {
                    ep.Consumer<HttpEater>();
                });
            });
            
            var uu = bus.Start();

            Thread.Sleep(10000);
            uu.Stop();

        }

        [Test]
        public void OwinHost()
        {
            
        }
    }
}