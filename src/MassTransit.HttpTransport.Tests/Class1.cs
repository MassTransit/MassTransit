// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.HottpTransport.Tests
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;


    public class Class1
    {
        [Test]
        public void ConfigurationOptions()
        {
            var bus = Bus.Factory.CreateUsingHttp(cfg =>
            {
                var h = cfg.Host(new Uri("http://localhost:8080"));
//                var slack = cfg.Host("http", "slack.com", 80);
                var request = cfg.Host(new Uri("http://requestb.in"), host =>
                {
                    host.UseMethod(HttpMethod.Put);
                    //TODO: Serializer
                });

                //http://localhost:8080/listen_here
                cfg.ReceiveEndpoint("listen_here", ep =>
                {
                    ep.Consumer<HttpEater>();
                });
            });

            var epa = bus.GetSendEndpoint(new Uri("http://requestb.in/1jgiiy51")).Result;
            var r = epa.Send(new Ping {Hello = "Hal"}, CancellationToken.None).Wait(TimeSpan.FromMinutes(5));
            Console.WriteLine(r.ToString());

            var p = bus.GetProbeResult();
            Console.WriteLine(p.ToJsonString());
        }
    }


    public class HttpEater : IConsumer<Ping>
    {
        public async Task Consume(ConsumeContext<Ping> context)
        {
            await Console.Out.WriteAsync(context.Message.Hello);
        }
    }


    public class Ping
    {
        public string Hello { get; set; }
    }
}