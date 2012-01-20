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
	using System.Collections.Generic;
	using Magnum.TestFramework;
	using NUnit.Framework;
	using System.Linq;

	public class GivenALocalAddress
	{
		Uri Uri = new Uri("zmq-pgm://theserver:1234");
		ZeroMqAddress _addr;

		[When]
		public void WhenParsed()
		{
			_addr = ZeroMqAddress.Parse(Uri);
		}

		[Then]
		public void TheHost()
		{
			_addr.Host.ShouldEqual("theserver");
		}

		[Then]
		public void ThePort()
		{
			_addr.Port.ShouldEqual(1234);
		}

		[Then]
		public void IsLocal()
		{
			_addr.IsLocal.ShouldBeTrue();
		}

		[Then]
		public void IsTransactional()
		{
			_addr.IsTransactional.ShouldBeFalse();
		}

		[Then]
		public void Rebuilt()
		{
			_addr.PullSocket.ShouldEqual(new Uri("zmq-pgm://theserver:1234"));
		}
	}

	class Given_invalid_zmq_address
	{
		Uri _uri;

		[When]
		public void set_uri()
		{
			_uri = new Uri("zmq-tcp://comp:5555/");
		}

		[Then]
		public void ending_slash_forbidden()
		{
			ZeroMqAddress addr;
			IEnumerable<string> reasons;
			Assert.IsFalse(ZeroMqAddress.TryParse(
				_uri,
				out addr,
				out reasons
				));
			Assert.IsNull(addr);
			Assert.IsNotNull(reasons);
			Assert.IsTrue(
				reasons.Any(x => x.Contains("'/'")),
				"the reasons should include the faulty character"
				);
		}

		[Then]
		public void ending_slash_forbidden_Parse()
		{
			Assert.Throws<FormatException>(
				() => ZeroMqAddress.Parse(_uri)
				);
		}
	}
}