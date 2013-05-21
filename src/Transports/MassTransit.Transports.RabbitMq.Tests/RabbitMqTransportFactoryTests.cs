// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Context;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using Management;
    using NUnit.Framework;
    using RabbitMQ.Client;
    using Serialization;


    [TestFixture, Explicit("integration tests and require custom configuration")]
    public class RabbitMqTransportFactoryTests
    {
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

        [Test]
        public void CanConnect()
        {
            IDuplexTransport t = _factory.BuildLoopback(new TransportSettings(_queue));
            _factory.ConnectionCount().ShouldEqual(1);
        }

        [Test]
        public void EndpointSendAndReceive()
        {
            using (var management = new RabbitMqEndpointManagement(_queue))
            {
                management.BindQueue(_queue.Name, _exchange.Name, ExchangeType.Fanout, "", null);
            }

            IMessageSerializer serializer = new XmlMessageSerializer();

            var message = new BugsBunny {Food = "Carrot"};

            IDuplexTransport transport = _factory.BuildLoopback(new TransportSettings(_exchange));
            IOutboundTransport error = _factory.BuildError(new TransportSettings(_error));

            var messageSerializers = new SupportedMessageSerializers();
            messageSerializers.AddSerializer(serializer);

            var sendEndpoint = new Endpoint(_exchange, serializer, transport, error,
                new InMemoryInboundMessageTracker(5), messageSerializers);
            sendEndpoint.Send(message);


            var receiveEndpoint = new Endpoint(_queue, serializer, transport, error,
                new InMemoryInboundMessageTracker(5), messageSerializers);
            receiveEndpoint.Receive(o =>
                {
                    return b =>
                        {
                            var bb = (BugsBunny)b;
                            Console.WriteLine(bb.Food);
                        };
                }, TimeSpan.Zero);
        }

        [Test]
        public void TransportSendAndReceive()
        {
            using (var management = new RabbitMqEndpointManagement(_queue))
            {
                management.BindQueue(_queue.Name, _exchange.Name, ExchangeType.Fanout, "", null);
            }

            IOutboundTransport t = _factory.BuildOutbound(new TransportSettings(_exchange));
            var context = new SendContext<string>("dru");
            context.SetBodyWriter(stream =>
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(context.Message);
                    stream.Write(buffer, 0, buffer.Length);
                });
            t.Send(context);

            IInboundTransport i = _factory.BuildInbound(new TransportSettings(_queue));

            i.Receive(s =>
                {
                    return ss =>
                        {
                            string name;
                            using (var stream = new MemoryStream())
                            {
                                ss.CopyBodyTo(stream);

                                name = Encoding.UTF8.GetString(stream.ToArray());
                            }

                            Assert.AreEqual("dru", name);
                            Console.WriteLine(name);
                        };
                }, 1.Minutes());
        }

        // need to configure mt vhost for this:
        readonly IRabbitMqEndpointAddress _queue =
            RabbitMqEndpointAddress.Parse("rabbitmq://guest:guest@localhost:5672/mt/mt-unit-tests");

        readonly IRabbitMqEndpointAddress _exchange =
            RabbitMqEndpointAddress.Parse("rabbitmq://guest:guest@localhost:5672/mt/dru");

        readonly IRabbitMqEndpointAddress _error =
            RabbitMqEndpointAddress.Parse("rabbitmq://guest:guest@localhost:5672/mt/mt-unit-tests-error");

        RabbitMqTransportFactory _factory;
    }


    [Serializable]
    public class BugsBunny
    {
        public string Food { get; set; }
    }
}