// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Transports.RabbitMq.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using Magnum.TestFramework;
    using NUnit.Framework;
    using Serialization;
    using Magnum.Extensions;

    [TestFixture]
    public class RabbitMqTransportFactoryTests
    {
        Uri _queue = new Uri("rabbitmq://localhost:5672/queue/dru");
        Uri _exchange = new Uri("rabbitmq://localhost:5672/exchange/dru");

        RabbitMqTransportFactory _factory;

        [SetUp]
        public void Setup()
        {
            _factory = new RabbitMqTransportFactory();
        }
        [TearDown]
        public void Teardown()
        {
            _factory.Dispose();            
            _factory = null;
        }
        [Test, Explicit]
        public void CanConnect()
        {
            var t = _factory.BuildLoopback(new TransportSettings(RabbitMqAddress.ParseForInbound(_queue)));
            _factory.ConnectionsCount().ShouldEqual(1);

        }

		[Test, Explicit]
        public void TransportSendAndReceive()
		{
		    _factory.Bind(_queue, _exchange, "fanout");

		    var t = _factory.BuildOutbound(new TransportSettings(RabbitMqAddress.ParseForOutbound(_exchange)));
            t.Send((s)=>
            {
                var b = Encoding.UTF8.GetBytes("dru");
                s.Body.Write(b, 0,b.Length);
            });

		    var i = _factory.BuildInbound(new TransportSettings(RabbitMqAddress.ParseForInbound(_queue)));

            i.Receive(s=>
            {
                return ss =>
                {
                    var buff = new byte[3];
                    ss.Body.Read(buff, 0, buff.Length);
                    var name = Encoding.UTF8.GetString(buff);
                    Assert.AreEqual("dru", name);
                    Console.WriteLine(name);
                };
            }, 1.Minutes());
        }


		[Test,Explicit]
        public void EndpointSendAndReceive()
		{
            _factory.Bind(_queue, _exchange, "fanout");

		    var ex = RabbitMqAddress.ParseForOutbound(_exchange);
            
            IMessageSerializer ser = new XmlMessageSerializer();

            var msg = new BugsBunny() {Food = "Carrot"};

            using (var stream = new MemoryStream())
            {
                ser.Serialize(stream, msg);
            }

		    var lb = _factory.BuildLoopback(new TransportSettings(ex));
            var oe = new Endpoint(ex, ser, lb, null);
            oe.Send(msg);



		    var qu = RabbitMqAddress.ParseForInbound(_queue);
            var e = new Endpoint(qu, ser, lb, null);
            e.Receive(o=>
            {
                return b =>
                {
                    var bb = (BugsBunny) b;
                    Console.WriteLine(bb.Food);
                };
            }, TimeSpan.Zero);  
        }
    }

    [Serializable]
    public class BugsBunny
    {
        public string Food { get; set;}
    }
}