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
namespace MassTransit.Configuration
{
	using System;
	using Serialization;
	using Transports;

    /// <summary>
	/// Allows for the configuration of the EndpointFactory through the use of an EndpointFactoryConfigurator
	/// </summary>
	public interface IEndpointResolverConfigurator
	{
		/// <summary>
		/// Sets the object builder to use when creating objects. Also passed to endpoints when they are created.
		/// </summary>
		/// <param name="objectBuilder">An initialized instance of the object builder</param>
		void SetObjectBuilder(IObjectBuilder objectBuilder);

		/// <summary>
		/// Sets the default serializer for the endpoints (current default is BinaryMessageSerializer)
		/// </summary>
		/// <typeparam name="TSerializer">The class to use for serialization</typeparam>
		void SetDefaultSerializer<TSerializer>() where TSerializer : IMessageSerializer;

		/// <summary>
		/// Sets the default serializer for the endpoints (current default is BinaryMessageSerializer)
		/// </summary>
		void SetDefaultSerializer(Type serializerType);

		/// <summary>
		/// Register a transport so that it can be used to build endpoints for supported Uris
		/// </summary>
		/// <typeparam name="T">The class supporting the ConfigureEndpoint method</typeparam>
		void RegisterTransport<TTransportFactory>() where TTransportFactory : IEndpointFactory;

		/// <summary>
		/// Register a transport so that it can be used to build endpoints for supported Uris
		/// </summary>
		/// <param name="transportType">The type supporting the ConfigureEndpoint method</param>
		void RegisterTransport(Type transportFactoryType);

		/// <summary>
		/// Specifies a configuration action to perform on a particular endpoint when it is created.
		/// </summary>
		/// <param name="uriString">The Uri to be configured (matching is case insensitive)</param>
		/// <param name="action">The action to perform when the transport for the endpoint is created.</param>
		void ConfigureEndpoint(string uriString, Action<IEndpointConfigurator> action);

		/// <summary>
		/// Specifies a configuration action to perform on a particular endpoint when it is created.
		/// </summary>
		/// <param name="uri">The Uri to be configured</param>
		/// <param name="action">The action to perform when the transport for the endpoint is created.</param>
		void ConfigureEndpoint(Uri uri, Action<IEndpointConfigurator> action);
	}
}