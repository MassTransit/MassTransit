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
namespace MassTransit.EndpointConfigurators
{
	using System;
	using Builders;
	using Exceptions;
	using Magnum;
	using Serialization;
	using Transports;

	public class EndpointConfiguratorImpl :
		EndpointConfigurator,
		EndpointFactoryBuilderConfigurator
	{
		readonly EndpointSettings _settings;
		readonly Uri _uri;
		IEndpointAddress _errorAddress;
		Func<ITransportFactory, ITransportSettings, IOutboundTransport> _errorTransportFactory;
		Func<ITransportFactory, ITransportSettings, IDuplexTransport> _transportFactory;

		public EndpointConfiguratorImpl(Uri uri, IEndpointFactoryDefaultSettings defaultSettings)
		{
			_uri = uri;
			Guard.AgainstNull(uri, "uri");

			_transportFactory = DefaultTransportFactory;
			_errorTransportFactory = DefaultErrorTransportFactory;

			_settings = defaultSettings.CreateEndpointSettings(uri);
		}

		public EndpointConfigurator UseSerializer(IMessageSerializer serializer)
		{
			_settings.Serializer = serializer;
			return this;
		}

		public EndpointConfigurator SetErrorAddress(Uri uri)
		{
			_errorAddress = new EndpointAddress(uri);
			return this;
		}

		public EndpointConfigurator SetErrorTransportFactory(
			Func<ITransportFactory, ITransportSettings, IOutboundTransport> errorTransportFactory)
		{
			_errorTransportFactory = errorTransportFactory;
			return this;
		}

		public void Validate()
		{
			if (_errorAddress != null)
			{
				if (string.Compare(_errorAddress.Uri.Scheme, _settings.Address.Uri.Scheme, true) != 0)
					throw new ConfigurationException("The error transport must be of the same type as the endpoint transport");
			}
		}

		public EndpointFactoryBuilder Configure(EndpointFactoryBuilder builder)
		{
			var endpointBuilder = CreateBuilder();

			builder.AddEndpointBuilder(_uri, endpointBuilder);

			return builder;
		}

		public EndpointBuilder CreateBuilder()
		{
			ITransportSettings errorSettings = new TransportSettings(_errorAddress ?? _settings.ErrorAddress, _settings);

			var endpointBuilder = new EndpointBuilderImpl(_uri, _settings, errorSettings, _transportFactory,
				_errorTransportFactory);

			return endpointBuilder;
		}

		static IDuplexTransport DefaultTransportFactory(ITransportFactory transportFactory, ITransportSettings settings)
		{
			return transportFactory.BuildLoopback(settings);
		}

		static IOutboundTransport DefaultErrorTransportFactory(ITransportFactory transportFactory, ITransportSettings settings)
		{
			return transportFactory.BuildError(settings);
		}
	}
}