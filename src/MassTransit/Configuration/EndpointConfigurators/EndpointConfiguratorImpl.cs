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
		ITransportSettings,
		EndpointFactoryBuilderConfigurator
	{
		static readonly EndpointDefaults _defaults = new EndpointDefaults();
		readonly Uri _uri;
		Func<ITransportFactory, ITransportSettings, IOutboundTransport> _errorTransportFactory;
		Func<IMessageSerializer> _serializerFactory;

		public EndpointConfiguratorImpl(Uri uri)
		{
			_uri = uri;
			Guard.AgainstNull(uri, "uri");

			Address = new EndpointAddress(uri);

			CreateIfMissing = _defaults.CreateMissingQueues;
			PurgeExistingMessages = _defaults.PurgeOnStartup;
			Transactional = _defaults.CreateTransactionalQueues;
			TransactionTimeout = _defaults.TransactionTimeout;
			RequireTransactional = false;

			_serializerFactory = () => new XmlMessageSerializer();
		}


		public EndpointConfigurator UseSerializer(Func<IMessageSerializer> serializerFactory)
		{
			_serializerFactory = serializerFactory;
		}

		public EndpointConfigurator SetErrorTransportFactory(Func<ITransportFactory, ITransportSettings, IOutboundTransport> errorTransportFactory)
		{
			_errorTransportFactory = errorTransportFactory;
			return this;
		}

		public void Validate()
		{
			throw new NotImplementedException();
		}

		public EndpointFactoryBuilder Configure(EndpointFactoryBuilder builder)
		{
			var endpointBuilder = new EndpointBuilderImpl(_uri, builder.Defaults);

			builder.AddEndpointBuilder(_uri, endpointBuilder);

			return builder;
		}

		public IEndpointAddress Address { get; private set; }

		public bool Transactional { get; private set; }

		public bool RequireTransactional { get; private set; }

		public TimeSpan TransactionTimeout { get; private set; }

		public bool CreateIfMissing { get; private set; }

		public bool PurgeExistingMessages { get; private set; }

		public EndpointSettings New(Action<EndpointConfigurator> action)
		{
			try
			{
				action(this);

				IMessageSerializer serializer = _serializerFactory();
				Guard.AgainstNull(serializer, "The message serializer cannot be null");

				var settings = new CreateEndpointSettings(Address, serializer, this);

				Guard.AgainstNull(settings.Address, "An address for the endpoint must be specified");
				Guard.AgainstNull(settings.ErrorAddress, "An error address for the endpoint must be specified");
				Guard.AgainstNull(settings.Serializer, "A message serializer for the endpoint must be specified");

				var endpointSettings = new EndpointSettings
					{
						Normal = settings,
						Error = new CreateEndpointSettings(settings.ErrorAddress, settings)
					};

				return endpointSettings;
			}
			catch (Exception ex)
			{
				throw new EndpointException(Address.Uri, "Failed to create endpoint", ex);
			}
		}

		IEndpoint BuildEndpoint(Uri uri)
		{
			Action<EndpointConfigurator> configurator = BuildEndpointConfiguration(uri);

			return _factory.BuildEndpoint(uri, configurator);
		}

		Action<EndpointConfigurator> BuildEndpointConfiguration(Uri uri)
		{
			var key = new Uri(uri.ToString().ToLowerInvariant());

			return x =>
				{
					x.SetSerializer(_serializer);

					Action<EndpointConfigurator> endpointConfigurator;
					if (_endpointConfigurators.TryGetValue(key, out endpointConfigurator))
					{
						endpointConfigurator(x);
					}
				};
		}

		public static void Defaults(Action<IEndpointDefaults> configureDefaults)
		{
			configureDefaults(_defaults);
		}
	}
}