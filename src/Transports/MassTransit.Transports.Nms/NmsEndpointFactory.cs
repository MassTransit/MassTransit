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
namespace MassTransit.Transports.Nms
{
	using System;
	using Exceptions;
	using Magnum;
	using Serialization;

	public static class NmsEndpointFactory
	{
		public static IEndpoint New(IEndpointAddress address, IMessageSerializer serializer)
		{
			return New(new CreateEndpointSettings(address)
				{
					Serializer = serializer,
				});
		}

		public static IEndpoint New(CreateEndpointSettings settings)
		{
			try
			{
				Guard.Against.Null(settings.Address, "An address for the endpoint must be specified");
				Guard.Against.Null(settings.ErrorAddress, "An error address for the endpoint must be specified");
				Guard.Against.Null(settings.Serializer, "A message serializer for the endpoint must be specified");

				var transport = new NmsTransport(settings.Address);

				var errorSettings = new CreateEndpointSettings(settings.ErrorAddress, settings);
				ITransport errorTransport = new NmsTransport(errorSettings.Address);

				var endpoint = new NmsEndpoint(settings.Address, settings.Serializer, transport, errorTransport);

				return endpoint;
			}
			catch (Exception ex)
			{
				throw new EndpointException(settings.Address.Uri, "Failed to create NMS endpoint", ex);
			}
		}
	}
}