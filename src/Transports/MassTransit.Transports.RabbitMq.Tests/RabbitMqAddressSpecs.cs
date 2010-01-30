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
    using Serialization;

    [TestFixture]
    public class RabbitMqAddressSpecs
    {
        Uri _address = new Uri("rabbitmq://localhost/dru");
        [Test]
        public void AddressSpec()
        {
            var addr = new RabbitMqEndpointAddress(_address);
            Assert.AreEqual(new Uri("amqp-0-8://localhost:5672/dru"), addr.RabbitMqAddress);
        }

		[Test, Category("Integration")]
        public void Send()
        {
            var t = new RabbitMqTransport(new RabbitMqEndpointAddress(_address));
            t.Send((s)=>
            {
                var b = Encoding.UTF8.GetBytes("dru");
                s.Write(b, 0,b.Length);
            });
        }

		[Test, Category("Integration")]
		public void Receive()
        {
            var t = new RabbitMqTransport(new RabbitMqEndpointAddress(new Uri("rabbitmq://localhost/dru")));
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
            var addr = new RabbitMqEndpointAddress(_address);

            IMessageSerializer ser = new XmlMessageSerializer();

            var msg = new BugsBunny() {Food = "Carrot"};

            using (var stream = new MemoryStream())
            {
                ser.Serialize(stream, msg);
            }
            var e = new RabbitMqEndpoint(addr, ser, new RabbitMqTransport(addr), null);
            e.Send(new BugsBunny() {Food = "Carrot"});
        }

		[Test, Category("Integration")]
        public void EndpointReceive()
        {
            var addr = new RabbitMqEndpointAddress(_address);

            IMessageSerializer ser = new XmlMessageSerializer();

           
            var e = new RabbitMqEndpoint(addr, ser, new RabbitMqTransport(addr), null);
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