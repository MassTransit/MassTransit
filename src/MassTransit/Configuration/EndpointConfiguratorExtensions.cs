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
namespace MassTransit
{
	using System;
	using EndpointConfigurators;
	using Magnum.Reflection;
	using Serialization;
	using Transports;
	using Util;

	public static class EndpointConfiguratorExtensions
	{
		/// <summary>
		/// Specify a serializer for this endpoint (overrides the default)
		/// </summary>
		/// <typeparam name="TSerializer"></typeparam>
		public static EndpointConfigurator UseSerializer<TSerializer>(this EndpointConfigurator configurator)
			where TSerializer : IMessageSerializer, new()
		{
			return configurator.UseSerializer(new TSerializer());
		}

		/// <summary>
		/// Specify a serializer for this endpoint (overrides the default)
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="serializerType"></param>
		public static EndpointConfigurator UseSerializer(this EndpointConfigurator configurator, Type serializerType)
		{
			return configurator.UseSerializer((IMessageSerializer) FastActivator.Create(serializerType));
		}

		/// <summary>
		/// Specifies a null transport for error messages
		/// </summary>
		/// <param name="configurator"></param>
		public static EndpointConfigurator DiscardFaultingMessages(this EndpointConfigurator configurator)
		{
			return configurator.SetErrorTransportFactory((factory, settings) => new NullOutboundTransport(settings.Address));
		}

		/// <summary>
		/// Overrides the default error address with a new error address
		/// </summary>
		/// <returns></returns>
		public static EndpointConfigurator SetErrorAddress(this EndpointConfigurator configurator, string uriString)
		{
			return configurator.SetErrorAddress(uriString.ToUri("Error URI was not valid"));
		}
	}
}