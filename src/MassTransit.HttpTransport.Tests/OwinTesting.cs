namespace MassTransit.HttpTransport.Tests
{
    using System;
    using System.Net.Http;
    using System.Threading;
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

//            Thread.Sleep(10000);
            uu.Stop();

        }

        [Test]
        public void OwinHost()
        {
            var hostSettings = new HttpHostSettingsImpl("http", "localhost", 8080, HttpMethod.Get);
            //Here to make sure the assembly is loaded
            Microsoft.Owin.Host.HttpListener.OwinHttpListener x = null;
            using (var h = new RuntimeInstance(hostSettings))
            {
                h.Start(Pipe.Execute<ReceiveContext>(cxt =>
                {
                    Console.WriteLine("HTTPParty");
                }));
                
                Thread.Sleep(10000);
            }
        }
    }
}