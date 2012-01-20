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
	using NUnit.Framework;
	using ZMQ;

	[TestFixture]
	public class X
	{
		Context _context;
		ZeroMqConnection _zmqc;

		[SetUp]
		public void SetUp()
		{
			_context = new Context();
			_zmqc = new ZeroMqConnection(_context,
				new ZeroMqAddress(new Uri("zmq-tcp://localhost:5555")),
				SocketType.REQ);
		}

		[TearDown]
		public void Teardown()
		{
			_zmqc.Dispose();
			_zmqc = null;

			_context.Dispose();
			_context = null;
		}

		[Test]
		public void BasicSmokeTest()
		{
		}

		[Test]
		public void ConnectTest()
		{
			_zmqc.Connect();
		}
	}
}