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
namespace MassTransit.Transports.ZeroMq.Tests
{
	using System;
	using System.Text;
	using System.Threading;
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using NUnit.Framework;
	using ZMQ;

	[TestFixture]
	public class Inbound_Specs
	{
		InboundZeroMqTransport _inbound;
		Context _context;

		[SetUp]
		public void SetUp()
		{
			_context = new Context();
			var address = new ZeroMqAddress(new Uri("zmq-tcp://localhost:5555"));
			var zeroMqConnection = new ZeroMqConnection(_context, address, SocketType.SUB);
			ConnectionHandler<ZeroMqConnection> connection = new ConnectionHandlerImpl<ZeroMqConnection>(zeroMqConnection);
			_inbound = new InboundZeroMqTransport(address, connection, true);

			//push simple message in

			zeroMqConnection.Connect();

			using (var pub = _context.Socket(SocketType.PUB))
			{
				pub.Connect("zmq-tcp://localhost:5556");
				pub.Send("Hello World", Encoding.UTF8);
			}
		}

		[TearDown]
		public void TearDown()
		{
			_inbound.Dispose();
			_context.Dispose();
		}

		[Test]
		public void SmokeTest()
		{
		}

		[Test]
		public void CanRcv()
		{
			var mre = new ManualResetEvent(false);
			_inbound.Receive(cxt =>
				{
					return context =>
						{
							var x = context.BodyStream.ReadToEndAsText();
							x.ShouldEqual("dru");
							mre.Set();
						};
				}, 2.Seconds());
			mre.WaitOne(5.Seconds());
		}
	}
}