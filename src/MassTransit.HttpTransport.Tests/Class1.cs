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
namespace MassTransit.HttpTransport.Tests
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using HttpTransport;
    using NUnit.Framework;


    public class Class1
    {
        [Test]
        [Explicit]
        public void CheckProbe()
        {
            var bus = Bus.Factory.CreateUsingHttp(cfg =>
            {
                var mainHost = cfg.Host(new Uri("http://localhost:8080"));

                // TODO: As of now, you have to explicitly add each callers address.
                // No bueno
                var allowedOutboundCommunication = cfg.Host(new Uri("http://requestb.in"), host =>
                {
                    host.Method = HttpMethod.Put;
                });

                //http://localhost:8080/
                cfg.ReceiveEndpoint(ep =>
                {
                    ep.Consumer<HttpEater>();
                });
            });
        }

        [Test]
        [Explicit]
        public async Task Request()
        {
            var bus = Bus.Factory.CreateUsingHttp(cfg =>
            {
                var mainHost = cfg.Host(new Uri("http://localhost:8080"));
                //TODO: serializer?

                cfg.ReceiveEndpoint(ep =>
                {
                    ep.Consumer<HttpEater>();
                });
            });

            var mc = new MessageRequestClient<Ping, Pong>(bus, new Uri("http://requestb.in/15alnbk1"), TimeSpan.FromMinutes(500));

            await mc.Request(new Ping(), default(CancellationToken));
        }

        [Test]
        [Explicit]
        public void RespondTest()
        {
            var bus = Bus.Factory.CreateUsingHttp(cfg =>
            {
                var mainHost = cfg.Host(new Uri("http://localhost:8080"));
                //TODO: serializer?

                cfg.ReceiveEndpoint(ep =>
                {
                    ep.Consumer<HttpEater>();
                });
            });

            bus.StartAsync();

            Thread.Sleep(TimeSpan.FromMinutes(500));
        }

        [Test]
        [Explicit]
        public void SendACommand()
        {
            var bus = Bus.Factory.CreateUsingHttp(cfg =>
            {
                var mainHost = cfg.Host(new Uri("http://localhost:8080"));
                //TODO: serializer?

                var allowedOutboundCommunication = cfg.Host(new Uri("http://requestb.in"), host =>
                {
                    host.Method = HttpMethod.Put;
                    //TODO: Serializer
                });
            });

            var epa = bus.GetSendEndpoint(new Uri("http://requestb.in/15alnbk1")).Result;
            var r = epa.Send(new Ping {Hello = "Hal"}, CancellationToken.None).Wait(TimeSpan.FromMinutes(5));
            Console.WriteLine(r.ToString());
        }
    }


    public class HttpEater : IConsumer<Ping>
    {
        public async Task Consume(ConsumeContext<Ping> context)
        {
//            await Console.Out.WriteLineAsync(string.Format("Request-Id: {0}", context.RequestId));
//            await Console.Out.WriteLineAsync(string.Format("Conversation-Id: {0}", context.ConversationId));
//            await Console.Out.WriteLineAsync(string.Format("Initiator-Id: {0}", context.InitiatorId));
//            await Console.Out.WriteLineAsync(string.Format("Message-Id: {0}", context.MessageId));
//            await Console.Out.WriteAsync(context.Message.Hello);

            context.Respond(new Pong {Value = $"{context.Message.Hello}, World."});
        }
    }


    public class Pong
    {
        public string Value { get; set; }
    }


    public class Ping
    {
        public string Hello { get; set; }
    }
}