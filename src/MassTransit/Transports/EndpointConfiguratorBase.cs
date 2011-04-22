// Copyright 2007-2011 The Apache Software Foundation.
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
namespace MassTransit.Transports
{
	using System;
	using Configuration;
	using Exceptions;
	using Magnum;
	using Magnum.Reflection;
	using Serialization;

	public class EndpointConfigurator :
		IEndpointConfigurator,
		ITransportSettings
	{
		private static readonly EndpointDefaults _defaults = new EndpointDefaults();
		private Func<IMessageSerializer> _serializerFactory;

		public EndpointConfigurator(Uri uri)
		{
			Guard.AgainstNull(uri, "uri");

			Address = new EndpointAddress(uri);

			CreateIfMissing = _defaults.CreateMissingQueues;
			PurgeExistingMessages = _defaults.PurgeOnStartup;
			Transactional = _defaults.CreateTransactionalQueues;
			TransactionTimeout = _defaults.TransactionTimeout;
			RequireTransactional = false;

			_serializerFactory = () => new CustomXmlMessageSerializer();
		}

		public void SetSerializer<T>()
			where T : IMessageSerializer
		{
			_serializerFactory = () =>
				{
					try
					{
						return FastActivator<T>.Create();
					}
					catch (Exception ex)
					{
						throw new ConfigurationException("Unable to create message serializer " + typeof (T).FullName, ex);
					}
				};
		}

		public void SetSerializer(IMessageSerializer serializer)
		{
			_serializerFactory = () => serializer;
		}

		public void SetSerializer(Type serializerType)
		{
			_serializerFactory = () =>
				{
					try
					{
						return (IMessageSerializer) FastActivator.Create(serializerType);
					}
					catch (Exception ex)
					{
						throw new ConfigurationException("Unable to create message serializer " + serializerType.FullName, ex);
					}
				};
		}

		public void SetUri(Uri uri)
		{
			Address = new EndpointAddress(uri);
		}

		public IEndpointAddress Address { get; private set; }

		public bool Transactional { get; private set; }

		public bool RequireTransactional { get; private set; }

		public TimeSpan TransactionTimeout { get; private set; }

		public bool CreateIfMissing { get; private set; }

		public bool PurgeExistingMessages { get; private set; }

		public EndpointSettings New(Action<IEndpointConfigurator> action)
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

				var endpointSettings = new EndpointSettings();

				endpointSettings.Normal = settings;
				endpointSettings.Error = new CreateEndpointSettings(settings.ErrorAddress, settings);

				return endpointSettings;
			}
			catch (Exception ex)
			{
				throw new EndpointException(Address.Uri, "Failed to create endpoint", ex);
			}
		}

		public static void Defaults(Action<IEndpointDefaults> configureDefaults)
		{
			configureDefaults(_defaults);
		}
	}
}