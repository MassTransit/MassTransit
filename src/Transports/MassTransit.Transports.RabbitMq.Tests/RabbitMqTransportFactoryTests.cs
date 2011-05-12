// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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

	[TestFixture]
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

		readonly IRabbitMqEndpointAddress _queue = RabbitMqEndpointAddress.Parse("rabbitmq://localhost/queue/dru");
		readonly IRabbitMqEndpointAddress _exchange = RabbitMqEndpointAddress.Parse("rabbitmq://localhost/exchange/dru");

		RabbitMqTransportFactory _factory;

		[Test, Explicit]
		public void CanConnect()
		{
			IDuplexTransport t = _factory.BuildLoopback(new TransportSettings(_queue));
			_factory.ConnectionCount().ShouldEqual(1);
		}


		[Test, Explicit]
		public void EndpointSendAndReceive()
		{
			using (var management = new RabbitMqEndpointManagement(_queue))
			{
				management.BindQueue(_queue.Name, _exchange.Name, ExchangeType.Fanout, "");
			}

			IMessageSerializer serializer = new XmlMessageSerializer();

			var message = new BugsBunny {Food = "Carrot"};

			IDuplexTransport transport = _factory.BuildLoopback(new TransportSettings(_exchange));
			var sendEndpoint = new Endpoint(_exchange, serializer, transport, null);
			sendEndpoint.Send(message);


			var receiveEndpoint = new Endpoint(_queue, serializer, transport, null);
			receiveEndpoint.Receive(o =>
				{
					return b =>
						{
							var bb = (BugsBunny) b;
							Console.WriteLine(bb.Food);
						};
				}, TimeSpan.Zero);
		}

		[Test, Explicit]
		public void TransportSendAndReceive()
		{
			using (var management = new RabbitMqEndpointManagement(_queue))
			{
				management.BindQueue(_queue.Name, _exchange.Name, ExchangeType.Fanout, "");
			}

			IOutboundTransport t = _factory.BuildOutbound(new TransportSettings(_exchange));
			var context = new SendContext<string>("dru");
			context.SetBodyWriter(stream =>
				{
					var buffer = Encoding.UTF8.GetBytes(context.Message);
					stream.Write(buffer, 0, buffer.Length);
				});
			t.Send(context);

			IInboundTransport i = _factory.BuildInbound(new TransportSettings(_queue));

			i.Receive(s =>
				{
					return ss =>
						{
							string name;
							using(var stream = new MemoryStream())
							{
								ss.CopyBodyTo(stream);

								name = Encoding.UTF8.GetString(stream.ToArray());
							}
							
							Assert.AreEqual("dru", name);
							Console.WriteLine(name);
						};
				}, 1.Minutes());
		}
	}

	[Serializable]
	public class BugsBunny
	{
		public string Food { get; set; }
	}
}