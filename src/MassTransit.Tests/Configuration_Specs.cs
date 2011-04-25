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
namespace MassTransit.Tests
{
	using Magnum.TestFramework;
	using MassTransit.Serialization;
	using MassTransit.Transports;
	using NUnit.Framework;

	[TestFixture]
	public class Configuration_Specs
	{
		[Test]
		public void Setting_a_specific_message_serializer_on_the_endpoint_should_work()
		{
			IEndpointCache endpointCache = EndpointCacheFactory.New(x =>
				{
					x.AddTransportFactory<LoopbackTransportFactory>();
					x.ConfigureEndpoint("loopback://localhost/mt_client", y => y.UseSerializer<DotNotXmlMessageSerializer>());
				});

			IEndpoint endpoint = endpointCache.GetEndpoint("loopback://localhost/mt_client");
			endpoint.ShouldNotBeNull();

			Endpoint endpointClass = endpoint as Endpoint;
			endpointClass.ShouldNotBeNull();

			IMessageSerializer serializer = endpointClass.Serializer;
			serializer.ShouldNotBeNull();
			serializer.ShouldBeAnInstanceOf<DotNotXmlMessageSerializer>();
		}
	}
}