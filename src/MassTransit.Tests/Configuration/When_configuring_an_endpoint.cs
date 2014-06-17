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
namespace MassTransit.Tests.Configuration
{
	using Magnum.TestFramework;
	using MassTransit.Serialization;
	using MassTransit.Transports;
	using MassTransit.Transports.Loopback;

	[Scenario]
	public class When_configuring_an_endpoint
	{
		IEndpointCache _endpointCache;

		[When]
		public void Configuring_an_endpoint_serializer()
		{
			_endpointCache = EndpointCacheFactory.New(x =>
			    {
			        x.SetDefaultRetryLimit(5);
			        x.SetDefaultInboundMessageTrackerFactory(retryLimit => new InMemoryInboundMessageTracker(retryLimit));
					x.AddTransportFactory<LoopbackTransportFactory>();
					x.ConfigureEndpoint("loopback://localhost/mt_client", y =>
						{
							y.UseSerializer<XmlMessageSerializer>()
								.DiscardFaultingMessages();
						});
					x.ConfigureEndpoint("loopback://localhost/mt_other", y => { y.SetErrorAddress("loopback://localhost/mt_error"); });
				});
		}


		[Finally]
		public void Finally()
		{
			_endpointCache.Dispose();
		}

		[Then]
		public void Should_get_the_endpoint()
		{
			IEndpoint endpoint = _endpointCache.GetEndpoint("loopback://localhost/mt_client");
			endpoint.ShouldNotBeNull();
		}

		[Then]
		public void Should_be_an_endpoint_class_instance()
		{
			IEndpoint endpoint = _endpointCache.GetEndpoint("loopback://localhost/mt_client");
			var endpointClass = endpoint as Endpoint;
			endpointClass.ShouldNotBeNull();
		}

		[Then]
		public void Should_use_the_specified_message_serializer()
		{
			IEndpoint endpoint = _endpointCache.GetEndpoint("loopback://localhost/mt_client");
			var endpointClass = endpoint as Endpoint;
			IMessageSerializer serializer = endpointClass.Serializer;

			serializer.ShouldNotBeNull();
			serializer.ShouldBeAnInstanceOf<XmlMessageSerializer>();
		}

		[Then]
		public void Should_use_the_null_transport_for_faulting_messages()
		{
			IEndpoint endpoint = _endpointCache.GetEndpoint("loopback://localhost/mt_client");
			var endpointClass = endpoint as Endpoint;
			endpointClass.ErrorTransport.ShouldBeAnInstanceOf<NullOutboundTransport>();
		}

		[Then]
		public void Should_get_an_unconfigured_endpoint()
		{
			IEndpoint endpoint = _endpointCache.GetEndpoint("loopback://localhost/mt_server");
			endpoint.ShouldNotBeNull();
		}

		[Then]
		public void Should_use_the_default_serializer_for_unconfigured()
		{
			IEndpoint endpoint = _endpointCache.GetEndpoint("loopback://localhost/mt_server");
			var endpointClass = endpoint as Endpoint;
			IMessageSerializer serializer = endpointClass.Serializer;

			serializer.ShouldNotBeNull();
			serializer.ShouldBeAnInstanceOf<XmlMessageSerializer>();
		}

		[Then]
		public void Should_use_the_proper_transport_for_unconfigured_endpoint()
		{
			IEndpoint endpoint = _endpointCache.GetEndpoint("loopback://localhost/mt_server");
			var endpointClass = endpoint as Endpoint;
			endpointClass.ErrorTransport.ShouldBeAnInstanceOf<LoopbackTransport>();
		}

		[Then]
		public void Should_use_specific_address_for_errors()
		{
			IEndpoint endpoint = _endpointCache.GetEndpoint("loopback://localhost/mt_other");
			var endpointClass = endpoint as Endpoint;
			IOutboundTransport errorTransport = endpointClass.ErrorTransport;
			errorTransport.Address.Uri.ToString().ShouldEqual("loopback://localhost/mt_error");
		}
	}
}