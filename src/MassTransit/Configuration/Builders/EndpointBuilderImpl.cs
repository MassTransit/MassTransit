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
namespace MassTransit.Builders
{
	using System;
	using EndpointConfigurators;
	using Magnum;
	using Transports;
	using Util;

	public class EndpointBuilderImpl :
		EndpointBuilder
	{
		readonly Uri _uri;

		public EndpointBuilderImpl([NotNull] Uri uri, IEndpointDefaults defaults)
		{
			Guard.AgainstNull(uri, "uri");

			_uri = uri;
		}

		public IEndpoint CreateEndpoint(ITransportFactory transportFactory)
		{
			EndpointSettings endpointSettings = configurator.New(configureCallback);

			IDuplexTransport transport = transportFactory.BuildLoopback(endpointSettings.Normal);
			IOutboundTransport errorTransport = transportFactory.BuildError(endpointSettings.Error);

			var endpoint = new Endpoint(transport.Address, endpointSettings.Normal.Serializer, transport, errorTransport);

			return endpoint;
		}
	}
}