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
namespace MassTransit.Transports.Msmq.Tests
{
	using System;
	using Exceptions;
	using NUnit.Framework;

	[TestFixture, Category("Integration")]
	public class Creating_an_endpoint_that_does_not_exist
	{
		[SetUp]
		public void Setup()
		{
			EndpointCacheFactory.ConfigureDefaultSettings(x =>
				{
					x.SetCreateMissingQueues(false);
					x.SetPurgeOnStartup(false);
				});

			_uri = new Uri("msmq://localhost/idontexist_tx");
		}

		Uri _uri;

		[Test]
		[ExpectedException(typeof (EndpointException))]
		public void Should_throw_an_endpoint_exception_from_the_endpoint_factory()
		{
			IEndpointCache endpointCache = EndpointCacheFactory.New(x => { x.AddTransportFactory<MsmqTransportFactory>(); });

			endpointCache.GetEndpoint(_uri);
		}

		[Test]
		[ExpectedException(typeof (TransportException))]
		public void Should_throw_an_endpoint_exception_from_the_msmq_endpoint_factory()
		{
			var transportFactory = new MsmqTransportFactory();
			var settings = new TransportSettings(new EndpointAddress(_uri));
			settings.CreateIfMissing = false;

			transportFactory.BuildInbound(settings);
		}
	}
}