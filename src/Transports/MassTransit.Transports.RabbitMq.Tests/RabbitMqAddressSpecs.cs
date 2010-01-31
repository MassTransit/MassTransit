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
    using NUnit.Framework;
    using RabbitMQ.Client;
    using Serialization;

    [TestFixture]
    public class RabbitMqAddressSpecs
    {
        Uri _address = new Uri("rabbitmq://localhost/dru");
        Uri _rabbitAddress = new Uri("amqp-0-8://localhost:5672/dru");
        ConnectionFactory _factory = new ConnectionFactory();

		[Test, Category("Integration")]
        public void Send()
        {
            var t = new RabbitMqTransport(new EndpointAddress(_address), _factory.CreateConnection(_rabbitAddress));
            t.Send((s)=>
            {
                var b = Encoding.UTF8.GetBytes("dru");
                s.Write(b, 0,b.Length);
            });
        }

		[Test, Category("Integration")]
		public void Receive()
        {
            var t = new RabbitMqTransport(new EndpointAddress(new Uri("rabbitmq://localhost/dru")), _factory.CreateConnection(_rabbitAddress));
            t.Receive(s=>
            {
                return ss =>
                {
                    var buff = new byte[3];
                    ss.Read(buff, 0, buff.Length);
                    var name = Encoding.UTF8.GetString(buff);
                    Assert.AreEqual("dru", name);
                    Console.WriteLine(name);
                };
            });
        }

		[Test, Category("Integration")]
        public void EndpointSend()
        {
            var addr = new EndpointAddress(_address);

            IMessageSerializer ser = new XmlMessageSerializer();

            var msg = new BugsBunny() {Food = "Carrot"};

            using (var stream = new MemoryStream())
            {
                ser.Serialize(stream, msg);
            }
            var e = new RabbitMqEndpoint(addr, ser, new RabbitMqTransport(addr, _factory.CreateConnection(_rabbitAddress)), null);
            e.Send(new BugsBunny() {Food = "Carrot"});
        }

		[Test, Category("Integration")]
        public void EndpointReceive()
        {
            var addr = new EndpointAddress(_address);

            IMessageSerializer ser = new XmlMessageSerializer();


            var e = new RabbitMqEndpoint(addr, ser, new RabbitMqTransport(addr, _factory.CreateConnection(_rabbitAddress)), null);
            e.Receive(o=>
            {
                return b =>
                {
                    var bb = (BugsBunny) b;
                    Console.WriteLine(bb.Food);
                };
            });
        }
    }

    [Serializable]
    public class BugsBunny
    {
        public string Food { get; set;}
    }
}